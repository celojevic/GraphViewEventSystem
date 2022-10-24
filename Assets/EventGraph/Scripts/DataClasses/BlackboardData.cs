using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace EventGraph
{
    public class BlackboardData : EventGraphElementData
    {




        public BlackboardData(EventGraphElementData data) : base(data)
        {
        }

#if UNITY_EDITOR
        public BlackboardData(GraphElement graphElement) : base(graphElement)
        {
        }
#endif



    }
}