using UnityEngine;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UIElements;

public static class EventGraphSaver 
{

    // TODO add option to open file location after save
    public static void Save(EventGraphView graphView, string fileName)
    {
        if (graphView.graphElements.ToList().Count == 0) return;

        CreateFolders();

        string path = $"{Application.persistentDataPath}/EventGraphs/{fileName}.json";
        if (File.Exists(path))
        {
            if (!EditorUtility.DisplayDialog("File Already Exists",
                "File already exists with name. OVerwrite?",
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
                // TODO how to find type of node from string when loading?
                graphData.nodeJsons.Add(node.Serialize());
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
        if (graphView.graphElements.ToList().Count > 0)
        {
            if (!EditorUtility.DisplayDialog("Graph Has Elements",
                "Are you sure you want to load? You will lose all unsaved progress.",
                "Yes", "No"))
            {
                return;
            }
        }

        graphView.ClearGraph();

        string path = $"{Application.persistentDataPath}/EventGraphs/{fileName}.json";
        string json = File.ReadAllText(path);

        // TODO load groups first

        EventGraphSaveData graphData = (EventGraphSaveData)JsonUtility.FromJson(json, typeof(EventGraphSaveData));
        List<ConnectionSaveData> savedConnections = new List<ConnectionSaveData>();

        for (int i = 0; i < graphData.nodeJsons.Count; i++)
        {
            if (!graphData.nodeJsons[i].Contains("nodeType")) continue;

            NodeSaveDataBase nodeData = (NodeSaveDataBase)JsonUtility.FromJson(
                graphData.nodeJsons[i], typeof(NodeSaveDataBase));

            if (nodeData.nodeType == nameof(ChoiceNode))
            {
                LoadNode<ChoiceNodeSaveData>(graphData.nodeJsons[i], graphView, savedConnections);
            }
            else if (nodeData.nodeType == nameof(LevelCompareNode))
            {
                LoadNode<LevelCompareNodeSaveData>(graphData.nodeJsons[i], graphView, savedConnections);
            }

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
    }

    static void LoadNode<T>(
        string json, EventGraphView graphView, 
        List<ConnectionSaveData> savedConnections
    ) where T : NodeSaveDataBase
    {
        T nodeSaveData = (T)JsonUtility.FromJson(json, typeof(T));
        graphView.CreateNode(nodeSaveData);

        // cache the conns to set edges after all nodes are created
        if (nodeSaveData.connections.HasElements())
            savedConnections.AddRange(nodeSaveData.connections);
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



}
