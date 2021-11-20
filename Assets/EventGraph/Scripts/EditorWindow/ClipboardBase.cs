#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ClipboardBase : ScriptableObject
{
    public List<GraphElement> graphElements;
}

#endif