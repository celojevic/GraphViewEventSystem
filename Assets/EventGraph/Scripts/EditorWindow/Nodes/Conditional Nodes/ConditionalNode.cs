#if UNITY_EDITOR

using UnityEngine;
using UnityEditor.Experimental.GraphView;

[System.Serializable]
public abstract class ConditionalNode<T> : NodeBase
{

    public string valueNodeGuid { get; set; }

    #region Constructors

    public ConditionalNode(NodeBase copy) : base(copy)
    {
    }

    public ConditionalNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
    {
        DrawNode();
    }

    public ConditionalNode(EventGraphView graphView, ConditionalNodeData<T> nodeData) 
        : base(graphView, nodeData)
    { 
    }

    #endregion

    protected override void DrawNode()
    {
        DrawInputContainer();
        DrawOutputContainer();

        mainContainer.Add(this.CreatePort("Value", Direction.Input));
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

#endif

[System.Serializable]
public abstract class ConditionalNodeData<T> : NodeDataBase
{

    public string valueNodeGuid;

    public ConditionalNodeData(ConditionalNodeData<T> data) : base(data) { }

    public ConditionalNodeData(ConditionalNode<T> node) : base(node) { }

    public abstract bool EvaluateCondition(T value);

}
