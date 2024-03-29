using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EventGraph.Editor
{
#if UNITY_EDITOR

    using UnityEditor.Experimental.GraphView;

    public class EdgeConnectorListenerBase : IEdgeConnectorListener
    {

        /// <summary>
        /// Parent port
        /// </summary>
        private PortBase _port;
        private EventGraphView _graphView;
        private GraphViewChange _graphViewChange;
        private List<Edge> _edgesToCreate;
        private List<GraphElement> _edgesToDelete;

        public EdgeConnectorListenerBase(EventGraphView graphView, PortBase port)
        {
            _edgesToCreate = new List<Edge>();
            _edgesToDelete = new List<GraphElement>();
            _graphViewChange.edgesToCreate = _edgesToCreate;

            _graphView = graphView;
            _port = port;
        }

        // position = pos relative to editor window, origin at top-left
        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            _graphView.OpenSearchWindow(_graphView.editorWindow.position.position + position, _port);
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            if (edge is not EdgeBase e)
            {
                Debug.LogError("Invalid edge type, must be EdgeBase: " + edge?.GetType());
                return;
            }

            _edgesToCreate.Clear();

            // TODO prevent dropping value nodes on anything but value ports
            //if (e.input.node.GetType().BaseType.IsGenericType && e.output.node.GetType().BaseType.IsGenericType
            //    && e.input.portName == "Value"
            //    && e.input.node.GetType().BaseType.GetGenericTypeDefinition() != typeof(ConditionalNode<>)
            //    || e.output.node.GetType().BaseType.GetGenericTypeDefinition() != typeof(VariableNodeBase<>))
            //{
            //    return;
            //}

            _edgesToCreate.Add(edge);

            _edgesToDelete.Clear();

            if (edge.input.capacity == Port.Capacity.Single)
            {
                foreach (Edge connection in edge.input.connections)
                {
                    if (connection != edge)
                    {
                        _edgesToDelete.Add(connection);
                    }
                }
            }

            if (edge.output.capacity == Port.Capacity.Single)
            {
                foreach (Edge connection2 in edge.output.connections)
                {
                    if (connection2 != edge)
                    {
                        _edgesToDelete.Add(connection2);
                    }
                }
            }

            if (_edgesToDelete.Count > 0)
            {
                graphView.DeleteElements(_edgesToDelete);
            }

            List<Edge> edgesToCreate = _edgesToCreate;
            if (graphView.graphViewChanged != null)
            {
                edgesToCreate = graphView.graphViewChanged(_graphViewChange).edgesToCreate;
            }

            foreach (Edge item in edgesToCreate)
            {
                graphView.AddElement(item);
                edge.input.Connect(item);
                edge.output.Connect(item);
            }

            // check if var node was dropped on cndNode value port
            if (e != null && e.ConnectsVarToCndNode())
            {
                e.SetValueNodeGuid(false);
            }

        }

    }

#endif
}