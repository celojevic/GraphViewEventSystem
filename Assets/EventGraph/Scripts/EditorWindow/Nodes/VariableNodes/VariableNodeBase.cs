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
        if (!string.IsNullOrEmpty(nodeData?.varObjName))
        {
            var list = EventGraphEditorUtils.FindScriptableObjects<IntVariable>();
            foreach (var item in list)
            {
                if (item.name == nodeData.varObjName)
                {
                    variable = item as VariableBase<T>;
                    break;
                }
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

    public string varObjName;

    public VariableNodeData(NodeDataBase data) : base(data)
    {
        this.varObjName = (data as VariableNodeData<T>)?.varObjName;
    }
    public VariableNodeData(NodeBase node) : base(node)
    {
        this.varObjName = (node as VariableNodeBase<T>)?.variable.name;
    }

    public override void Parse(EventGraphParser parser)
    {
        throw new System.NotImplementedException();
    }

}