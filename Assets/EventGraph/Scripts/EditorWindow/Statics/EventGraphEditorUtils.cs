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

    public static AudioClip FindAudioClip(string clipName)
    {
        string[] clips = AssetDatabase.FindAssets("t:AudioClip");
        foreach (var item in clips)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(item));
            if (clip.name == clipName)
                return clip;
        }

        Debug.LogError("Couldn't find audio clip with name: " + clipName);
        return null;
    }

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

    public static ObjectField CreateObjectField(Type type, UnityEngine.Object value, string title = "")
    {
        ObjectField objField = new ObjectField(title);
        objField.objectType = type;
        objField.value = value;

        return objField;
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