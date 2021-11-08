using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class WaitNode : NodeBase
{

    public float timeToWait;

    public WaitNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
    {
        DrawNode();
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(new WaitNodeSaveData(this));
    }

    protected override void DrawNode()
    {
        titleContainer.Add(new Label("Time to Wait"));

        inputContainer.Add(this.CreateInputPort());
        outputContainer.Add(this.CreatePort("Output"));

        FloatField timeField = new FloatField();
        mainContainer.Add(timeField);
    }

}

public class WaitNodeSaveData : NodeSaveDataBase
{

    public WaitNodeSaveData(NodeBase node) : base(node)
    {

    }


}