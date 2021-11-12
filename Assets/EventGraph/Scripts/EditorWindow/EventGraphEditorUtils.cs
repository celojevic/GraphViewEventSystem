#if UNITY_EDITOR

using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

using UnityEngine.UIElements;

public static class EventGraphEditorUtils
{

    public static TextField CreateTextField(string value = null, string label = null,
        EventCallback<ChangeEvent<string>> onValueChanged = null)
    {
        TextField textField = new TextField() 
        { 
            value = value ,
            label = label            
        };

        if (onValueChanged != null)
            textField.RegisterValueChangedCallback(onValueChanged);

        textField.multiline = true;

        return textField;
    }

    public static Button CreateButton(string text, Action onClick = null)
    {
        return new Button(onClick) { text = text };
    }

    public static Port CreatePort(this NodeBase node, string portName = "", 
        Direction direction = Direction.Output,
        Port.Capacity capacity = Port.Capacity.Single,
        Orientation orientation = Orientation.Horizontal)
    {
        Port port = node.InstantiatePort(orientation, direction, capacity, default);
        port.portName = portName;
        return port;
    }
    public static Port CreateInputPort(this NodeBase node)
    {
        return node.CreatePort("Input", Direction.Input, Port.Capacity.Multi, Orientation.Horizontal);
    }

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
        return new List<T>(arr)[0];
    }


}

#endif