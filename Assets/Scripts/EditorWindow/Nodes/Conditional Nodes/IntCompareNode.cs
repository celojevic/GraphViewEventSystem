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

    public IntCompareNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView) { }
    
    public IntCompareNode(EventGraphView graphView, NodeDataBase nodeData) : base(graphView, nodeData)
    {
        if (!(nodeData is IntCompareNodeData))
        {
            Debug.LogError("Save data was not the same type but tried to load it as such.");
            return;
        }

        IntCompareNodeData lcn = nodeData as IntCompareNodeData;
        this.intToCompare = lcn.intToCompare;
        this.comparisonOperator = lcn.comparisonOperator;

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

    public override bool EvaluateConditions(int value)
    {
        switch (comparisonOperator)
        {
            case ComparisonOperator.EqualTo:
                return (intToCompare == value);
            case ComparisonOperator.LessThan:
                return (intToCompare < value);
            case ComparisonOperator.GreaterThan:
                return (intToCompare > value);
            case ComparisonOperator.LessThanOrEqualTo:
                return intToCompare <= value;
            case ComparisonOperator.GreaterThanOrEqualTo:
                return intToCompare >= value;
            default:
                return true;
        }
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(new IntCompareNodeData(this));
    }

}

// TODO also make this generic and have base class like conditionalNode
[System.Serializable]
public class IntCompareNodeData : NodeDataBase
{

    public int intToCompare;
    public ComparisonOperator comparisonOperator;

    public IntCompareNodeData(NodeBase node) : base(node)
    {
        if (!(node is IntCompareNode icNode))
        {
            Debug.LogError("Node is not IntCompareNode but tried to load it as such.");
            return;
        }

        this.intToCompare = icNode.intToCompare;
        this.comparisonOperator = icNode.comparisonOperator;
    }

}

public enum ComparisonOperator
{
    EqualTo,
    LessThan, GreaterThan,
    LessThanOrEqualTo, GreaterThanOrEqualTo,
}