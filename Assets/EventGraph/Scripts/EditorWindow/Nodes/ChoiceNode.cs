using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class ChoiceNode : NodeBase
{

    public string message;
    public List<string> choices;

    #region Constructors

    public ChoiceNode(ChoiceNode copy) : base(copy)
    {
        this.message = copy.message;
        this.choices = copy.choices;
        DrawNode();
    }

    public ChoiceNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
    {
        this.message = "Choice Node";

        this.choices = new List<string>();
        this.choices.Add("Next Dialogue");

        DrawNode();
    }

    public ChoiceNode(EventGraphView graphView, NodeDataBase nodeData) 
        : base(graphView, nodeData)
    {
        if (!(nodeData is ChoiceNodeData))
        {
            Debug.LogError("Save data was not the same type but tried to load it as such.");
            return;
        }

        ChoiceNodeData cnData = nodeData as ChoiceNodeData;
        this.message = cnData.message;
        this.choices = cnData.choices;

        DrawNode();
    }

    #endregion

    protected override void DrawNode()
    {
        DrawTitleContainer();
        DrawInputContainer();
        DrawOutputContainer();

        RefreshExpandedState();
    }

    void DrawTitleContainer()
    {
        this.RemoveCollapse();

        TextField messageTextField = EventGraphEditorUtils.CreateTextField(message, "",
            (evt) => 
            { 
                message = evt.newValue; 
            }
        );
        titleContainer.Insert(0, messageTextField);

        titleContainer.Insert(0, EventGraphEditorUtils.CreateImage("Dialogue"));
    }

    void DrawInputContainer()
    {
        inputContainer.Add(this.CreateInputPort());
    }

    void DrawOutputContainer()
    {
        // add choice button
        Button addChoiceButton = EventGraphEditorUtils.CreateButton("Add Choice", () =>
        {
            choices.Add("New Choice");
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
        Port outputPort = this.CreatePort();

        Button deleteChoiceButton = EventGraphEditorUtils.CreateButton("X", () =>
        {
            if (choices.Count == 1) return;

            if (outputPort.connected)
                graphView.DeleteElements(outputPort.connections);

            List<VisualElement> list = new List<VisualElement>(outputContainer.Children());
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == outputPort)
                {
                    // offset the index for non-choice elements in the outputContainer
                    choices.RemoveAt(i - (list.Count - choices.Count));
                    break;
                }
            }

            graphView.RemoveElement(outputPort);

        });
        outputPort.Add(deleteChoiceButton);

        TextField choiceTextField = EventGraphEditorUtils.CreateTextField(choiceText);
        outputPort.Add(choiceTextField);

        outputContainer.Add(outputPort);        
    }

    public override string Serialize()
    {
        return JsonUtility.ToJson(new ChoiceNodeData(this));
    }

}

[System.Serializable]
public class ChoiceNodeData : NodeDataBase
{

    public string message;
    public List<string> choices = new List<string>();

    public ChoiceNodeData(NodeDataBase data) : base(data)
    {
        ChoiceNodeData oldData = data as ChoiceNodeData;
        this.message = oldData.message;
        this.choices = oldData.choices;
    }

    public ChoiceNodeData(NodeBase node) : base(node)
    {
        ChoiceNode cn = node as ChoiceNode;
        message = cn.message;

        List<VisualElement> nodeOutputElements = new List<VisualElement>(node.outputContainer.Children());
        for (int i = 0; i < nodeOutputElements.Count; i++)
        {
            VisualElement item = nodeOutputElements[i];

            if (item is Port port)
            {
                // save ports textField text
                foreach (VisualElement portElement in port.Children())
                    if (portElement is TextField tf)
                        choices.Add(tf.text);
            }
        }
    }

    public override void Parse(EventGraphParser parser)
    {
        List<ChoiceAction> choiceActions = new List<ChoiceAction>();
        for (int i = 0; i < this.choices.Count; i++)
        {
            var index = i;
            choiceActions.Add(new ChoiceAction
            {
                choice = choices[i],
                callback = () =>
                {
                    if (edges.Count <= index)
                    {
                        // TODO end dialogue if nothing next and button pressed
                        Debug.LogWarning("ChoiceNode with edge at index doesn't go anywhere: " + index);
                        return;
                    }

                    parser.curNodeGuid = edges[index].toNodeGuid;
                    parser.Next();
                }
            });
        }

        UIDialogue.instance.ShowMessage(message, choiceActions);
    }

}

[System.Serializable]
public struct EdgeData
{
    /// <summary>
    /// Index of the edge in the parent node's output container.
    /// </summary>
    public int portIndex;
    /// <summary>
    /// Guid of the parent node the edge originates from.
    /// </summary>
    public string parentNodeGuid;
    /// <summary>
    /// Guid of the node the edge connects to.
    /// </summary>
    public string toNodeGuid;
    /// <summary>
    /// Type of edge connection. Can be "" or "var".
    /// </summary>
    public string edgeType;
}