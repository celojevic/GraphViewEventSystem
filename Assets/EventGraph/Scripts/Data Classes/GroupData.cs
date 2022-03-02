using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif

namespace EventGraph
{
    [System.Serializable]
    public class GroupData : EventGraphElementData
    {

        public string title;
        public List<string> nodeGuids;

        public GroupData(GroupData data) : base(data)
        {
        }

#if UNITY_EDITOR
        public GroupData(GroupBase group) : base(group)
        {
            this.title = group.title;

            nodeGuids = new List<string>();
            foreach (GraphElement element in group.containedElements)
            {
                if (element is NodeBase node)
                    nodeGuids.Add(node.viewDataKey);
            }
        }
#endif

    }

}