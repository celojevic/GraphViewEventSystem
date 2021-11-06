using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class EventGraphElementSaveData
{
    public string guid;
    public Vector2 position;

    public EventGraphElementSaveData(GraphElement ge)
    {
        this.guid = ge.viewDataKey;
        this.position = ge.GetPosition().position;
    }
}

[System.Serializable]
public class NodeSaveDataBase : EventGraphElementSaveData
{
    public string nodeType;
    public string groupGuid;
    public bool isEntryNode;
    public List<ConnectionSaveData> connections = new List<ConnectionSaveData>();

    public NodeSaveDataBase(NodeBase node) : base(node)
    {
        groupGuid = node.groupGuid;
        isEntryNode = false;
        nodeType = node.GetType().ToString();

        var list = new List<VisualElement>(node.outputContainer.Children());
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] is Port port)
            {
                // save the ports connection
                if (port.connected)
                {
                    string toNodeGuid = "";
                    foreach (Edge edge in port.connections)
                        toNodeGuid = edge.input.node.viewDataKey;
                    if (string.IsNullOrEmpty(toNodeGuid))
                    {
                        Debug.LogError("ToNodeGuid was null");
                        return;
                    }

                    connections.Add(new ConnectionSaveData()
                    {
                        choiceIndex = i,
                        parentNodeGuid = this.guid,
                        toNodeGuid = toNodeGuid,
                    });
                }
            }
        }
    }
}

[System.Serializable]
public class GroupSaveData : EventGraphElementSaveData
{

    public List<string> nodeGuids;

    public GroupSaveData(Group group) : base(group)
    {
        nodeGuids = new List<string>();

        foreach (var item in group.containedElements)
        {
            if (item is NodeBase node)
                nodeGuids.Add(node.viewDataKey);
        } 
    }

}

[System.Serializable]
public class EventGraphSaveData
{
    public List<string> nodeJsons = new List<string>();
    public List<GroupSaveData> groups = new List<GroupSaveData>();
}