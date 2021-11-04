using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EventGraphSearchWindow : ScriptableObject, ISearchWindowProvider
{

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
        List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
        {
            new SearchTreeGroupEntry(new GUIContent("Open Search Window")),

            new SearchTreeGroupEntry(new GUIContent("Nodes"), 1),
            new SearchTreeEntry(new GUIContent("Choice Node", _indentIcon))
            {
                level = 2,
                userData = "ChoiceNode"
            },
            new SearchTreeEntry(new GUIContent("Level Compare Node", _indentIcon))
            {
                level = 2,
                userData = "LevelCompareNode"
            },

            new SearchTreeGroupEntry(new GUIContent("Groups"), 1),
            new SearchTreeEntry(new GUIContent("Group", _indentIcon))
            {
                level = 2,
                userData = new Group()
            }
        };

        return searchTreeEntries;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        Vector2 localMousePos = _graphView.GetLocalMousePos(context.screenMousePosition, true);

        switch (SearchTreeEntry.userData)
        {
            case "ChoiceNode":
                _graphView.AddElement(new ChoiceNode(localMousePos, _graphView));
                return true;

            case "LevelCompareNode":
                _graphView.AddElement(new LevelCompareNode(localMousePos, _graphView));
                return true;

            case Group _:
                _graphView.CreateGroup(localMousePos);
                return true;

            default:
                return false;
        }
    }

}
