using UnityEngine;
using EventGraph.Runtime;

namespace EventGraph
{
#if UNITY_EDITOR

    using UnityEditor.Experimental.GraphView;

    [System.Serializable]
    public class EntryNode : NodeBase
    {
        public EntryNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
        {
            SetPosition(new Rect(new Vector2(100, 200), Vector2.zero));
            DrawNode();
        }

        protected override string colorHex => "";

        public override string Serialize()
        {
            return JsonUtility.ToJson(new EntryNodeData(this));
        }

        protected override void DrawNode()
        {
            title = "Start";

            PortBase port = this.CreatePort(
                "Output", Direction.Output, Port.Capacity.Single, Orientation.Horizontal);
            outputContainer.Add(port);

            capabilities = 0;

            RefreshExpandedState();
        }
    }

#endif

    [System.Serializable]
    public class EntryNodeData : NodeDataBase
    {

        public EntryNodeData(EntryNodeData data) : base(data) { }

#if UNITY_EDITOR
        public EntryNodeData(NodeBase node) : base(node)
        {
        }
#endif

        public override void Parse(EventGraphParser parser)
        {
            Debug.LogError("this node need not be parsed");
            throw new System.NotImplementedException();
        }

    }
}