using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif

[System.Serializable]
public class EventGraphElementData
{

    public string guid;
    public Vector2 position;

    public EventGraphElementData(EventGraphElementData data)
    {
        this.guid = data.guid;
        this.position = data.position;
    }

    public EventGraphElementData(GraphElement ge)
    {
        this.guid = ge.viewDataKey;
        this.position = ge.GetPosition().position;
    }

}