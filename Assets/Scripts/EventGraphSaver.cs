using UnityEngine;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using Newtonsoft.Json;

public static class EventGraphSaver 
{

    // TODO add option to open file location after save
    public static void Save(EventGraphView graphView, string fileName)
    {
        if (graphView.graphElements.ToList().Count == 0) return;

        CreateFolders();

        EventGraphSaveData graphData = new EventGraphSaveData();

        graphView.graphElements.ForEach(ge =>
        {
            if (ge is NodeBase node)
            {
                // TODO how to find type of node from string when loading?
                graphData.nodeJsons.Add(node.Serialize());
            }
            else if (ge is Group group)
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

        string path = $"{Application.persistentDataPath}/EventGraphs/{fileName}.json";
        string json = File.ReadAllText(path);
        Debug.Log(json);

        EventGraphSaveData saveData = (EventGraphSaveData)JsonUtility.FromJson(json, typeof(EventGraphSaveData));
        Debug.Log(saveData.nodeJsons[0]);

        if (saveData.nodeJsons[0].Contains("nodeType"))
        {
            NodeSaveDataBase nodeData = (NodeSaveDataBase)JsonUtility.FromJson(
                saveData.nodeJsons[0], typeof(NodeSaveDataBase));
            Debug.Log(nodeData.nodeType);

            if (nodeData.nodeType == nameof(ChoiceNode))
            {
                ChoiceNodeSaveData cnData = (ChoiceNodeSaveData)JsonUtility.FromJson(
                    saveData.nodeJsons[0], typeof(ChoiceNodeSaveData));

                Debug.Log(cnData.message);
            }
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
