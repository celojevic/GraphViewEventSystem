using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public abstract class ConditionalNode : NodeBase
{

    public ConditionalNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
    {
        DrawNode();
    }
    public ConditionalNode(EventGraphView graphView, NodeSaveDataBase saveData) : base(graphView, saveData)
    { 
    }

    public abstract bool EvaluateConditions();

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

[System.Serializable]
public class LevelCompareNodeSaveData : NodeSaveDataBase
{



    public LevelCompareNodeSaveData(NodeBase node) : base(node)
    {

    }

}

[System.Serializable]
public class LevelCompareNode : ConditionalNode
{


    public LevelCompareNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView) { }
    public LevelCompareNode(EventGraphView graphView, NodeSaveDataBase saveData)
        : base(graphView, saveData)
    {
        if (!(saveData is LevelCompareNodeSaveData))
        {
            Debug.LogError("Save data was not the same type but tried to load it as such.");
            return;
        }

        LevelCompareNodeSaveData cnData = saveData as LevelCompareNodeSaveData;


        DrawNode();
    }

    protected override void DrawNode()
    {
        base.DrawNode();

        titleContainer.Add(new Label("Level Comparison"));

        inputContainer.Add(
            EventGraphEditorUtils.CreatePort(this, "Input", Direction.Input, Port.Capacity.Multi));

        mainContainer.Add(new EnumField(ComparisonOperator.EqualTo));
    }

    public override bool EvaluateConditions()
    {
        return true;
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(new LevelCompareNodeSaveData(this));
    }

}

public enum ComparisonOperator
{
    EqualTo,
    LessThan, GreaterThan,
    LessThanOrEqualTo,
    GreaterThanOrEqualTo,
}