using EventGraph.Constants;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using EventGraph.Runtime;

namespace EventGraph
{

#if UNITY_EDITOR

    using UnityEditor.UIElements;
    using EventGraph.Editor;

    public class WaitNode : NodeBase
    {

        public float timeToWait;

        protected override string colorHex => ColorConstants.LAVENDER;


        #region Constructors

        public WaitNode(WaitNode copy) : base(copy)
        {
            this.timeToWait = copy.timeToWait;
            DrawNode();
        }

        public WaitNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
        {
            DrawNode();
        }

        public WaitNode(EventGraphView graphView, WaitNodeData nodeData) : base(graphView, nodeData)
        {
            this.timeToWait = nodeData.timeToWait;
            DrawNode();
        }

        #endregion


        public override string Serialize()
        {
            return JsonUtility.ToJson(new WaitNodeData(this));
        }

        protected override void DrawNode()
        {
            titleContainer.Add(new Label("Time to Wait"));
            SetNodeColor();

            inputContainer.Add(this.CreateInputPort());
            outputContainer.Add(this.CreatePort("Output"));

            FloatField timeField = new FloatField();
            timeField.value = this.timeToWait;
            timeField.RegisterValueChangedCallback(evt =>
            {
                this.timeToWait = evt.newValue;
            });
            mainContainer.Add(timeField);

            titleContainer.Insert(0, EventGraphEditorUtils.CreateImage("Wait"));

            RefreshExpandedState();
        }

    }

#endif

    public class WaitNodeData : NodeDataBase
    {

        public float timeToWait;


        #region Constructors

        public WaitNodeData(WaitNodeData copy) : base(copy)
        {
            this.timeToWait = copy.timeToWait;
        }

#if UNITY_EDITOR
        public WaitNodeData(WaitNode node) : base(node)
        {
            this.timeToWait = node.timeToWait;
        }
#endif

        #endregion


        #region Runtime

        public override void Parse(EventGraphParser parser)
        {
            parser.StartCoroutine(WaitNodeCo(parser));
        }

        IEnumerator WaitNodeCo(EventGraphParser parser)
        {
            yield return new WaitForSeconds(this.timeToWait);

            parser.SetNext();
        }

        #endregion


    }

}
