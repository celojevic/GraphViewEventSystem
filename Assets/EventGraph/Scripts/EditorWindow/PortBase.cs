using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PortBase : Port
{

    public void SetEdgeConnector(EdgeConnector value)
    { 
        m_EdgeConnector = value; 
    }

    public PortBase(Orientation orientation, Direction direction, Capacity capacity, Type type)
        : base(orientation, direction, capacity, type)
    {

    }

    public override void OnStopEdgeDragging()
    {
        base.OnStopEdgeDragging();
        Debug.Log("Stopped dragging");
    }


}
