using System.Collections.Generic;


namespace EventGraph
{

    [System.Serializable]
    public class EventGraphData
    {
        public string entryNode;
        public List<string> nodeJsons = new List<string>();
        public List<GroupData> groups = new List<GroupData>();
    }

}
