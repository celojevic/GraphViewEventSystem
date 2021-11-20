using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

// TODO set this up and use it in EventGraphEditorExtensions.CreatePort
public class EdgeConnectorBase : EdgeConnector
{
    public override EdgeDragHelper edgeDragHelper => throw new System.NotImplementedException();

    protected override void RegisterCallbacksOnTarget()
    {
        throw new System.NotImplementedException();
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        throw new System.NotImplementedException();
    }
}
