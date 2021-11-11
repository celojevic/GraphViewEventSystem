using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

[System.Serializable]
public class GroupData : EventGraphElementData
{

    public List<string> nodeGuids;

    public GroupData(Group group) : base(group)
    {
        nodeGuids = new List<string>();

        foreach (GraphElement element in group.containedElements)
        {
            if (element is NodeBase node)
                nodeGuids.Add(node.viewDataKey);
        }
    }

}