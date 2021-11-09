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

    public WaitNode(EventGraphView graphView, NodeSaveDataBase saveData) : base(graphView, saveData)
    {
        if (saveData is WaitNodeSaveData wnData)
        {
            this.timeToWait = wnData.timeToWait;
        }

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
        timeField.value = this.timeToWait;
        timeField.RegisterValueChangedCallback(evt =>
        {
            this.timeToWait = evt.newValue;
        });
        mainContainer.Add(timeField);
    }

}

public class WaitNodeSaveData : NodeSaveDataBase
{

    public float timeToWait;

    public WaitNodeSaveData(NodeBase node) : base(node)
    {
        WaitNode waitNode = node as WaitNode;
        this.timeToWait = waitNode.timeToWait;
    }

}