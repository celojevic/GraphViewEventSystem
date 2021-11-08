using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class NodeBase : Node
{

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

    public NodeBase(EventGraphView graphView, NodeSaveDataBase saveData)
    {
        this.position = saveData.position;
        this.graphView = graphView;
        this.viewDataKey = saveData.guid;
        this.groupGuid = saveData.groupGuid;

        SetPosition(new Rect(saveData.position, Vector2.zero));
    }

    #endregion

    #region Abstract

    protected abstract void DrawNode();

    public abstract string Serialize();

    #endregion

    public void ConnectEdge(ConnectionSaveData conn)
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

            Port nextNodeInputPort = nextNode.inputContainer.Children().FirstElement() as Port;
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

}