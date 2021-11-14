using UnityEngine;
using UnityEditor.UIElements;
using UnityEditor;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif

[System.Serializable]
public class EventGraphElementData
{

    public string guid;
    public Vector2 position;

    public EventGraphElementData(EventGraphElementData data)
    {
        this.guid = data.guid;
        this.position = data.position;
    }

    public EventGraphElementData(GraphElement graphElement)
    {
        this.guid = graphElement.viewDataKey;
        this.position = graphElement.GetPosition().position;
    }

}

public class VariableNodeBase<T> : NodeBase
{

    public VariableBase<T> variable;

    public VariableNodeBase(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
    {
    }

    public override string Serialize()
    {
        throw new System.NotImplementedException();
    }

    protected override void DrawNode()
    {
    }

}

public class IntVariableNode : VariableNodeBase<int>
{

    public IntVariableNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
    {
        this.graphView = graphView;
        DrawNode();
    }

    public override string Serialize()
    {
        throw new System.NotImplementedException();
    }

    protected override void DrawNode()
    {
        ObjectField objField = EventGraphEditorUtils.CreateObjectField(typeof(IntVariable));
        titleContainer.Add(objField);

        titleContainer.Add(this.CreatePort());

        // TODO remove the collapse symbol thing to make look nice
        this.expanded = false;
        this.capabilities &= ~Capabilities.Collapsible;

        RefreshExpandedState();
    }
}