using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class NodeBase : Node
{

    public string guid => viewDataKey;

    public string groupGuid;

    protected EventGraphView graphView;

    #region Constructors

    public NodeBase(NodeBase copy)
    {
        this.graphView = copy.graphView;
    }

    public NodeBase(Vector2 pos, EventGraphView graphView)
    {
        this.graphView = graphView;
        SetPosition(new Rect(pos, Vector2.zero));
    }

    public NodeBase(EventGraphView graphView, NodeDataBase nodeData)
    {
        this.graphView = graphView;
        this.viewDataKey = nodeData.guid;
        this.groupGuid = nodeData.groupGuid;
        SetPosition(new Rect(nodeData.position, Vector2.zero));
    }

    #endregion

    #region Abstract

    /// <summary>
    /// Must be called in constructors of derived classes after setting all the data.
    /// </summary>
    protected abstract void DrawNode();

    /// <summary>
    /// Serialize this class into a JSON string for saving.
    /// </summary>
    /// <returns></returns>
    public abstract string Serialize();

    #endregion

    #region Overrides

    public override Port InstantiatePort(Orientation orientation, Direction direction, 
        Port.Capacity capacity, Type type)
    {
        EdgeConnectorListenerBase listener = new EdgeConnectorListenerBase();
        PortBase port = new PortBase(orientation, direction, capacity, type);
        port.SetEdgeConnector(new EdgeConnector<Edge>(listener));
        port.AddManipulator(port.edgeConnector);

        return port;
    }

    #endregion

    public void ConnectEdge(EdgeData conn)
    {
        List<VisualElement> elements = new List<VisualElement>(this.outputContainer.Children());
        if (elements[conn.portIndex] is Port port)
        {
            NodeBase nextNode = graphView.GetElementByGuid(conn.toNodeGuid) as NodeBase;
            if (nextNode == null)
            {
                Debug.LogError("NextNode was null");
                return;
            }

            Port nextInputPort;

            // get next port based on edge type
            // i.e. "var" edge type will connect a VarNode output to the mainContainer
            if (conn.edgeType == "var")
                nextInputPort = nextNode.GetVarInputPort();
            else
                nextInputPort = nextNode.GetInputPort();

            if (nextInputPort == null) return;

            Edge edge = port.ConnectTo(nextInputPort);
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

    public List<Port> GetOutputPorts()
    {
        List<Port> outputPorts = new List<Port>();
        foreach (var item in this.outputContainer.Children())
            if (item is Port)
                outputPorts.Add(item as Port);
        return outputPorts;
    }

    public Port GetOutputPort(int index)
    {
        int count = 0;
        foreach (var item in this.outputContainer.Children())
        {
            if (!(item is Port port)) continue;

            if (index == count)
                return port;
            else
                count++;
        }

        Debug.LogError("Couldnt find output port with index: " + index);
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
            //Debug.LogError("Couldn't find input port for node");
            return null;
        }
    }

    public Port GetVarInputPort()
    {
        foreach (var item in mainContainer.Children())
        {
            if (item is Port port)
                return port;
        }

        return null;
    }

}
