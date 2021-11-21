
[System.Serializable]
public struct EdgeData
{
    /// <summary>
    /// Index of the edge in the parent node's output container.
    /// </summary>
    public int portIndex;
    /// <summary>
    /// Guid of the parent node the edge originates from.
    /// </summary>
    public string parentNodeGuid;
    /// <summary>
    /// Guid of the node the edge connects to.
    /// </summary>
    public string toNodeGuid;
    /// <summary>
    /// Type of edge connection. Can be "" or "var".
    /// </summary>
    public string edgeType;
}