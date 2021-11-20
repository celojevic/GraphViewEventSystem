using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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


}
