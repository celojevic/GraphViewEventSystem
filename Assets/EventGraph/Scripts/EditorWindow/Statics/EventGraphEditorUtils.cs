// TODO just move to Editor folder
namespace EventGraph.Editor
{
#if UNITY_EDITOR

    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    public static class EventGraphEditorUtils
    {

        /// <summary>
        /// Creates a seeded modification of the inColor using the inString as the seed.
        /// </summary>
        /// <param name="inColor"></param>Color to modify.
        /// <param name="inString"></param>String to use as a seed.
        /// <returns></returns>
        public static Color ModifyColorHSV(Color inColor, string inString, float maxChange=0.2f)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(inString);
            int seed = BitConverter.ToInt32(bytes, 0);
            System.Random rand = new System.Random(seed);
            float next = (float)rand.NextDouble() * 2f - 1f; // -1 to 1
            Color.RGBToHSV(inColor, out float h, out float s, out float v);
            float change = maxChange * next;
            return Color.HSVToRGB(h + change, s + change, v + change);
        }



        public static VisualElement CreateSpace()
        {
            return new Label(" ");
        }

        public static AudioClip FindAudioClip(string clipName)
        {
            if (string.IsNullOrEmpty(clipName)) return null;

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
            EventCallback<ChangeEvent<string>> onValueChanged = null, bool wordWrap = true)
        {
            TextField textField = new TextField()
            {
                value = value,
                label = label
            };

            if (onValueChanged != null)
                textField.RegisterValueChangedCallback(onValueChanged);

            textField.multiline = true;
            textField.style.maxWidth = 350;
            textField.style.minWidth = 100;
            textField.style.whiteSpace = wordWrap ? WhiteSpace.Normal : WhiteSpace.NoWrap;

            return textField;
        }

        public static Button CreateButton(string text, Action onClick = null)
        {
            return new Button(onClick) { text = text };
        }

        public static FloatField CreateFloatField(string label=null, float defaultValue=0f,
            EventCallback<ChangeEvent<float>> onValueChanged = null)
        {
            FloatField ff = new FloatField();
            ff.label = label;
            ff.value = defaultValue;
            ff.RegisterValueChangedCallback(onValueChanged);
            return ff;
        }

        public static void RemoveCollapse(this Node node)
        {
            node.titleContainer.RemoveAt(1);
        }

        public static ObjectField CreateObjectField(Type type, UnityEngine.Object defaultValue = null,
            string title = "")
        {
            ObjectField objField = new ObjectField(title);
            objField.objectType = type;
            objField.value = defaultValue;

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

}

