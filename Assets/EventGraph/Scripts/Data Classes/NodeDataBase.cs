using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public abstract class NodeDataBase : EventGraphElementData
{

    public string nodeType;
    public string nodeDataType;
    public string groupGuid;
    public bool isEntryNode;
    public List<EdgeData> edges = new List<EdgeData>();

    public NodeDataBase(NodeDataBase data) : base(data)
    {
        this.nodeType = data.nodeType;
        this.nodeDataType = data.nodeDataType;
        this.groupGuid = data.groupGuid;
        this.isEntryNode = data.isEntryNode;
        this.edges = data.edges;
    }

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

    protected string GetFirstNextNodeGuid() => edges[0].toNodeGuid;

    public abstract void Parse(EventGraphParser parser);
}

/// <summary>
/// Use this class instead of NodeDataBase to create base instances for things like Reflection.
/// </summary>
public class NodeDataWrapper : NodeDataBase
{
    public NodeDataWrapper(NodeBase node) : base(node) { }

    public override void Parse(EventGraphParser parser) { }
}
