using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EventGraph.Editor
{
    [CustomEditor(typeof(EventGraphDataObject))]
    public class EventGraphDataObjectEditor : UnityEditor.Editor
    {
        private EventGraphDataObject _target;

        private void OnEnable()
        {
            _target = target as EventGraphDataObject;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor"))
            {
                EventGraphEditorWindow.Open(_target);
            }

            base.OnInspectorGUI();
        }
    }
}