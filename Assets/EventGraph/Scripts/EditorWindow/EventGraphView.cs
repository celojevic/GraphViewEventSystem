#if UNITY_EDITOR

using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.UIElements;


// TODO ctrl+D duplicate nodes in graph
// TODO drop edge on graph to open search window - much more complicated than it seems
// TODO draw mouse coordinates on cursor
// TODO look into Placemat and StickyNote classes, blackboard maybe?
// TODO color gradient edges. color depends on nodes connected

public class EventGraphView : GraphView
{

    public EventGraphEditorWindow editorWindow;
    public Dictionary<string, EventGraphDataObject> dictSaveData = new Dictionary<string, EventGraphDataObject>();

    public DataOperation saveFlags => editorWindow.saveTypeFlags;

    private SearchWindowBase _searchWindow;
    private ClipboardBase _clipboard;
    private MiniMap _minimap;


    public EventGraphView(EventGraphEditorWindow editorWindow)
    {
        this.editorWindow = editorWindow;

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
        //deleteSelection += Delete;

        RegisterCallback<MouseMoveEvent>(evt =>
        {
            _localMousePos = evt.localMousePosition;
        });

        _clipboard = ScriptableObject.CreateInstance<ClipboardBase>();
    }

    //private void Delete(string operationName, AskUser askUser)
    //{
    //    // TODO undo/redo functionality
    //}

    /// <summary>
    /// Simply allows paste functionality.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private bool CanPaste(string data)
    {
        return true;
    }

    private void DeselectEverything()
    {
        graphElements.ForEach(x => x.Unselect(this));
    }

    // TODO the reconnectEdge stuff is very slow with large amounts of nodes selected
    //      marked all the relevant blocks as //SLOW
    //      might just need to change the list looping to dicts/hashsets
    private void Paste(string operationName, string data)
    {
        if (_clipboard.graphElements.HasElements())
        {
            // cache new ports to be connected
            List<ReconnectEdgeData> newEdges = new List<ReconnectEdgeData>();

            // select the newly created node(s)
            List<ISelectable> newSelection = new List<ISelectable>();
            DeselectEverything();

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
                    newNode.Select(this, true);
                    newSelection.Add(newNode);

                    // SLOW
                    // cache data for new edges to be made
                    List<Port> outputPorts = node.GetOutputPorts();
                    if (outputPorts.Count > 0)
                    {
                        for (int i = 0; i < outputPorts.Count; i++)
                        {
                            // only one connection per output port
                            Edge edge = null;
                            if (outputPorts[i].connections.HasElements())
                                edge = outputPorts[i].connections?.FirstElement();

                            var reconnect = new ReconnectEdgeData
                            {
                                portIndex = i,
                                oldNextGuid = edge != null ? (edge.input.node as NodeBase).guid : "",
                                oldParentGuid = node.guid,
                                newParentGuid = newNode.guid,
                                // newNextGuid will be found later
                                newNextGuid = ""
                            };
                            newEdges.Add(reconnect);
                        }
                    }


                }
            }

            // SLOW
            // find the newNextGuids for each new edge
            for (int i = 0; i < newEdges.Count; i++)
            {
                if (!string.IsNullOrEmpty(newEdges[i].newNextGuid)) continue;

                for (int j = 0; j < newEdges.Count; j++)
                {
                    if (newEdges[i].oldParentGuid == newEdges[j].oldParentGuid) continue;

                    if (newEdges[i].oldNextGuid == newEdges[j].oldParentGuid)
                    {
                        newEdges[i].newNextGuid = newEdges[j].newParentGuid;
                        break;
                    }

                }
            }

            // SLOW
            // loop again thru newEdges and create them
            foreach (var edge in newEdges)
            {
                // no output edge
                if (string.IsNullOrEmpty(edge.newNextGuid)) continue;

                NodeBase fromNode = GetNodeByGuid(edge.newParentGuid) as NodeBase;
                if (fromNode == null)
                {
                    Debug.LogError("fromNode was null");
                    continue;
                }

                NodeBase toNode = GetNodeByGuid(edge.newNextGuid) as NodeBase;
                if (toNode == null)
                {
                    Debug.LogError("toNode was null");
                    continue;
                }

                Edge newEdge = fromNode?.GetOutputPort(edge.portIndex)?.ConnectTo(toNode?.GetInputPort());
                AddElement(newEdge);
            }

            // auto-select the newly copied elements
            selection = newSelection;
        }
    }

    private string Copy(IEnumerable<GraphElement> elements)
    {
        _clipboard.graphElements = elements != null ? new List<GraphElement>(elements) : null;

        // loop thru elements and create/cache new instances of them here instead of on paste?

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

    #region Search Window

    void CreateSearchWindow()
    {
        nodeCreationRequest = (context) => OpenSearchWindow(context.screenMousePosition);
    }

    public void OpenSearchWindow(Vector2 position, PortBase portDroppedFrom = null)
    {
        if (_searchWindow == null)
        {
            _searchWindow = ScriptableObject.CreateInstance<SearchWindowBase>();
            _searchWindow.Init(this);
        }

        _searchWindow.portDroppedFrom = portDroppedFrom;

        SearchWindow.Open(new SearchWindowContext(position), _searchWindow);
    }

    #endregion

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
                "Add Int Compare Node",
                actionEvent => AddElement(new IntCompareNode(
                    GetLocalMousePos(actionEvent.eventInfo.localMousePosition), this)
                )
            )));

        this.AddManipulator(new ContextualMenuManipulator(menuEvent => menuEvent.menu.AppendAction(
                "Add IntVar Node",
                actionEvent => AddElement(new IntVariableNode(
                    GetLocalMousePos(actionEvent.eventInfo.localMousePosition), this)
                )
            )));
    }

    #region Groups

    /// <summary>
    /// Creates a groupBase at the given position with the given title,
    /// adds the element to the graphView, and returns the groupBase.
    /// </summary>
    /// <param name="pos"></param>Position of the group.
    /// <param name="title"></param>Title to assign.
    /// <returns></returns>
    public GroupBase CreateGroup(Vector2 pos, string title = "Event Group")
    {
        GroupBase group = new GroupBase() { title = title };
        group.SetPosition(new Rect(pos, Vector2.zero));

        foreach (var item in selection)
        {
            if (!(item is NodeBase node)) continue;

            node.groupGuid = group.guid;
            group.AddElement(node);
        }

        AddElement(group);

        return group;
    }

    public GroupBase CreateGroup(GroupData data)
    {
        GroupBase group = CreateGroup(data.position, data.title);
        group.viewDataKey = data.guid;

        return group;
    }

    #endregion

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
            mousePos -= editorWindow.position.position;
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