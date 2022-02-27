using System;

#if UNITY_EDITOR

using UnityEditor.Experimental.GraphView;

public class PortBase : Port
{

    public string guid => viewDataKey;

    public PortBase(Orientation orientation, Direction direction, Capacity capacity, Type type)
        : base(orientation, direction, capacity, type)
    {

    }

    public void SetEdgeConnector(EdgeConnector value)
    {
        m_EdgeConnector = value;
    }


    public override void Disconnect(Edge edge)
    {
        if (edge.ConnectsVarToCndNode())
            edge.SetValueNodeGuid(true);

        base.Disconnect(edge);
    }

}

#endif
