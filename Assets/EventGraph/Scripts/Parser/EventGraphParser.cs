using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO can use RuntimeInitializeOnLoadMethod and make this a regular class?
public class EventGraphParser : MonoBehaviour
{

    public Button talkButton;

    public string curNodeGuid { get; set; }
    public NodeDataBase curNodeData => nodes.ContainsKey(curNodeGuid) ? nodes[curNodeGuid] : null;

    public string fileName = "NewEventGraph";

    private Dictionary<string, NodeDataBase> nodes = new Dictionary<string, NodeDataBase>();

    private EntryNodeData _entryNodeData;

    private void Start()
    {
        // TODO load all events into memory on start
        //      cache in dict<EventGraphData, fileName string>
        LoadFile();

        talkButton.onClick.AddListener(() => Next());
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
            Type dataType = Type.GetType(nodeData.nodeDataType);

            nodes.Add(nodeData.guid, (NodeDataBase)JsonUtility.FromJson(json, dataType));
        }

        _entryNodeData = JsonUtility.FromJson<EntryNodeData>(data.entryNode);
        ResetToFirstNode();
    }

    void ResetToFirstNode()=> curNodeGuid = _entryNodeData.edges[0].toNodeGuid;

    public void Next()
    {
        Type dataType = Type.GetType(curNodeData.nodeDataType);
        NodeDataBase nodeData = (NodeDataBase)Activator.CreateInstance(dataType, nodes[curNodeGuid]);

        if (nodeData.GetType().BaseType == typeof(NodeDataBase))
        {
            nodeData.Parse(this);
        }
        else // BaseType is ConditionalNodeData<>
        {
            HandleConditionalNode();
        }

    }

    void HandleConditionalNode()
    {
        // create data class from type string
        Type dataType = Type.GetType(curNodeData.nodeDataType);
        NodeDataBase cndNodeData = (NodeDataBase)Activator.CreateInstance(dataType, curNodeData);

        // get the var node  if it exists
        // then pass it to the evalCnd
        int varToCompare = 0;
        bool found = false;
        if (nodes.TryGetValue(
            cndNodeData?.GetType()?.GetField("valueNodeGuid")?.GetValue(cndNodeData).ToString(),
            out NodeDataBase varNodeData))
        {

            // TODO make generic
            string soGuid = varNodeData.GetType().GetField("soGuid").GetValue(varNodeData).ToString();
            var list = EventGraphEditorUtils.FindScriptableObjects<IntVariable>();
            foreach (var soVar in list)
            {
                if (soVar.guid == soGuid)
                {
                    varToCompare = soVar.value;
                    found = true;
                }
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
