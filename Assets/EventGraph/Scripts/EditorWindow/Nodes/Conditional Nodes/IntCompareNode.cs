using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class IntCompareNode : ConditionalNode<int>
{

    public int intToCompare;
    public ComparisonOperator comparisonOperator;

    #region Constructors

    public IntCompareNode(IntCompareNode copy) : base(copy)
    {
        this.intToCompare = copy.intToCompare;
        this.comparisonOperator = copy.comparisonOperator;

        DrawNode();
    }

    public IntCompareNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView) { }
    
    public IntCompareNode(EventGraphView graphView, IntCompareNodeData data) 
        : base(graphView, data)
    {
        this.intToCompare = data.intToCompare;
        this.comparisonOperator = data.comparisonOperator;

        DrawNode();
    }

    #endregion

    protected override void DrawNode()
    {
        base.DrawNode();
        DrawTitleContainer();
        DrawMainContainer();
    }

    void DrawTitleContainer()
    {
        titleContainer.Add(new Label("Int Comparison"));

        titleContainer.Insert(0, EventGraphEditorUtils.CreateImage("IntCompare"));

        RefreshExpandedState();
    }

    void DrawMainContainer()
    {
        EnumField comparisonOperatorField = new EnumField(ComparisonOperator.EqualTo);
        comparisonOperatorField.value = comparisonOperator;
        comparisonOperatorField.RegisterValueChangedCallback(evt =>
        {
            comparisonOperator = (ComparisonOperator)evt.newValue;
        });
        mainContainer.Add(comparisonOperatorField);

        IntegerField intField = new IntegerField();
        intField.value = intToCompare;
        intField.RegisterValueChangedCallback(evt =>
        {
            this.intToCompare = evt.newValue;
        });
        mainContainer.Add(intField);
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(new IntCompareNodeData(this));
    }

}

[System.Serializable]
public class IntCompareNodeData : ConditionalNodeData<int>
{

    public int intToCompare;
    public ComparisonOperator comparisonOperator;

    public IntCompareNodeData(IntCompareNodeData data) : base(data)
    {
        this.intToCompare = data.intToCompare;
        this.comparisonOperator = data.comparisonOperator;
    }

    public IntCompareNodeData(IntCompareNode node) : base(node)
    {
        this.intToCompare = node.intToCompare;
        this.comparisonOperator = node.comparisonOperator;
    }

    #region Runtime

    public override bool EvaluateCondition(int value)
    {
        switch (comparisonOperator)
        {
            case ComparisonOperator.EqualTo:
                return value == intToCompare;
            case ComparisonOperator.LessThan:
                return value < intToCompare;
            case ComparisonOperator.GreaterThan:
                return value > intToCompare;
            case ComparisonOperator.LessThanOrEqualTo:
                return value <= intToCompare;
            case ComparisonOperator.GreaterThanOrEqualTo:
                return value >= intToCompare;
            default:
                return true;
        }
    }

    public override void Parse(EventGraphParser parser)
    {
        throw new System.NotImplementedException();
    }

    #endregion

}

public enum ComparisonOperator
{
    EqualTo,
    LessThan, GreaterThan,
    LessThanOrEqualTo, GreaterThanOrEqualTo,
}
