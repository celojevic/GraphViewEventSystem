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
    using UnityEngine.SceneManagement;

    public class ChangeSceneNode : NodeBase
    {

        public string sceneName;

        protected override string colorHex => ColorConstants.VERMILION;


        #region Constructors

        public ChangeSceneNode(ChangeSceneNode copy) : base(copy)
        {
            this.sceneName = copy.sceneName;
		
			// This must be called here to draw in the right order
            DrawNode();
        }

        public ChangeSceneNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
        {
            DrawNode();
        }

        public ChangeSceneNode(EventGraphView graphView, ChangeSceneNodeData nodeData) : base(graphView, nodeData)
        {
            // Copy node data here
            this.sceneName = nodeData.sceneName;
            DrawNode();
        }

        #endregion


        public override string Serialize()
        {
            return JsonUtility.ToJson(new ChangeSceneNodeData(this));
        }

        protected override void DrawNode()
        {
			// Set title and color
            titleContainer.Add(new Label("ChangeSceneNode"));
            SetNodeColor();

            // Create input and output ports
            inputContainer.Add(this.CreateInputPort());
            outputContainer.Add(this.CreatePort("Output"));

            // Add custom drawing here
            extensionContainer.Add(new Label("Scene"));
            var sceneField = EventGraphEditorUtils.CreateTextField(sceneName, null,
                evt => { sceneName = evt.newValue; });
            extensionContainer.Add(sceneField);
			
			// Refresh last
            RefreshExpandedState();
        }

    }

#endif

    public class ChangeSceneNodeData : NodeDataBase
    {
        
        public string sceneName;


        #region Constructors

        public ChangeSceneNodeData(ChangeSceneNodeData copy) : base(copy)
        {
            // Copy node data here
            this.sceneName = copy.sceneName;
        }

#if UNITY_EDITOR
        public ChangeSceneNodeData(ChangeSceneNode node) : base(node)
        {
            // Copy node data here
            this.sceneName = node.sceneName;
        }
#endif

        #endregion


        #region Runtime

        public override void Parse(EventGraphParser parser)
        {
            // Put custom runtime parsing code here
            SceneManager.LoadScene(sceneName);
        }

        #endregion

    }

}
