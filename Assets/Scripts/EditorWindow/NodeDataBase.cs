using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class EventGraphElementData
{

    public string guid;
    public Vector2 position;

    #region Constructors

    public EventGraphElementData() { }

    public EventGraphElementData(GraphElement ge)
    {
        this.guid = ge.viewDataKey;
        this.position = ge.GetPosition().position;
    }

    #endregion

}

[System.Serializable]
public abstract class NodeDataBase : EventGraphElementData
{
    public string nodeType;
    public string nodeDataType;

    public string groupGuid;
    public bool isEntryNode;
    public List<EdgeData> edges = new List<EdgeData>();

    public NodeDataBase(NodeBase node) : base(node)
    {
        this.groupGuid = node.groupGuid;
        this.isEntryNode = false;

        // save the types so we can easily and generically cast them when loading
        this.nodeType = node.GetType().ToString();
        this.nodeDataType = this.GetType().ToString();

        List<VisualElement> list = new List<VisualElement>(node.outputContainer.Children());
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

                    edges.Add(new EdgeData()
                    {
                        choiceIndex = i,
                        parentNodeGuid = this.guid,
                        toNodeGuid = toNodeGuid,
                    });
                }
            }
        }
    }

    public abstract void Parse();
}

public class NodeDataWrapper : NodeDataBase
{
    public NodeDataWrapper(NodeBase node) : base(node) { }

    public override void Parse() { }
}

[System.Serializable]
public class GroupData : EventGraphElementData
{

    public List<string> nodeGuids;

    public GroupData(Group group) : base(group)
    {
        nodeGuids = new List<string>();

        foreach (GraphElement element in group.containedElements)
        {
            if (element is NodeBase node)
                nodeGuids.Add(node.viewDataKey);
        } 
    }

}

[System.Serializable]
public class EventGraphData
{
    public string entryNode;
    public List<string> nodeJsons = new List<string>();
    public List<GroupData> groups = new List<GroupData>();
}
