using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class IntCompareNode : ConditionalNode<int>
{
    public int intToCompare;
    public ComparisonOperator comparisonOperator;

    public IntCompareNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView) { }
    public IntCompareNode(EventGraphView graphView, NodeSaveDataBase saveData)
        : base(graphView, saveData)
    {
        if (!(saveData is LevelCompareNodeSaveData))
        {
            Debug.LogError("Save data was not the same type but tried to load it as such.");
            return;
        }

        LevelCompareNodeSaveData lcn = saveData as LevelCompareNodeSaveData;
        this.intToCompare = lcn.intToCompare;
        this.comparisonOperator = lcn.comparisonOperator;

        DrawNode();
    }

    protected override void DrawNode()
    {
        base.DrawNode();

        titleContainer.Add(new Label("Int Comparison"));

        inputContainer.Add(this.CreateInputPort());

        DrawMainContainer();
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
        return JsonUtility.ToJson(new LevelCompareNodeSaveData(this));
    }

}

// TODO also make this generic and have base class like conditionalNode
[System.Serializable]
public class LevelCompareNodeSaveData : NodeSaveDataBase
{
    public int intToCompare;
    public ComparisonOperator comparisonOperator;

    public LevelCompareNodeSaveData(NodeBase node) : base(node)
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