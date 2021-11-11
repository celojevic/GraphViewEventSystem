using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class EventGraphParser : MonoBehaviour
{

    public string fileName = "NewEventGraph";

    [SerializeField] private int _testVal = 4;

    private Dictionary<string, NodeDataBase> nodes = new Dictionary<string, NodeDataBase>();
    private string _curNodeGuid;

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
        _curNodeGuid = entryNodeData.edges[0].toNodeGuid;

    }

    void Next()
    {
        // construct the data from nodeDataType
        // call Parse() on data
        // for cndNodes, have Parse() call EvaluateCnd()

        if (nodes[_curNodeGuid].nodeType == nameof(ChoiceNode))
        {
            HandleChoiceNode();
        }
        else if (nodes[_curNodeGuid].nodeType == nameof(WaitNode))
        {
            HandleWaitNode();
        }
        else if (nodes[_curNodeGuid].nodeType == nameof(IntCompareNode))
        {
            HandleConditionalNode();
        }

    }

    // TODO make generic
    void HandleConditionalNode()
    {
        // create data class from type string
        Type dataType = Type.GetType(nodes[_curNodeGuid].nodeDataType);
        var nodeData = Activator.CreateInstance(dataType, nodes[_curNodeGuid]);

        // get the evaluation result by passing in the appropriate comparison value
        object[] parameters = new object[] { _testVal };
        bool result = (bool)nodeData.GetType().GetMethod("EvaluateCondition").Invoke(nodeData, parameters);


        // move next based on result

    }

    void HandleWaitNode()
    {
        WaitNodeData data = nodes[_curNodeGuid] as WaitNodeData;
        StartCoroutine(WaitNodeCo(data));
    }

    IEnumerator WaitNodeCo(WaitNodeData data)
    {
        yield return new WaitForSeconds(data.timeToWait);

        _curNodeGuid = data.edges[0].toNodeGuid;
        Next();
    }

    void HandleChoiceNode()
    {
        ChoiceNodeData choiceData = nodes[_curNodeGuid] as ChoiceNodeData;
        List<ChoiceAction> choices = new List<ChoiceAction>();
        for (int i = 0; i < choiceData.choices.Count; i++)
        {
            var index = i;
            choices.Add(new ChoiceAction
            {
                choice = choiceData.choices[i],
                callback = () =>
                {
                    if (choiceData.edges.Count <= index)
                    {
                        Debug.LogWarning("ChoiceNode with edge at index doesn't go anywhere: " + index);
                        return;
                    }
                    _curNodeGuid = choiceData.edges[index].toNodeGuid;
                    Next();
                }
            });
        }

        UIDialogue.instance.ShowMessage(choiceData.message, choices);
    }

}

public struct ChoiceAction
{
    public string choice;
    public Action callback;
}
