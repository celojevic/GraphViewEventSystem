#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EventGraphClipboard : ScriptableObject
{
    public List<GraphElement> graphElements;
}

#endif