using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

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

    public void OnDropOutsidePort(Edge edge, Vector2 position)
    {
        // TODO positioning messed up if too far from entry node
        Vector2 worldMousePosition = _graphView.editorWindow.rootVisualElement.ChangeCoordinatesTo(
            _graphView.editorWindow.rootVisualElement.parent, 
            position + _graphView.editorWindow.position.position // (+) gives the right coords here
        );
        Vector2 localMousePosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition);

        _graphView.OpenSearchWindow(localMousePosition, _port);
    }

    public void OnDrop(GraphView graphView, Edge edge)
    {
        _edgesToCreate.Clear();
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

    }

}
