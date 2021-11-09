using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class EventGraphParser : MonoBehaviour
{

    public string fileName = "NewEventGraph";

    private Dictionary<string, NodeDataBase> nodes = new Dictionary<string, NodeDataBase>();

    private void Start()
    {
        LoadFile();
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

        Debug.Log(nodes.Count);

    }

}
