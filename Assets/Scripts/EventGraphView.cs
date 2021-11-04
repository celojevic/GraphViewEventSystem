using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class StartNode : Pill
{
}

public class EventGraphView : GraphView
{

    private EventGraphSearchWindow _searchWindow;
    private EventGraphEditorWindow _editorWindow;

    public EventGraphView(EventGraphEditorWindow editorWindow)
    {
        _editorWindow = editorWindow;

        CreateStartNode();
        CreateSearchWindow();
        CreateGridBg();
        this.AddStyleSheets("GridBackground.uss");
        AddManips();
        SetupCallbacks();
    }

    void CreateStartNode()
    {
        StartNode startNode = new StartNode();

        startNode.text = "Start";

        Add(startNode);
    }

    void SetupCallbacks()
    {
        elementsAddedToGroup = (group, elements) =>
        {
            foreach (var item in elements)
                if (item is NodeBase node)
                    node.groupGuid = group.viewDataKey;
        };

        elementsRemovedFromGroup = (group, elements) =>
        {
            foreach (var item in elements)
                if (item is NodeBase node)
                    node.groupGuid = "";
        };
    }

    void CreateSearchWindow()
    {
        if (_searchWindow == null)
        {
            _searchWindow = ScriptableObject.CreateInstance<EventGraphSearchWindow>();
            _searchWindow.Init(this);
        }

        nodeCreationRequest = context => 
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
    }

    void AddManips()
    {
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        CreateRightClickMenu();
    }

    void CreateRightClickMenu()
    {
        this.AddManipulator(new ContextualMenuManipulator(menuEvent => menuEvent.menu.AppendAction(
                "Add Group",
                actionEvent => CreateGroup(
                    GetLocalMousePos(actionEvent.eventInfo.localMousePosition))
            )));

        this.AddManipulator(new ContextualMenuManipulator(menuEvent => menuEvent.menu.AppendAction(
                "Add Choice Node",
                actionEvent => AddElement(new ChoiceNode(
                    GetLocalMousePos(actionEvent.eventInfo.localMousePosition), this)
                )
            )));

        this.AddManipulator(new ContextualMenuManipulator(menuEvent => menuEvent.menu.AppendAction(
                "Add Level Compare Node",
                actionEvent => AddElement(new LevelCompareNode(
                    GetLocalMousePos(actionEvent.eventInfo.localMousePosition), this)
                )
            )));
    }

    public void CreateGroup(Vector2 pos, string title = "Event Group")
    {
        Group group = new Group() { title = title };
        group.SetPosition(new Rect(pos, Vector2.zero));

        foreach (var item in selection)
        {
            if (!(item is NodeBase)) continue;
            group.AddElement(item as NodeBase);
        }

        AddElement(group);
    }

    void CreateGridBg()
    {
        var bg = new GridBackground();
        bg.StretchToParentSize();
        Insert(0, bg);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new List<Port>();

        ports.ForEach(port =>
        {
            if (startPort == port) return;
            if (startPort.node == port.node) return;
            if (startPort.direction == port.direction) return;

            compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    internal Vector2 GetLocalMousePos(Vector2 mousePos, bool isSearchWindow = false)
    {
        if (isSearchWindow)
            mousePos -= _editorWindow.position.position;
        return contentViewContainer.WorldToLocal(mousePos);
    }
}