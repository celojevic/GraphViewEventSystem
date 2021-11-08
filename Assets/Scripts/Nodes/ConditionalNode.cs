using UnityEngine;

[System.Serializable]
public abstract class ConditionalNode<T> : NodeBase
{

    public ConditionalNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
    {
        DrawNode();
    }
    public ConditionalNode(EventGraphView graphView, NodeSaveDataBase saveData) : base(graphView, saveData)
    { 
    }

    public abstract bool EvaluateConditions(T value);

    protected override void DrawNode()
    {
        DrawOutputContainer();
    }

    void DrawOutputContainer()
    {
        outputContainer.Add(this.CreatePort("True"));
        outputContainer.Add(this.CreatePort("False"));
    }

}
