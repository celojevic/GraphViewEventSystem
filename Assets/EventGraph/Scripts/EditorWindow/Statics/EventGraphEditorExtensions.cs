
namespace EventGraph
{
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


        #region Edges

        public static bool ConnectsVarToCndNode(this Edge edge)
        {
            return
                // edge exists
                edge != null
                // ports exist
                && edge.input != null && edge.output != null
                // input port on cnd node should always be Value
                && edge.input.portName == "Value"
                // nodes exist
                && edge.input.node != null && edge.output.node != null
                // make sure they are both generic types
                && edge.input.node.GetType().BaseType.IsGenericType
                && edge.output.node.GetType().BaseType.IsGenericType
                // check if they are var node to cnd node
                && edge.input.node.GetType().BaseType.GetGenericTypeDefinition() == typeof(ConditionalNode<>)
                && edge.output.node.GetType().BaseType.GetGenericTypeDefinition() == typeof(VariableNodeBase<>);
        }

        public static void SetValueNodeGuid(this Edge edge, bool asNull)
        {
            try
            {
                edge.input.node.GetType().GetField("valueNodeGuid")
                    .SetValue(edge.input.node,
                        asNull ? null : (edge.output.node as NodeBase).guid);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
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

        public static Color Invert(this Color color)
        {
            return new Color(1f - color.r, 1f - color.g, 1f - color.b);
        }

    }


#endif

}
