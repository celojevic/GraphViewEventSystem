using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO can use RuntimeInitializeOnLoadMethod and make this a regular class?
public class EventGraphParser : MonoBehaviour
{

    public string curNodeGuid { get; set; }
    public NodeDataBase curNodeData => nodes.ContainsKey(curNodeGuid) ? nodes[curNodeGuid] : null;

    public string fileName = "NewEventGraph";

    [SerializeField] private int _testVal = 4;

    private Dictionary<string, NodeDataBase> nodes = new Dictionary<string, NodeDataBase>();

    private void Start()
    {
        // TODO load all events into memory on start
        //      cache in dict<EventGraphData, fileName string>
        LoadFile();
        Next();
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
        
        EntryNodeData entryNodeData = JsonUtility.FromJson<EntryNodeData>(data.entryNode);
        curNodeGuid = entryNodeData.edges[0].toNodeGuid;

    }

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
        NodeDataBase nodeData = (NodeDataBase)Activator.CreateInstance(dataType, curNodeData);

        // get the evaluation result by passing in the appropriate comparison value
        object[] parameters = new object[] { _testVal };
        bool result = (bool)nodeData.GetType().GetMethod("EvaluateCondition").Invoke(nodeData, parameters);

        if (result)
        {
            // 0 is always true port
            curNodeGuid = nodeData.edges[0].toNodeGuid;
        }
        else
        {
            // 1 is always false port
            curNodeGuid = nodeData.edges[1].toNodeGuid;
        }

        Next();
    }

}

public struct ChoiceAction
{
    public string choice;
    public Action callback;
}
