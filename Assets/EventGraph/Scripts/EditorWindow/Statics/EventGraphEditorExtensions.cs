#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public static class EventGraphEditorExtensions
{

    #region Ports

    public static PortBase CreatePort(this NodeBase node, string portName = "",
        Direction direction = Direction.Output,
        Port.Capacity capacity = Port.Capacity.Single,
        Orientation orientation = Orientation.Horizontal)
    {
        PortBase port = node.InstantiatePort(
            orientation, direction, capacity, default) as PortBase;
        port.portName = portName;
        return port;
    }

    public static PortBase CreateInputPort(this NodeBase node)
    {
        return node.CreatePort("Input", Direction.Input, Port.Capacity.Multi, Orientation.Horizontal);
    }

    #endregion

    public static VisualElement AddStyleSheets(this VisualElement element, params string[] styleSheets)
    {
        foreach (var item in styleSheets)
            element.styleSheets.Add((StyleSheet)EditorGUIUtility.Load(item));
        return element;
    }

    public static bool HasElements<T>(this List<T> elements)
    {
        return elements != null && elements.Count > 0;
    }
    public static bool HasElements<T>(this IEnumerable<T> elements)
    {
        if (elements == null) return false;
        return HasElements(new List<T>(elements));
    }

    public static T FirstElement<T>(this IEnumerable<T> arr)
    {
        if (arr == null) return default(T);
        var list = new List<T>(arr);
        return list.HasElements() ? list[0] : default(T);
    }

    public static string RemoveString(this string s, string toRemove)
    {
        return s.Replace(toRemove, "");
    }

}

#endif