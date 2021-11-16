using UnityEngine;

public class IntVariableNode : VariableNodeBase<int>
{

    public IntVariableNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
    {
        DrawNode();
    }
    public IntVariableNode(IntVariableNode copy) : base(copy)
    {
        this.variable = copy.variable;
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
        this.RemoveCollapse();

        var objField = EventGraphEditorUtils.CreateObjectField(typeof(IntVariable), variable);
        titleContainer.Insert(0, objField);

        // TODO change this to titleContainer so it looks nicer.
        //      but must change how it saves the node, bc it currently looks for outputContainer
        outputContainer.Add(this.CreatePort());

        RefreshExpandedState();
    }

}

public class IntVariableNodeData : VariableNodeData<int>
{

    public string variableName;

    #region Constructors

    public IntVariableNodeData(IntVariableNodeData data) : base(data)
    {
        this.variableName = data.variableName;
    }
    
    public IntVariableNodeData(IntVariableNode node) : base(node)
    {
        this.variableName = node.name;
    }

    #endregion



}

