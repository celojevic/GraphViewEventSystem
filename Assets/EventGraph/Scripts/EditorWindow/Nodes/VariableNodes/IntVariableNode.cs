using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

public class IntVariableNode : VariableNodeBase<int>
{
    public override string variableTypeName => "IntVariable";

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
        this.RemoveCollapse();

        ObjectField objField = EventGraphEditorUtils.CreateObjectField(typeof(IntVariable), variable);
        objField.RegisterValueChangedCallback(evt =>
        {
            this.variable = evt.newValue as IntVariable;
        });
        titleContainer.Insert(0, objField);

        // TODO change this to titleContainer so it looks nicer.
        //      but must change how it saves the node, bc it currently looks for outputContainer
        outputContainer.Add(this.CreatePort());

        RefreshExpandedState();
    }

}

public class IntVariableNodeData : VariableNodeData<int>
{


    #region Constructors

    public IntVariableNodeData(IntVariableNodeData data) : base(data)
    {
    }
    
    public IntVariableNodeData(IntVariableNode node) : base(node)
    {
    }

    #endregion



}

