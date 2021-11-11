#if UNITY_EDITOR

using System.Collections.Generic;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.UIElements;

public class EventGraphView : GraphView
{

    private EventGraphSearchWindow _searchWindow;
    private EventGraphEditorWindow _editorWindow;

    private MiniMap _minimap;

    public EventGraphView(EventGraphEditorWindow editorWindow)
    {
        _editorWindow = editorWindow;

        CreateSearchWindow();
        CreateGridBg();
        CreateMinimap();
        this.AddStyleSheets("GridBackground.uss");
        AddManips();
        CreateEntryNode();
        SetupCallbacks();
    }

    #region Minimap

    void CreateMinimap()
    {
        _minimap = new MiniMap();
        _minimap.anchored = true;
        _minimap.SetPosition(new Rect(15, 50, 192, 108));
        Add(_minimap);
        _minimap.visible = false;
    }

    public void ToggleMinimap(bool visible)
    {
        _minimap.visible = visible;
    }

    #endregion

    void CreateEntryNode()
    {
        AddElement(new EntryNode(Vector2.zero, this));
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

        //CreateRightClickMenu();
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
                actionEvent => AddElement(new IntCompareNode(
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
        GridBackground bg = new GridBackground();
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

    internal void ClearGraph()
    {
        List<GraphElement> graphElements = base.graphElements.ToList();

        // dont remove the entry node
        graphElements.Remove(GetEntryNode());

        if (graphElements.Count > 0)
        {
            if (!EditorUtility.DisplayDialog("Graph Has Elements",
                "Are you sure you want to load? You will lose all unsaved progress.",
                "Yes", "No"))
            {
                return;
            }
        }

        DeleteElements(graphElements);
    }

    public EntryNode GetEntryNode() => (EntryNode)base.graphElements.ToList().Find(x => x is EntryNode);

}

#endif