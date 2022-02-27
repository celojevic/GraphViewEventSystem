using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor.Experimental.GraphView;

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

#endif
