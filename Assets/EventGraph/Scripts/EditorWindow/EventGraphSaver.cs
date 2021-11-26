using UnityEngine;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

using System;
using System.IO;
using System.Collections.Generic;

public static class EventGraphSaver 
{

    #region Saving

    // TODO add option to open file location after save
    public static void Save(EventGraphView graphView, string fileName)
    {
        if (graphView.graphElements.ToList().Count <= 1)
        {
            Debug.LogWarning("Nothing in graph to save.");
            return;
        }

        if (graphView.saveFlags == 0) // nothing
        {
            Debug.LogWarning("No save flags selected. Make sure you check at least one save flag in the toolbar.");
            return;
        }

        CreateFolders();
        
        if (graphView.saveFlags.HasFlag(DataType.JSON))
        {
            SaveAsJSON(graphView, fileName);
        }
        if (graphView.saveFlags.HasFlag(DataType.ScriptableObject))
        {
            SaveAsSO(graphView, fileName);
        }

        // reload the files in the toolbar
        graphView.editorWindow.PopulateJsonPopup();
    }

    static void SaveAsSO(EventGraphView graphView, string fileName)
    {
        string path = $"Assets/EventGraph/SaveData/{fileName}.asset";
        Debug.Log(path);

        var so = ScriptableObject.CreateInstance<EventGraphDataObject>();
        so.graphData = GetGraphData(graphView);

        AssetDatabase.CreateAsset(so, path);
        AssetDatabase.SaveAssets();

        Debug.Log("ScriptableObject - Save successful! Path: " + path);
    }

    static void SaveAsJSON(EventGraphView graphView, string fileName)
    {
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

        string json = JsonUtility.ToJson(GetGraphData(graphView), true);
        File.WriteAllText(path, json);

        Debug.Log("JSON - Save successful! Path: " + path);
    }

    static EventGraphData GetGraphData(EventGraphView graphView)
    {
        EventGraphData graphData = new EventGraphData();

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
            else if (graphElement is GroupBase group)
            {
                graphData.groups.Add(new GroupData(group));
            }
        });

        return graphData;
    }

    #endregion



    #region Loading

    public static void LoadFromSo(EventGraphView graphView, string fileName)
    {
        string path = $"Assets/EventGraph/SaveData/{fileName}.asset";
        EventGraphDataObject data = AssetDatabase.LoadAssetAtPath<EventGraphDataObject>(path);
        LoadFromData(graphView, data?.graphData);
    }

    public static void LoadFromJson(EventGraphView graphView, string fileName)
    {
        if (!Directory.Exists($"{Application.persistentDataPath}/EventGraphs"))
        {
            Debug.LogError("Load directory doesn't exist. Try saving something first");
            return;
        }

        string path = $"{Application.persistentDataPath}/EventGraphs/{fileName}.json";
        string json = File.ReadAllText(path);

        EventGraphData graphData = (EventGraphData)JsonUtility.FromJson(json, typeof(EventGraphData));
        LoadFromData(graphView, graphData);
    }

    static void LoadFromData(EventGraphView graphView, EventGraphData graphData)
    {
        if (graphData != null && graphData != null)
        {
            ReconstructGraph(graphView, graphData);
        }
        else
        {
            Debug.LogError("Loaded graphData was null.");
        }
    }

    static void ReconstructGraph(EventGraphView graphView, EventGraphData graphData)
    {
        if (graphView == null) return;
        if (graphData == null) return;

        graphView.ClearGraph();

        // load groups first
        foreach (var groupData in graphData.groups)
            graphView.CreateGroup(groupData);

        // cache connections saved in nodes we are about to create
        List<EdgeData> savedConnections = new List<EdgeData>();

        // recreate nodes
        for (int i = 0; i < graphData.nodeJsons.Count; i++)
        {
            // is it a node?
            if (!graphData.nodeJsons[i].Contains("nodeType")) continue;

            // load as base type to get nodeType info
            NodeDataWrapper nodeData = (NodeDataWrapper)JsonUtility.FromJson(
                graphData.nodeJsons[i], typeof(NodeDataWrapper));

            // get save type and load data as parent
            Type dataType = Type.GetType(nodeData.nodeDataType);
            var data = JsonUtility.FromJson(graphData.nodeJsons[i], dataType);

            // create node from loaded save data
            Type nodeType = Type.GetType(nodeData.nodeType);
            object nodeObj = Activator.CreateInstance(nodeType, graphView, data);
            NodeBase node = nodeObj as NodeBase;

            // add node to graph
            graphView.AddElement(node);

            // replace grouped nodes back into the group
            if (!string.IsNullOrEmpty(node.groupGuid))
            {
                GroupBase group = graphView.GetElementByGuid(node.groupGuid) as GroupBase;
                if (group != null)
                    group.AddElement(node);
            }

            // cache edges to add
            if ((data as NodeDataBase).edges.HasElements())
                savedConnections.AddRange((data as NodeDataBase).edges);
        }

        // reconnect nodes
        foreach (EdgeData conn in savedConnections)
        {
            NodeBase node = graphView.GetElementByGuid(conn.parentNodeGuid) as NodeBase;
            if (node != null)
                node.ConnectEdge(conn);
            else
                Debug.LogError("Couldn't get node from guid: " + conn.parentNodeGuid);
        }

        // reconnect first node to entry node
        ConnectEntryNode(graphView, graphData);
    }

    static void ConnectEntryNode(EventGraphView graphView, EventGraphData graphData)
    {
        EntryNodeData entryData = (EntryNodeData)JsonUtility.FromJson(graphData.entryNode, typeof(EntryNodeData));
        EntryNode entryNode = graphView.GetEntryNode();
        NodeBase toNode = graphView.GetNodeByGuid(entryData.edges[0].toNodeGuid) as NodeBase;
        Edge edge = entryNode.GetFirstOutputPort().ConnectTo(toNode.GetInputPort());
        graphView.AddElement(edge);
    }

    #endregion

    static void CreateFolders()
    {
        // json save path
        CreateFolder($"{Application.persistentDataPath}/EventGraphs");

        // SO save path
        if (!AssetDatabase.IsValidFolder("Assets/EventGraph"))
            AssetDatabase.CreateFolder("Assets", "EventGraph");
        if (!AssetDatabase.IsValidFolder("Assets/EventGraph/SaveData"))
            AssetDatabase.CreateFolder("Assets/EventGraph", "SaveData");
        AssetDatabase.Refresh();
    }

    static void CreateFolder(string path)
    {
        if (Directory.Exists(path)) return;

        Directory.CreateDirectory(path);
    }


    public static EventGraphData LoadGraphDataJson(string fileName)
    {
        return (EventGraphData)JsonUtility.FromJson(
            File.ReadAllText($"{Application.persistentDataPath}/EventGraphs/{fileName}.json"), 
            typeof(EventGraphData)
        );
    }


}
