using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class EventGraphElementSaveData
{

    public string guid;
    public Vector2 position;

    #region Constructors

    public EventGraphElementSaveData() { }

    public EventGraphElementSaveData(GraphElement ge)
    {
        this.guid = ge.viewDataKey;
        this.position = ge.GetPosition().position;
    }

    #endregion

}

[System.Serializable]
public class NodeSaveDataBase : EventGraphElementSaveData
{
    public string nodeType;
    public string nodeSaveDataType;

    public string groupGuid;
    public bool isEntryNode;
    public List<ConnectionSaveData> connections = new List<ConnectionSaveData>();

    public NodeSaveDataBase(NodeBase node) : base(node)
    {
        this.groupGuid = node.groupGuid;
        this.isEntryNode = false;

        // save the types so we can easily and generically cast them when loading
        this.nodeType = node.GetType().ToString();
        this.nodeSaveDataType = this.GetType().ToString();

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