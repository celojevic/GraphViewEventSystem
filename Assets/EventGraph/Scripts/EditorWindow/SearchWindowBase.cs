using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Reflection;
using System;

public class SearchWindowBase : ScriptableObject, ISearchWindowProvider
{

    public PortBase portDroppedFrom = null;

    private EventGraphView _graphView;
    private Texture2D _indentIcon;

    public void Init(EventGraphView graphView)
    {
        _graphView = graphView;

        _indentIcon = new Texture2D(1, 1);
        //_indentIcon.SetPixel(0, 0, Color.clear);
        _indentIcon.Apply();
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> nodeEntries = new List<SearchTreeEntry>();
        // TODO make custom asmdef and use that
        var types = Assembly.GetExecutingAssembly().GetTypes();
        for (int i = 0; i < types.Length; i++)
        {
            if (types[i] == null) continue;

            if ((types[i].BaseType == typeof(NodeBase) || types[i].BaseType?.BaseType == typeof(NodeBase))
                && !types[i].IsAbstract && !types[i].IsGenericType && types[i] != typeof(EntryNode))
            {
                nodeEntries.Add(new SearchTreeEntry(new GUIContent(types[i].ToString(), _indentIcon))
                {
                    level = 2,
                    userData = types[i]
                });
            }
        }

        List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>();
        searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent("Open Search Window")));

        // add nodes
        searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent("Nodes"), 1));
        searchTreeEntries.AddRange(nodeEntries);

        // add groups
        searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent("Groups"), 1));
        searchTreeEntries.Add(new SearchTreeEntry(new GUIContent("Group", _indentIcon))
        {
            level = 2,
            userData = typeof(Group)
        });

        return searchTreeEntries;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        Vector2 localMousePos = _graphView.GetLocalMousePos(context.screenMousePosition, true);

        // TODO doesnt work for groups, either make a type check here or make 
        //      a custom group class EventGraphGroup with proper constructor
        var node = (GraphElement)Activator.CreateInstance(
            (Type)SearchTreeEntry.userData, localMousePos, _graphView);
        _graphView.AddElement(node);

        // if no other elements, automatically connect startNosde to this new one
        if (_graphView.graphElements.ToList().Count == 2)
        {
            EntryNode entryNode = (EntryNode)_graphView.graphElements.ToList()[0];
            entryNode.ConnectEdge(new EdgeData
            {
                portIndex = 0,
                parentNodeGuid = entryNode.guid,
                toNodeGuid = node.viewDataKey
            });
        }
        else if (portDroppedFrom != null)
        {
            if (portDroppedFrom.connected)
            {
                _graphView.DeleteElements(portDroppedFrom.connections);
                portDroppedFrom.DisconnectAll();
            }

            Edge edge = portDroppedFrom.ConnectTo((node as NodeBase).GetInputPort());
            _graphView.AddElement(edge);
        }

        return true;

        //switch (SearchTreeEntry.userData)
        //{
        //    case Group _:
        //        _graphView.CreateGroup(localMousePos);
        //        return true;
        //}
    }

}
