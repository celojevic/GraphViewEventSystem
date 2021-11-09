using UnityEngine;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System;

public static class EventGraphSaver 
{

    // TODO add option to open file location after save
    public static void Save(EventGraphView graphView, string fileName)
    {
        if (graphView.graphElements.ToList().Count <= 1) return;

        CreateFolders();

        string path = $"{Application.persistentDataPath}/EventGraphs/{fileName}.json";
        if (File.Exists(path))
        {
            if (!EditorUtility.DisplayDialog("File Already Exists",
                "File already exists with name. Overwrite?",
                "Yes", "No"))
            {
                return;
            }
        }

        EventGraphSaveData graphData = new EventGraphSaveData();

        graphView.graphElements.ForEach(graphElement =>
        {
            if (graphElement is NodeBase node)
            {
                if (node is EntryNode)
                {
                    graphData.entryNode = node.Serialize();
                }
                else
                {
                    graphData.nodeJsons.Add(node.Serialize());
                }
            }
            else if (graphElement is Group group)
            {
                graphData.groups.Add(new GroupSaveData(group));
            }
        });

        string json = JsonUtility.ToJson(graphData, true);
        File.WriteAllText(path, json);

        Debug.Log("Save successful! Path: " + path);
    }

    public static void Load(EventGraphView graphView, string fileName)
    {
        if (!Directory.Exists($"{Application.persistentDataPath}/EventGraphs"))
        {
            Debug.LogError("Load directory doesn't exist. Try saving something first");
            return;
        }

        graphView.ClearGraph();

        string path = $"{Application.persistentDataPath}/EventGraphs/{fileName}.json";
        string json = File.ReadAllText(path);



        // TODO load groups first



        EventGraphSaveData graphData = (EventGraphSaveData)JsonUtility.FromJson(json, typeof(EventGraphSaveData));
        List<ConnectionSaveData> savedConnections = new List<ConnectionSaveData>();

        for (int i = 0; i < graphData.nodeJsons.Count; i++)
        {
            // is it a node?
            if (!graphData.nodeJsons[i].Contains("nodeType")) continue;

            // load as base type to get nodeType info
            NodeSaveDataBase nodeData = (NodeSaveDataBase)JsonUtility.FromJson(
                graphData.nodeJsons[i], typeof(NodeSaveDataBase));

            // get save type and load data as parent
            Type saveDataType = Type.GetType(nodeData.nodeSaveDataType);
            var saveData = JsonUtility.FromJson(graphData.nodeJsons[i], saveDataType);

            // create node from loaded save data
            Type nodeType = Type.GetType(nodeData.nodeType);
            var node = Activator.CreateInstance(nodeType, graphView, saveData);

            // add node to graph
            graphView.AddElement((GraphElement)node);

            // cache edges to add
            if ((saveData as NodeSaveDataBase).connections.HasElements())
                savedConnections.AddRange((saveData as NodeSaveDataBase).connections);
        }

        // reconnect nodes
        foreach (ConnectionSaveData conn in savedConnections)
        {
            NodeBase node = graphView.GetElementByGuid(conn.parentNodeGuid) as NodeBase;
            if (node != null)
                node.ConnectEdge(conn);
            else
                Debug.LogError("Couldn't get node from guid: " + conn.parentNodeGuid);
        }

        // connect entry node
        EntryNodeSaveData entryData = (EntryNodeSaveData)JsonUtility.FromJson(graphData.entryNode, typeof(EntryNodeSaveData));
        EntryNode entryNode = graphView.GetEntryNode();
        NodeBase toNode = graphView.GetNodeByGuid(entryData.connections[0].toNodeGuid) as NodeBase;
        Edge edge = entryNode.GetFirstOutputPort().ConnectTo(toNode.GetInputPort());
        graphView.AddElement(edge);

    }

    static void CreateFolders()
    {
        CreateFolder($"{Application.persistentDataPath}/EventGraphs");
    }

    static void CreateFolder(string path)
    {
        if (Directory.Exists(path)) return;

        Directory.CreateDirectory(path);
    }


    public static EventGraphSaveData LoadGraphDataJson(string fileName)
    {
        return (EventGraphSaveData)JsonUtility.FromJson(
            File.ReadAllText($"{Application.persistentDataPath}/EventGraphs/{fileName}.json"), 
            typeof(EventGraphSaveData)
        );
    }


}
