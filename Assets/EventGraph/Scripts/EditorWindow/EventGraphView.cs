#if UNITY_EDITOR

using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.UIElements;


// TODO duplicate nodes in graph
// TODO drop edge on graph to open search window
// TODO draw mouse coordinates on cursor
// TODO look into Placemat and StickyNote classes

public class EventGraphClipboard : ScriptableObject
{
    public List<GraphElement> graphElements;
}

public class EventGraphView : GraphView
{

    private EventGraphSearchWindow _searchWindow;
    private EventGraphEditorWindow _editorWindow;
    private EventGraphClipboard _clipboard;
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
        SetupClipboard();

    }

    #region Clipboard, Copy/Paste Functionality

    private Vector2 _localMousePos;

    void SetupClipboard()
    {
        serializeGraphElements += Copy;
        unserializeAndPaste += Paste;
        canPasteSerializedData += CanPaste;
        deleteSelection += Delete;

        RegisterCallback<MouseMoveEvent>(evt =>
        {
            _localMousePos = evt.localMousePosition;
        });

        _clipboard = ScriptableObject.CreateInstance<EventGraphClipboard>();
    }

    private void Delete(string operationName, AskUser askUser)
    {
        // TODO undo/redo functionality
    }

    /// <summary>
    /// Simply allows paste functionality.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private bool CanPaste(string data)
    {
        return true;
    }

    private void Paste(string operationName, string data)
    {
        if (_clipboard.graphElements.HasElements())
        {
            // cache new ports to be connected
            List<EdgeData> newEdges = new List<EdgeData>();

            // select the newly created node(s)
            List<ISelectable> newSelection = new List<ISelectable>();
            selection.Clear();

            // need an origin point to relatively place copied elements
            Vector2 posOrigin = default;

            foreach (GraphElement graphElement in _clipboard.graphElements)
            {
                Vector2 mousePos = GetLocalMousePos(_localMousePos);

                if (graphElement is NodeBase node)
                {
                    // use first node that isn't the entry node as origin point
                    if (posOrigin == default)
                        posOrigin = node.GetPosition().position;

                    // create new node
                    Vector2 posOffset = node.GetPosition().position - posOrigin;
                    NodeBase newNode = (NodeBase)Activator.CreateInstance(node.GetType(), node);
                    newNode.SetPosition(new Rect(mousePos + posOffset, Vector2.zero));
                    AddElement(newNode);

                    // mark new node as selected
                    newNode.selected = true;
                    newSelection.Add(newNode);


                }
                else if (graphElement is Edge edge)
                {
                    // connect existing edge?
                }

            }

            // auto-select the newly copied elements
            selection = newSelection;
        }
    }

    private string Copy(IEnumerable<GraphElement> elements)
    {
        _clipboard.graphElements = null;
        _clipboard.graphElements = new List<GraphElement>(elements);

        // loop thru elements and create/cache new instances of them here instead of on paste

        return null;
    }

    #endregion

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