#if UNITY_EDITOR

using UnityEngine;
using UnityEditor.Experimental.GraphView;

[System.Serializable]
public abstract class ConditionalNode<T> : NodeBase
{

    public string valueNodeGuid;

    #region Constructors

    public ConditionalNode(ConditionalNode<T> copy) : base(copy)
    {
        this.valueNodeGuid = copy.valueNodeGuid;
    }

    public ConditionalNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
    {
        DrawNode();
    }

    public ConditionalNode(EventGraphView graphView, ConditionalNodeData<T> data) 
        : base(graphView, data)
    {
        this.valueNodeGuid = data.valueNodeGuid;
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

    public ConditionalNodeData(ConditionalNodeData<T> data) : base(data)
    {
        this.valueNodeGuid = data.valueNodeGuid;
    }

    public ConditionalNodeData(ConditionalNode<T> node) : base(node)
    {
        this.valueNodeGuid = node.valueNodeGuid;
    }

    public abstract bool EvaluateCondition(T value);

}
