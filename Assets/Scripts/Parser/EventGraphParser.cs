using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class EventGraphParser : MonoBehaviour
{

    public string fileName = "NewEventGraph";

    private Dictionary<string, NodeDataBase> nodes = new Dictionary<string, NodeDataBase>();
    [SerializeField] private string _curNodeGuid = "";

    private void Start()
    {
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
            NodeDataBase nodeData = (NodeDataBase)JsonUtility.FromJson(
                json, typeof(NodeDataBase));

            // get save type and load data as parent
            Type dataType = Type.GetType(nodeData.nodeDataType);

            nodes.Add(nodeData.guid, (NodeDataBase)JsonUtility.FromJson(json, dataType));
        }
        
        EntryNodeData entryNodeData = JsonUtility.FromJson<EntryNodeData>(data.entryNode);
        _curNodeGuid = entryNodeData.edges[0].toNodeGuid;

    }

    void Next()
    {
        if (nodes[_curNodeGuid].nodeType == nameof(ChoiceNode))
        {
            HandleChoiceNode();
        }
        else if (nodes[_curNodeGuid].nodeType == nameof(WaitNode))
        {
            Debug.Log("Wait");
        }

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
