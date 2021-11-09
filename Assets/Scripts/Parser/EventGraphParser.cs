using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class EventGraphParser : MonoBehaviour
{

    public string fileName = "NewEventGraph";

    private Dictionary<string, NodeSaveDataBase> nodes = new Dictionary<string, NodeSaveDataBase>();

    private void Start()
    {
        LoadFile();
    }

    void LoadFile()
    {
        EventGraphSaveData data = EventGraphSaver.LoadGraphDataJson(fileName);

        for (int i = 0; i < data.nodeJsons.Count; i++)
        {
            string json = data.nodeJsons[i];

            // is it a node?
            if (!json.Contains("nodeType")) continue;

            // load as base type to get nodeType info
            NodeSaveDataBase nodeData = (NodeSaveDataBase)JsonUtility.FromJson(
                json, typeof(NodeSaveDataBase));


            // get save type and load data as parent
            Type saveDataType = Type.GetType(nodeData.nodeSaveDataType);
            var saveData = JsonUtility.FromJson(json, saveDataType);

            nodes.Add(nodeData.guid, (NodeSaveDataBase)saveData);
        }

    }

}
