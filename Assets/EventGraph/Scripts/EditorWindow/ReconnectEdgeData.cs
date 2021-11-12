#if UNITY_EDITOR
 
public class ReconnectEdgeData
{
    public int portIndex;
    public string oldParentGuid;
    public string newParentGuid;
    public string oldNextGuid;
    public string newNextGuid;

    public override string ToString()
    {
        return $"PortIndex: {portIndex}\n" +
            $"old parent guid: {oldParentGuid} | old next guid: {oldNextGuid}\n" +
            $"new parent guid: {newParentGuid} | new next guid: {newNextGuid}";
    }
}

#endif