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

        //string json = JsonConvert.SerializeObject(graphData);
        string json = JsonUtility.ToJson(graphData, true);

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

        EventGraphSaveData saveData = (EventGraphSaveData)JsonUtility.FromJson(json, typeof(EventGraphSaveData));
        List<ConnectionSaveData> savedConnections = new List<ConnectionSaveData>();

        for (int i = 0; i < saveData.nodeJsons.Count; i++)
        {
            if (saveData.nodeJsons[i].Contains("nodeType"))
            {
                NodeSaveDataBase nodeData = (NodeSaveDataBase)JsonUtility.FromJson(
                    saveData.nodeJsons[i], typeof(NodeSaveDataBase));

                if (nodeData.nodeType == nameof(ChoiceNode))
                {
                    ChoiceNodeSaveData cnData = (ChoiceNodeSaveData)JsonUtility.FromJson(
                        saveData.nodeJsons[i], typeof(ChoiceNodeSaveData));
                    graphView.CreateNode(cnData);

                    // cache the conns to set edges after all nodes are created
                    if (cnData.connections.HasElements())
                    {
                        savedConnections.AddRange(cnData.connections);
                    }
                }
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
