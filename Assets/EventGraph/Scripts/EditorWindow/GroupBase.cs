using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GroupBase : Group
{

    public string guid => viewDataKey;
    
    private GraphView _graphView;

    public GroupBase(Vector2 position, EventGraphView graphView)
    {
        title = "Event Group";
        _graphView = graphView;
        SetPosition(new Rect(position, Vector2.zero));
    }


}
