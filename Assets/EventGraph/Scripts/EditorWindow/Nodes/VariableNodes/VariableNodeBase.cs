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
        if (!string.IsNullOrEmpty(nodeData.varObjName))
        {
            var thisVar = VariableBase<T>.Get(nodeData.varObjName);
            if (thisVar == null)
            {
                Debug.LogError("Thisvar was null");
                return;
            }

            variable = thisVar;
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

    public VariableNodeData(NodeDataBase data) : base(data) { }
    public VariableNodeData(NodeBase node) : base(node) { }

    public override void Parse(EventGraphParser parser)
    {
        throw new System.NotImplementedException();
    }

}