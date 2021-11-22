using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

public abstract class VariableNodeBase<T> : NodeBase
{

    public VariableBase<T> variable;
    /// <summary>
    /// Required to find all ScriptableObjects of the parent type.
    /// </summary>
    public abstract string variableTypeName { get; }

    #region Constructors

    public VariableNodeBase(Vector2 pos, EventGraphView graphView) : base(pos, graphView) { }

    public VariableNodeBase(VariableNodeBase<T> copy) : base(copy)
    {
        this.variable = copy.variable;
    }

    public VariableNodeBase(EventGraphView graphView, VariableNodeData<T> nodeData) 
        : base(graphView,nodeData)
    {
        string[] guids = AssetDatabase.FindAssets("t:" + variableTypeName);
        Type type = Type.GetType(variableTypeName);

        foreach (var item in guids)
        {
            var loadedVariable = Convert.ChangeType(
                AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(item), type), type);

            if ((loadedVariable as VariableBase<T>).guid == nodeData.soGuid)
            {
                variable = loadedVariable as VariableBase<T>;
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