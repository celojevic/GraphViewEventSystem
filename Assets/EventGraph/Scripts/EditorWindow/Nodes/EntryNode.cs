
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class EntryNode : NodeBase
{
    public EntryNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
    {
        SetPosition(new Rect(new Vector2(100, 200), Vector2.zero));
        DrawNode();
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(new EntryNodeData(this));
    }

    protected override void DrawNode()
    {
        title = "Start";

        Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, default);
        port.portName = "Output";
        outputContainer.Add(port);

        capabilities = 0;

        RefreshExpandedState();
    }
}

[System.Serializable]
public class EntryNodeData : NodeDataBase
{

    public EntryNodeData(NodeBase node) : base(node)
    {
    }

    public override void Parse()
    {
        throw new System.NotImplementedException();
    }
}
