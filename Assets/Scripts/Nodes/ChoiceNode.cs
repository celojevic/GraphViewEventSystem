using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class ChoiceNode : NodeBase
{

    public string message;
    public List<string> choices;

    public ChoiceNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
    {
        this.message = "Choice Node";

        choices = new List<string>();
        choices.Add("Next Dialogue");

        DrawNode();
    }

    protected override void DrawNode()
    {
        DrawTitleContainer();
        DrawInputContainer();
        DrawOutputContainer();

        RefreshExpandedState();
    }

    void DrawTitleContainer()
    {
        TextField messageTextField = EventGraphEditorUtils.CreateTextField(message);
        titleContainer.Insert(0, messageTextField);
    }

    void DrawInputContainer()
    {
        inputContainer.Add(EventGraphEditorUtils.CreatePort(this, "Input",
            Direction.Input, Port.Capacity.Multi)
        );
    }

    void DrawOutputContainer()
    {
        // add choice button
        Button addChoiceButton = EventGraphEditorUtils.CreateButton("Add Choice", () =>
        {
            //Port choicePort = EventGraphEditorUtils.CreatePort(this, "New Choice");
            choices.Add("New Choice");
            //outputContainer.Add(choicePort);
            CreateChoicePort("New Choice");
        });
        outputContainer.Add(addChoiceButton);

        // choice ports
        foreach (string choiceText in choices)
        {
            CreateChoicePort(choiceText);
        }
    }

    void CreateChoicePort(string choiceText)
    {
        Port outputPort = EventGraphEditorUtils.CreatePort(this);

        Button deleteChoiceButton = EventGraphEditorUtils.CreateButton("X", () =>
        {
            if (choices.Count == 1) return;

            if (outputPort.connected)
                graphView.DeleteElements(outputPort.connections);

            var list = new List<VisualElement>(outputContainer.Children());
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == outputPort)
                {
                    // offset the index for non-choice elements in the outputContainer
                    choices.RemoveAt(i-(list.Count - choices.Count));
                    break;
                }
            }

            graphView.RemoveElement(outputPort);

        });
        TextField choiceTextField = new TextField() { value = choiceText };

        outputPort.Add(choiceTextField);
        outputPort.Add(deleteChoiceButton);
        outputContainer.Add(outputPort);        
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(new ChoiceNodeSaveData(this));
    }

}

[System.Serializable]
public class ChoiceNodeSaveData : NodeSaveDataBase
{

    public string message;
    public List<string> choices = new List<string>();
    public List<PortSaveData> ports = new List<PortSaveData>();

    public ChoiceNodeSaveData(NodeBase node) : base(node)
    {
        ChoiceNode cn = node as ChoiceNode;
        message = cn.message;
        choices = cn.choices;

        foreach (var item in node.outputContainer.Children())
        {
            if (item is Port port)
            {
                if (!port.connected) continue;

                string toNodeGuid = "";
                foreach (Edge edge in port.connections)
                    toNodeGuid = edge.input.node.viewDataKey;
                if (string.IsNullOrEmpty(toNodeGuid))
                {
                    Debug.LogError("ToNodeGuid was null");
                    return;
                }

                ports.Add(new PortSaveData()
                {
                    parentNodeGuid = this.guid,
                    toNodeGuid = toNodeGuid,
                });
            }
        }

    }

}

[System.Serializable]
public class PortSaveData
{
    public string parentNodeGuid;
    public string toNodeGuid;
}