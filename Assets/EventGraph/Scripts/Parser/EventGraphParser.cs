using EventGraph.Databases;
using EventGraph.Runtime.UI;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace EventGraph.Runtime
{
    // TODO can use RuntimeInitializeOnLoadMethod and make this a regular class that returns parsed values?
    // TODO need a callback thing for when parsing begins and ends to reset vars and such
    //      - such as shakers on the dialogue boxes
    public class EventGraphParser : MonoBehaviour
    {

        public Button talkButton;
        public string fileName = "NewEventGraph";
        public bool autoParse = false;

        public string curNodeGuid { get; set; }
        public NodeDataBase curNodeData => nodes.ContainsKey(curNodeGuid) ? nodes[curNodeGuid] : null;

        private Dictionary<string, NodeDataBase> nodes = new Dictionary<string, NodeDataBase>();
        private EntryNodeData _entryNodeData;

        private void Start()
        {
            // TODO load all events into memory on start/awake
            //      cache in dict<EventGraphData, fileName string>
            LoadFile();

            talkButton?.onClick.AddListener(() => Next());
        }

        private void Update()
        {
            if (autoParse)
            {

            }
        }

        void LoadFile()
        {
            EventGraphData data = EventGraphSaver.LoadGraphDataJson(fileName);

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
        public void SetNext()
        {
            try
            {
                curNodeGuid = curNodeData.edges[0].toNodeGuid;
                Next();
            }
            catch
            {
                StopParsing();
            }
        }

        void HandleConditionalNode(NodeDataBase cndNodeData)
        {
            // get the var node  if it exists, then pass it for evaluation
            object varToCompare = 0;
            bool found = false;

            if (nodes.TryGetValue(
                cndNodeData?.GetType()?.GetField("valueNodeGuid")?.GetValue(cndNodeData).ToString(),
                out NodeDataBase varNodeData))
            {
                string soGuid = varNodeData.GetType().GetField("soGuid").GetValue(varNodeData).ToString();
                string varTypeName = varNodeData.GetType().GetProperty("variableTypeName").GetValue(varNodeData).ToString();

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
                StopParsing();
                return;
            }

            // get the evaluation result by passing in the appropriate comparison value
            bool result = (bool)cndNodeData.GetType().GetMethod("EvaluateCondition")
                .Invoke(cndNodeData, new object[] { varToCompare });
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
        public Action callback;
    }

}
