using System.Collections;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class WaitNode : NodeBase
{

    public float timeToWait;

    #region Constructors

    public WaitNode(WaitNode copy) : base(copy)
    {
        this.timeToWait = copy.timeToWait;
        DrawNode();
    }

    public WaitNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
    {
        DrawNode();
    }

    public WaitNode(EventGraphView graphView, NodeDataBase nodeData) : base(graphView, nodeData)
    {
        if (nodeData is WaitNodeData wnData)
        {
            this.timeToWait = wnData.timeToWait;
        }

        DrawNode();
    }

    #endregion

    public override string Serialize()
    {
        return JsonUtility.ToJson(new WaitNodeData(this));
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

public class WaitNodeData : NodeDataBase
{

    public float timeToWait;

    #region Constructors

    public WaitNodeData(WaitNodeData copy) : base(copy)
    {
        this.timeToWait = copy.timeToWait;
    }

    public WaitNodeData(NodeBase node) : base(node)
    {
        WaitNode waitNode = node as WaitNode;
        this.timeToWait = waitNode.timeToWait;
    }

    #endregion

    public override void Parse(EventGraphParser parser)
    {
        parser.StartCoroutine(WaitNodeCo(parser));
    }

    IEnumerator WaitNodeCo(EventGraphParser parser)
    {
        yield return new WaitForSeconds(this.timeToWait);

        parser.curNodeGuid = GetFirstNextNodeGuid();
        parser.Next();
    }

}