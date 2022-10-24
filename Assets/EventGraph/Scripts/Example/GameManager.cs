using EventGraph.Runtime.UI;
using UnityEngine;
using UnityEngine.UI;

namespace EventGraph.Runtime.Example
{

    public class GameManager : MonoBehaviour
    {

        [SerializeField] private Button _talkButton = null;
        [SerializeField] private EventGraphDataObject _event = null;

        private void OnEnable()
        {
            _talkButton.onClick.AddListener(OnTalk);
        }

        private void OnDisable()
        {
            _talkButton.onClick.RemoveListener(OnTalk);
        }

        private void OnTalk()
        {
            EventGraphParser parser = new EventGraphParser(_event.graphData, this);
            parser.StartParsing();
            parser.OnStopParsing = UIDialogue.instance.Hide;
        }

        // conceptual example usage with FishNet. doesn't actually work.
#if FISHNET
        [Server]
        void OnTalkNetwork()
        {
            // parse on server
            EventGraphParser parser = new EventGraphParser(Event.graphData);
            parser.Parent = this;
            parser.Player = player;
            parser.StartParsing();

            // tell player which node to parse specifically
            TargetStartParsing(conn, parser.curNodeGuid);
        }

        [TargetRpc]
        public void TargetStartParsing(NetworkConnection conn, string guid)
        {
            _parser = new EventGraphParser(_event.graphData);
            _parser.Player = conn.FirstObject.GetComponent<Player>();
            _parser.ParseNode(guid);
        }
#endif

    }
}