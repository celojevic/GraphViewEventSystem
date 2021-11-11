using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class NodeBase : Node
{

    public string guid => viewDataKey;

    public string groupGuid;

    protected EventGraphView graphView;
    protected Vector2 position;

    #region Constructors

    public NodeBase(Vector2 pos, EventGraphView graphView)
    {
        this.position = pos;
        this.graphView = graphView;

        SetPosition(new Rect(pos, Vector2.zero));

    }

    public NodeBase(EventGraphView graphView, NodeDataBase nodeData)
    {
        this.position = nodeData.position;
        this.graphView = graphView;
        this.viewDataKey = nodeData.guid;
        this.groupGuid = nodeData.groupGuid;

        SetPosition(new Rect(nodeData.position, Vector2.zero));
    }

    #endregion

    #region Abstract

    protected abstract void DrawNode();

    public abstract string Serialize();

    #endregion

    public void ConnectEdge(EdgeData conn)
    {
        List<VisualElement> elements = new List<VisualElement>(this.outputContainer.Children());
        if (elements[conn.choiceIndex] is Port port)
        {
            NodeBase nextNode = graphView.GetElementByGuid(conn.toNodeGuid) as NodeBase;
            if (nextNode == null)
            {
                Debug.LogError("NextNode was null");
                return;
            }

            Port nextNodeInputPort = nextNode.GetInputPort();
            if (nextNodeInputPort == null)
            {
                Debug.LogError("NextNodeInputPort was null");
                return;
            }

            Edge edge = port.ConnectTo(nextNodeInputPort);
            graphView.AddElement(edge);
        }
        else
        {
            Debug.LogError("Invalid port index");
            return;
        }

        this.RefreshExpandedState();
    }

    public Port GetFirstOutputPort()
    {
        foreach (var item in this.outputContainer.Children())
            if (item is Port)
                return item as Port;

        // no ports
        Debug.LogError("No output ports found for node.");
        return null;
    }

    public Port GetInputPort()
    {
        if (inputContainer.Children().FirstElement() is Port port)
        {
            return port;
        }
        else
        {
            Debug.LogError("Couldn't find input port for node");
            return null;
        }
    }

}
