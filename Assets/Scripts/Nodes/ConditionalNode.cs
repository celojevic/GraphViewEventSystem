using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ConditionalNode : NodeBase
{

    public ConditionalNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
    {
        DrawNode();
    }

    public abstract bool Compare();

    protected override void DrawNode()
    {
        DrawOutputContainer();
    }

    void DrawOutputContainer()
    {
        outputContainer.Add(EventGraphEditorUtils.CreatePort(this, "True"));
        outputContainer.Add(EventGraphEditorUtils.CreatePort(this, "False"));
    }

}

public class LevelCompareNode : ConditionalNode
{

    public override string Serialize()
    {
        return "";
    }

    public LevelCompareNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView) { }

    protected override void DrawNode()
    {
        base.DrawNode();

        titleContainer.Add(new Label("Level Comparison"));

        inputContainer.Add(
            EventGraphEditorUtils.CreatePort(this, "Input", Direction.Input, Port.Capacity.Multi));

        mainContainer.Add(new EnumField(ComparisonOperator.EqualTo));
    }

    public override bool Compare()
    {
        return true;
    }

}

public enum ComparisonOperator
{
    EqualTo,
    LessThan, GreaterThan,
    LessThanOrEqualTo,
    GreaterThanOrEqualTo,
}