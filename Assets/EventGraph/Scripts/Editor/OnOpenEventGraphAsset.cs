using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace EventGraph.Editor
{
    // https://www.youtube.com/watch?v=c_3DXBrH-Is&t=580s&ab_channel=GameDevGuide
    public class OnOpenEventGraphAsset
    {

        [OnOpenAsset]//(OnOpenAssetAttributeMode.Execute)]
        public static bool OpenEditor(int instanceId, int line)
        {
            EventGraphDataObject obj = EditorUtility.InstanceIDToObject(instanceId) as EventGraphDataObject;
            if (obj != null)
            {
                EventGraphEditorWindow.Open(obj);
                return true;
            }

            return false;
        }

    }
}