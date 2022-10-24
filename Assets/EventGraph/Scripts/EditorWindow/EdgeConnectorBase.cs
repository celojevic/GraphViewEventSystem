using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventGraph
{
#if UNITY_EDITOR

    using EventGraph.Editor;
    using UnityEditor.Experimental.GraphView;

    public class EdgeConnectorBase : EdgeConnector<EdgeBase>
    {

        public EdgeConnectorBase(IEdgeConnectorListener listener) : base(listener)
        {
        }

    }

#endif
}