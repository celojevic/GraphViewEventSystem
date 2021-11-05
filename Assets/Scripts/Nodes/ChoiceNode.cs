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
    public ChoiceNode(EventGraphView graphView, NodeSaveDataBase saveData) 
        : base(graphView, saveData)
    {
        if (!(saveData is ChoiceNodeSaveData))
        {
            Debug.LogError("Save data was not the same type but tried to load it as such.");
            return;
        }

        ChoiceNodeSaveData cnData = saveData as ChoiceNodeSaveData;
        this.message = cnData.message;
        this.choices = cnData.choices;

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
        TextField messageTextField = EventGraphEditorUtils.CreateTextField(message, "",
            (evt) => 
            { 
                message = evt.newValue; 
            }
        );
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
        Port outputPort = EventGraphEditorUtils.CreatePort(this);

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
        return JsonUtility.ToJson(new ChoiceNodeSaveData(this));
    }

    // can move this to base class without abstract?
    public override void ConnectEdge(ConnectionSaveData conn)
    {
        List<VisualElement> elements = new List<VisualElement>(this.outputContainer.Children());
        if (elements[conn.choiceIndex] is Port port)
        {
            NodeBase nextNode = graphView.GetElementByGuid(conn.toNodeGuid) as NodeBase;
            Port nextNodeInputPort = nextNode.inputContainer.Children().FirstElement() as Port;
            Edge edge = port.ConnectTo(nextNodeInputPort);
            graphView.AddElement(edge);
        }
        else
        {
            Debug.LogError("Invalid port index");
            return;
        }

        this.RefreshExpandedState();
    }
}

[System.Serializable]
public class ChoiceNodeSaveData : NodeSaveDataBase
{

    public string message;
    public List<string> choices = new List<string>();
    public List<ConnectionSaveData> connections = new List<ConnectionSaveData>();

    public ChoiceNodeSaveData(NodeBase node) : base(node)
    {
        ChoiceNode cn = node as ChoiceNode;
        message = cn.message;

        var list = new List<VisualElement>(node.outputContainer.Children());
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];

            if (item is Port port)
            {
                // save the ports connection
                if (port.connected)
                {
                    string toNodeGuid = "";
                    foreach (Edge edge in port.connections)
                        toNodeGuid = edge.input.node.viewDataKey;
                    if (string.IsNullOrEmpty(toNodeGuid))
                    {
                        Debug.LogError("ToNodeGuid was null");
                        return;
                    }

                    connections.Add(new ConnectionSaveData()
                    {
                        choiceIndex = i,
                        parentNodeGuid = this.guid,
                        toNodeGuid = toNodeGuid,
                    });
                }

                // save ports textField text
                foreach (var thing in port.Children())
                    if (thing is TextField tf)
                        choices.Add(tf.text);

            }
        }

        //foreach (var item in node.outputContainer.Children())
        //{
        //    if (item is Port port)
        //    {
        //        // save the ports connection
        //        if (port.connected)
        //        {
        //            string toNodeGuid = "";
        //            foreach (Edge edge in port.connections)
        //                toNodeGuid = edge.input.node.viewDataKey;
        //            if (string.IsNullOrEmpty(toNodeGuid))
        //            {
        //                Debug.LogError("ToNodeGuid was null");
        //                return;
        //            }

        //            connections.Add(new ConnectionSaveData()
        //            {

        //                parentNodeGuid = this.guid,
        //                toNodeGuid = toNodeGuid,
        //            });
        //        }

        //        // save ports textField text
        //        foreach (var thing in port.Children())
        //            if (thing is TextField tf)
        //                choices.Add(tf.text);

        //    }
        //}

    }

}

[System.Serializable]
public struct ConnectionSaveData
{
    public int choiceIndex;
    public string parentNodeGuid;
    public string toNodeGuid;
}