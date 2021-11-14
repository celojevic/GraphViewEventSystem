#if UNITY_EDITOR

using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public static class EventGraphEditorUtils
{

    public static Image CreateImage(string iconName)
    {
        Image icon = new Image();
        icon.image = LoadIcon(iconName);
        return icon;
    }

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

    public static void RemoveCollapse(this Node node)
    {
        node.titleContainer.RemoveAt(1);
    }

    public static ObjectField CreateObjectField(Type type)
    {
        ObjectField objField = new ObjectField();
        objField.objectType = type;

        return objField;
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

    /// <summary>
    /// Loads a texture from "EventGraph/Icons".
    /// </summary>
    /// <param name="iconName"></param>
    /// <returns></returns>
    public static Texture2D LoadIcon(string iconName)
    {
        return AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/EventGraph/Icons/{iconName}.png");
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

    public static string RemoveString(this string s, string toRemove)
    {
        return s.Replace(toRemove, "");
    }

    public static List<T> FindScriptableObjects<T>() where T : ScriptableObject
    {
        List<T> list = new List<T>();
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);

        for (int i = 0; i < guids.Length; i++)
            list.Add(AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[i])));

        return list;
    }

}

#endif