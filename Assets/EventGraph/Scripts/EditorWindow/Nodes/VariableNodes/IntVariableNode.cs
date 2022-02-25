using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using EventGraph.Constants;

public class IntVariableNode : VariableNodeBase<int>
{
    public override string variableTypeName => "IntVariable";

    protected override string colorHex => ColorConstants.INDIGO;

    public IntVariableNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
    {
        DrawNode();
    }
    public IntVariableNode(IntVariableNode copy) : base(copy)
    {
        DrawNode();
    }
    public IntVariableNode(EventGraphView graphView, IntVariableNodeData nodeData) : base(graphView, nodeData)
    {
        DrawNode();
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(new IntVariableNodeData(this));
    }

    protected override void DrawNode()
    {
        base.DrawNode();

        DrawTitleContainer();

        RefreshExpandedState();
    }

    void DrawTitleContainer()
    {
        ObjectField objField = EventGraphEditorUtils.CreateObjectField(typeof(IntVariable), variable);
        objField.RegisterValueChangedCallback(evt =>
        {
            this.variable = evt.newValue as IntVariable;
        });
        titleContainer.Insert(0, objField);

        titleContainer.Insert(0, EventGraphEditorUtils.CreateImage("IntVar"));

        SetNodeColor();
    }

}

public class IntVariableNodeData : VariableNodeData<int>
{

    public override string variableTypeName => "IntVariable";

    #region Constructors

    public IntVariableNodeData(IntVariableNodeData data) : base(data)
    {
    }
    
    public IntVariableNodeData(IntVariableNode node) : base(node)
    {
    }


    #endregion



}

