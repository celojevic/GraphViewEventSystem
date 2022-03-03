using EventGraph.Constants;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using EventGraph.Runtime;

namespace EventGraph
{

#if UNITY_EDITOR

    using EventGraph.Editor;
    using UnityEditor.UIElements;

    public class ChangeCameraNode : NodeBase
    {

        public string cameraName;

        protected override string colorHex => ColorConstants.CINNABAR;


        #region Constructors

        public ChangeCameraNode(ChangeCameraNode copy) : base(copy)
        {
            this.cameraName = copy.cameraName;

            // This must be called here to draw in the right order
            DrawNode();
        }

        public ChangeCameraNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
        {
            DrawNode();
        }

        public ChangeCameraNode(EventGraphView graphView, ChangeCameraNodeData nodeData) : base(graphView, nodeData)
        {
            // Copy node data here
            this.cameraName = nodeData.cameraName;

            DrawNode();
        }

        #endregion


        public override string Serialize()
        {
            return JsonUtility.ToJson(new ChangeCameraNodeData(this));
        }

        protected override void DrawNode()
        {
            // Set title, color, and icon
            titleContainer.Add(new Label("Change Camera"));
            SetNodeColor();
            titleContainer.Insert(0, EventGraphEditorUtils.CreateImage("ChangeCamera"));

            // Create input and output ports
            inputContainer.Add(this.CreateInputPort());
            outputContainer.Add(this.CreatePort("Output"));

            // Add custom drawing here
            extensionContainer.Add(new Label("Camera Name"));
            var nameField = EventGraphEditorUtils.CreateTextField(cameraName, null, (evt) =>
            {
                this.cameraName = evt.newValue;
            }, 
            false);
            extensionContainer.Add(nameField);

            // Refresh last
            RefreshExpandedState();
        }

    }

#endif

    public class ChangeCameraNodeData : NodeDataBase
    {

        public string cameraName;


        #region Constructors

        public ChangeCameraNodeData(ChangeCameraNodeData copy) : base(copy)
        {
            // Copy node data here
            this.cameraName = copy.cameraName;
        }

#if UNITY_EDITOR
        public ChangeCameraNodeData(ChangeCameraNode node) : base(node)
        {
            // Copy node data here
            this.cameraName = node.cameraName;
        }
#endif

        #endregion


        #region Runtime

        public override void Parse(EventGraphParser parser)
        {
            // TODO find camera with cameraName here and switch to it
        }

        #endregion

    }

}
