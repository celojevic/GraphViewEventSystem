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
            else if (graphElement is Group group)
            {
                graphData.groups.Add(new GroupData(group));
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



        EventGraphData graphData = (EventGraphData)JsonUtility.FromJson(json, typeof(EventGraphData));
        List<EdgeData> savedConnections = new List<EdgeData>();

        for (int i = 0; i < graphData.nodeJsons.Count; i++)
        {
            // is it a node?
            if (!graphData.nodeJsons[i].Contains("nodeType")) continue;

            // load as base type to get nodeType info
            NodeDataBase nodeData = (NodeDataBase)JsonUtility.FromJson(
                graphData.nodeJsons[i], typeof(NodeDataBase));

            // get save type and load data as parent
            Type dataType = Type.GetType(nodeData.nodeDataType);
            var data = JsonUtility.FromJson(graphData.nodeJsons[i], dataType);

            // create node from loaded save data
            Type nodeType = Type.GetType(nodeData.nodeType);
            var node = Activator.CreateInstance(nodeType, graphView, data);

            // add node to graph
            graphView.AddElement((GraphElement)node);

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

    static void CreateFolders()
    {
        CreateFolder($"{Application.persistentDataPath}/EventGraphs");
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