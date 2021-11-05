using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class NodeBase : Node
{

    public string groupGuid;

    protected EventGraphView graphView;
    protected Vector2 position;

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

    protected abstract void DrawNode();

    public abstract string Serialize();

}
