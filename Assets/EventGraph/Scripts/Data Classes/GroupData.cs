using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

[System.Serializable]
public class GroupData : EventGraphElementData
{

    public string title;
    public List<string> nodeGuids;

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

}