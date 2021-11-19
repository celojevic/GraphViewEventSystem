using UnityEditor.UIElements;
using UnityEngine;

public class VariableNodeBase<T> : NodeBase
{

    public VariableBase<T> variable;

    #region Constructors

    public VariableNodeBase(Vector2 pos, EventGraphView graphView) : base(pos, graphView) { }

    public VariableNodeBase(VariableNodeBase<T> copy) : base(copy)
    {
        this.variable = copy.variable;
    }

    public VariableNodeBase(EventGraphView graphView, VariableNodeData<T> nodeData) 
        : base(graphView,nodeData)
    {

        // TODO make generic somehow
        var list = EventGraphEditorUtils.FindScriptableObjects<IntVariable>();
        foreach (var soVar in list)
        {
            if (soVar.guid == nodeData.soGuid)
            {
                variable = soVar as VariableBase<T>;
                break;
            }
        }
    }

    #endregion

    public override string Serialize()
    {
        throw new System.NotImplementedException();
    }

    protected override void DrawNode()
    {
        throw new System.NotImplementedException();
    }

}

[System.Serializable]
public class VariableNodeData<T> : NodeDataBase
{

    public string soGuid;

    public VariableNodeData(NodeDataBase data) : base(data)
    {
        this.soGuid = (data as VariableNodeData<T>)?.soGuid;
    }
    public VariableNodeData(VariableNodeBase<T> node) : base(node)
    {
        this.soGuid = node.variable.guid;
    }

    public override void Parse(EventGraphParser parser)
    {
        throw new System.NotImplementedException();
    }

}