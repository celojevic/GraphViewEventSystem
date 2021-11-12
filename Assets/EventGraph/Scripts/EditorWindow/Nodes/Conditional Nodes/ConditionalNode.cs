using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public abstract class ConditionalNode<T> : NodeBase
{

    #region Constructors

    public ConditionalNode(NodeBase copy) : base(copy)
    {
    }

    public ConditionalNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
    {
        DrawNode();
    }

    public ConditionalNode(EventGraphView graphView, NodeDataBase nodeData) : base(graphView, nodeData)
    { 
    }

    #endregion

    protected override void DrawNode()
    {
        DrawInputContainer();

        mainContainer.Add(new Label(viewDataKey));
        DrawOutputContainer();
    }

    void DrawInputContainer()
    {
        inputContainer.Add(this.CreateInputPort());
    }

    void DrawOutputContainer()
    {
        outputContainer.Add(this.CreatePort("True"));
        outputContainer.Add(this.CreatePort("False"));
    }

}

[System.Serializable]
public abstract class ConditionalNodeData<T> : NodeDataBase
{

    public ConditionalNodeData(NodeDataBase data) : base(data) { }

    public ConditionalNodeData(NodeBase node) : base(node) { }

    public abstract bool EvaluateCondition(T value);

}
