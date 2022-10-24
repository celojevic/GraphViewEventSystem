using EventGraph.Constants;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using EventGraph.Runtime;

namespace EventGraph
{
    using EventGraph.Components;

#if UNITY_EDITOR

    using EventGraph.Editor;
    using EventGraph.Runtime.UI;
    using UnityEditor.UIElements;

    public class PortraitShakeNode : NodeBase
    {

        public float intensity;
        public float duration;

        protected override string colorHex => ColorConstants.CELADON;


        #region Constructors

        public PortraitShakeNode(PortraitShakeNode copy) : base(copy)
        {
            this.intensity = copy.intensity;
            this.duration = copy.duration;
		
			// This must be called here to draw in the right order
            DrawNode();
        }

        public PortraitShakeNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
        {
            DrawNode();
        }

        public PortraitShakeNode(EventGraphView graphView, PortraitShakeNodeData nodeData) : base(graphView, nodeData)
        {
            this.intensity = nodeData.intensity;
            this.duration = nodeData.duration;
		
            DrawNode();
        }

        #endregion


        public override string Serialize()
        {
            return JsonUtility.ToJson(new PortraitShakeNodeData(this));
        }

        protected override void DrawNode()
        {
			// Set title and color
            titleContainer.Add(new Label("PortraitShakeNode"));
            SetNodeColor();

            // Create input and output ports
            inputContainer.Add(this.CreateInputPort());
            outputContainer.Add(this.CreatePort("Output"));

            // intensity
            extensionContainer.Add(new Label("Intensity"));
            FloatField intensityField = new FloatField();
            intensityField.value = intensity;
            intensityField.RegisterValueChangedCallback((evt) => { this.intensity = evt.newValue; });
            extensionContainer.Add(intensityField);

            // duration
            extensionContainer.Add(new Label("Duration"));
            FloatField durationField = 
                EventGraphEditorUtils.CreateFloatField(null, duration, (evt) => { this.duration = evt.newValue; });
            extensionContainer.Add(durationField);

            // Refresh last
            RefreshExpandedState();
        }

    }

#endif

    public class PortraitShakeNodeData : NodeDataBase
    {

        public float intensity;
        public float duration;


        #region Constructors

        public PortraitShakeNodeData(PortraitShakeNodeData copy) : base(copy)
        {
            this.intensity = copy.intensity;
            this.duration = copy.duration;
        }

#if UNITY_EDITOR
        public PortraitShakeNodeData(PortraitShakeNode node) : base(node)
        {
            this.intensity = node.intensity;
            this.duration = node.duration;
        }
#endif

        #endregion


        #region Runtime

        public override void Parse(EventGraphParser parser)
        {
            foreach (var dialogueBox in UIDialogue.instance.dialogueBoxes)
            {
                Shaker shaker = dialogueBox.portrait.GetComponent<Shaker>();
                if (shaker == null)
                    shaker = dialogueBox.portrait.gameObject.AddComponent<Shaker>();
                shaker.Shake(intensity, duration);
            }

            parser.SetNext();
        }

        #endregion

    }

    public enum PortraitShake
    {
        /// <summary>
        /// If both are active, defaults to left.
        /// </summary>
        Active,
        Left,Right,

        Count
    }

}
