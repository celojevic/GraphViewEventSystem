using EventGraph.Databases;
using EventGraph.Runtime.UI;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#if FISHNET
using FishNet.Object;
#endif

namespace EventGraph.Runtime
{
    public class EventGraphParser
    {

        public string fileName = "NewEventGraph";

        public string curNodeGuid { get; set; }
        public NodeDataBase curNodeData => nodes.ContainsKey(curNodeGuid) ? nodes[curNodeGuid] : null;

        private Dictionary<string, NodeDataBase> nodes = new Dictionary<string, NodeDataBase>();
        private EntryNodeData _entryNodeData;

#if FISHNET
        /// <summary>
        /// Parent interactable. Server only.
        /// </summary>
        public Interactable Parent { get; [Server]set; }
        /// <summary>
        /// Parent only exists on server, so its basically a server check
        /// </summary>
        public bool IsServer => Parent != null;
        /// <summary>
        /// Exists on both
        /// </summary>
        public Player Player { get; set; }
#endif

        public EventGraphParser(EventGraphData data)
        {
            ParseGraphData(data);
        }

        public void ParseGraphData(EventGraphData data)
        {
            for (int i = 0; i < data.nodeJsons.Count; i++)
            {
                string json = data.nodeJsons[i];

                // is it a node?
                if (!json.Contains("nodeType")) continue;

                // load as base type to get nodeType info
                NodeDataWrapper nodeData = (NodeDataWrapper)JsonUtility.FromJson(
                    json, typeof(NodeDataWrapper));

                // get save type and load data as parent
                Type dataType = Type.GetType(EventGraphSaver.AppendNamespace(nodeData.nodeDataType));

                nodes.Add(nodeData.guid, (NodeDataBase)JsonUtility.FromJson(json, dataType));
            }

            _entryNodeData = JsonUtility.FromJson<EntryNodeData>(data.entryNode);
            ResetToFirstNode();
        }

        void ResetToFirstNode() => curNodeGuid = _entryNodeData.edges[0].toNodeGuid;

        public void StartParsing()
        {
            ResetToFirstNode();
            Next();
        }

        public void Next()
        {
            if (!nodes.TryGetValue(curNodeGuid, out NodeDataBase nodeData))
            {
                Debug.LogError("No node with guid: " + curNodeGuid);
                return;
            }

            ParseNodeType(nodeData);
        }

        public void ParseNode(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogError("Node guid was null");
                return;
            }
            if (!nodes.TryGetValue(guid, out NodeDataBase nodeData))
            {
                Debug.LogError("No node with guid: " + guid);
                return;
            }
            if (nodeData == null)
            {
                Debug.LogError("Cached node data was null, guid: " + guid);
                return;
            }

            ParseNodeType(nodeData);
        }

        private void ParseNodeType(NodeDataBase nodeData)
        {
            if (nodeData.GetType().BaseType == typeof(NodeDataBase))
            {
                nodeData.Parse(this);
            }
            else // BaseType is ConditionalNodeData<>
            {
                HandleConditionalNode(nodeData);
            }
        }

        /// <summary>
        /// Called from node data classes to progress the event.
        /// </summary>
        public bool SetNext()
        {
            try
            {
                curNodeGuid = curNodeData.edges[0].toNodeGuid;
                Next();
                return true;
            }
            catch
            {
                StopParsing();
                return false;
            }
        }

        void HandleConditionalNode(NodeDataBase cndNodeData)
        {
            // get the var node  if it exists
            // then pass it to the evalCnd
            object varToCompare = default;
            bool found = false;
            if (nodes.TryGetValue(
                cndNodeData?.GetType()?.GetField("valueNodeGuid")?.GetValue(cndNodeData).ToString(),
                out NodeDataBase varNodeData))
            {
                string soGuid = varNodeData.GetType().GetField("soGuid").GetValue(varNodeData).ToString();

                var variable = EventGraph.Databases.Database.GetVariable(soGuid);
                if (variable != null)
                {
                    varToCompare = variable.GetType().GetField("value").GetValue(variable);
                    found = true;
                }
            }

            if (!found)
            {
                Debug.LogWarning("No var node attached to cnd Node's value port");
                //StopParsing();
                //return;
            }

            // get the evaluation result by passing in the appropriate comparison value
            bool result = (bool)cndNodeData.GetType().GetMethod("EvaluateCondition")
                .Invoke(cndNodeData, new object[] { this, varToCompare });
            if (result)
            {
                // 0 is always true port
                curNodeGuid = cndNodeData.edges[0].toNodeGuid;
            }
            else
            {
                // 1 is always false port
                curNodeGuid = cndNodeData.edges[1].toNodeGuid;
            }

            Next();
        }

        public void StopParsing()
        {
            UIDialogue.instance.Hide();
            ResetToFirstNode();
        }

    }

    public struct ChoiceAction
    {
        public string choice;
        /// <summary>
        /// Bool = asServer if networking. Irrelevent otherwise
        /// </summary>
        public Action<bool> callback;
    }

}
