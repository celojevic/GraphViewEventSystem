using EventGraph.Characters;
using EventGraph.Constants;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using EventGraph.Runtime;
using EventGraph.Editor;
using EventGraph.Runtime.UI;

#if UNITY_EDITOR

using UnityEditor.UIElements;

namespace EventGraph
{

    public class ChoiceNode : NodeBase
    {

        public string message;
        public AudioClip voiceClip;
        public List<string> choices;

        public Character character;
        public string expression;
        public DialoguePosition dialoguePosition;

        protected override string colorHex => ColorConstants.CERULEAN;


        #region Constructors

        public ChoiceNode(ChoiceNode copy) : base(copy)
        {
            this.message = copy.message;
            this.choices = copy.choices;
            this.voiceClip = copy.voiceClip;

            DrawNode();
        }

        public ChoiceNode(Vector2 pos, EventGraphView graphView) : base(pos, graphView)
        {
            this.message = "Choice Node";
            this.choices = new List<string>();
            this.choices.Add("Next");

            DrawNode();
        }

        public ChoiceNode(EventGraphView graphView, ChoiceNodeData nodeData) : base(graphView, nodeData)
        {
            if (nodeData.message=="why u lie")
            {
                int i = 0;
            }

            this.message = nodeData.message;
            this.choices = nodeData.choices;
            if (!string.IsNullOrEmpty(nodeData.voiceClipName))
                this.voiceClip = EventGraphEditorUtils.FindAudioClip(nodeData.voiceClipName);

            // character foldout data
            this.character = EventGraphEditorUtils.FindScriptableObjects<Character>()
                .Find(x => x.guid == nodeData.characterFoldoutData.characterGuid);
            this.expression = nodeData.characterFoldoutData.expression;
            this.dialoguePosition = nodeData.characterFoldoutData.dialoguePosition;

            DrawNode();
        }

        #endregion


        #region Drawing

        protected override void DrawNode()
        {
            DrawTitleContainer();
            DrawInputContainer();
            DrawOutputContainer();
            DrawMainContainer();
            DrawExtensionContainer();

            RefreshExpandedState();
        }

        private void DrawExtensionContainer()
        {
            DrawCharacterFoldout();

            // voice clip object field
            ObjectField voiceClipField = EventGraphEditorUtils.CreateObjectField(
                typeof(AudioClip), voiceClip, "Voice Clip");
            voiceClipField.RegisterValueChangedCallback(evt =>
            {
                this.voiceClip = evt.newValue as AudioClip;
            });
            extensionContainer.Add(voiceClipField);

            TextField messageTextField = EventGraphEditorUtils.CreateTextField(message, "",
                (evt) =>
                {
                    message = evt.newValue;
                });
            extensionContainer.Add(messageTextField);
        }

        // TODO move to own class
        private Foldout _characterFoldout;
        private PopupField<string> _expressionsPopup;

        private void CreateExpressionsPopup()
        {
            var list = new List<string>();
            if (character != null && character.Expressions.HasElements())
                foreach (var item in character.Expressions)
                    list.Add(item.Expression);

            if (list.HasElements() && list.Contains(this.expression))
            {
                _expressionsPopup = new PopupField<string>("Expression", list, this.expression);
                _expressionsPopup.RegisterValueChangedCallback((evt) =>
                {
                    this.expression = evt.newValue;
                });
            }
            else if (list.HasElements())
            {
                _expressionsPopup = new PopupField<string>("Expression", list, 0);
                _expressionsPopup.RegisterValueChangedCallback((evt) =>
                {
                    this.expression = evt.newValue;
                });
                this.expression = list[0];
            }
            else
            {
                _expressionsPopup = new PopupField<string>();
            }

            _characterFoldout.Add(_expressionsPopup);
        }

        /// <summary>
        /// In the extension container.
        /// TODO move this into a new node and make choiceNode a stack node that can contain and parse it
        /// </summary>
        private void DrawCharacterFoldout()
        {
            _characterFoldout = new Foldout();
            _characterFoldout.text = "Character Data";

            // character SO field
            ObjectField characterField = EventGraphEditorUtils.CreateObjectField(
                typeof(Character), character, "Character");
            characterField.RegisterValueChangedCallback(evt =>
            {
                if ((evt.newValue as Character) == this.character) return;

                this.character = evt.newValue as Character;
                _characterFoldout.Remove(_expressionsPopup);
                CreateExpressionsPopup();
            });
            _characterFoldout.Add(characterField);

            // dialogue position enum field
            EnumField dialoguePositionField = new EnumField("Dialogue Position", this.dialoguePosition);
            dialoguePositionField.RegisterValueChangedCallback((evt) =>
            {
                this.dialoguePosition = (DialoguePosition)evt.newValue;
            });
            _characterFoldout.Add(dialoguePositionField);

            // character expression
            CreateExpressionsPopup();

            extensionContainer.Add(_characterFoldout);

        }

        private void DrawMainContainer()
        {
        }

        protected void DrawTitleContainer()
        {
            this.RemoveCollapse();

            titleContainer.Insert(0, EventGraphEditorUtils.CreateImage("Dialogue"));

            // TODO center, make bigger, make bold
            Label label = new Label("Choice Node");
            titleContainer.Add(label);

            SetNodeColor();
        }

        private void DrawInputContainer()
        {
            inputContainer.Add(this.CreateInputPort());
        }

        private void DrawOutputContainer()
        {
            // add choice button
            Button addChoiceButton = EventGraphEditorUtils.CreateButton("Add Choice", () =>
            {
                choices.Add("New Choice");
                CreateChoicePort("New Choice");
            });
            outputContainer.Add(addChoiceButton);

            // choice ports
            foreach (string choiceText in choices)
            {
                CreateChoicePort(choiceText);
            }
        }

        #endregion


        private void CreateChoicePort(string choiceText)
        {
            PortBase outputPort = this.CreatePort();

            Button deleteChoiceButton = EventGraphEditorUtils.CreateButton("X", () =>
            {
                if (choices.Count == 1) return;

                if (outputPort.connected)
                    graphView.DeleteElements(outputPort.connections);

                List<VisualElement> list = new List<VisualElement>(outputContainer.Children());
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] == outputPort)
                    {
                        // offset the index for non-choice elements in the outputContainer
                        choices.RemoveAt(i - (list.Count - choices.Count));
                        break;
                    }
                }

                graphView.RemoveElement(outputPort);

            });
            outputPort.Add(deleteChoiceButton);

            TextField choiceTextField = EventGraphEditorUtils.CreateTextField(choiceText);
            outputPort.Add(choiceTextField);


            outputContainer.Add(outputPort);
        }

        public override string Serialize()
        {
            return JsonUtility.ToJson(new ChoiceNodeData(this));
        }

    }

#endif


    [System.Serializable]
    public class ChoiceNodeData : NodeDataBase
    {

        public string message;
        public List<string> choices = new List<string>();
        public string voiceClipName;
        public CharacterFoldoutData characterFoldoutData;


        #region Constructors

        public ChoiceNodeData(ChoiceNodeData data) : base(data)
        {
            this.message = data.message;
            this.choices = data.choices;
            this.voiceClipName = data.voiceClipName;
            this.characterFoldoutData = data.characterFoldoutData;
        }

#if UNITY_EDITOR
        public ChoiceNodeData(ChoiceNode node) : base(node)
        {
            message = node.message;
            this.voiceClipName = node.voiceClip?.name;
            this.characterFoldoutData = new CharacterFoldoutData(node);

            List<VisualElement> nodeOutputElements = new List<VisualElement>(node.outputContainer.Children());
            for (int i = 0; i < nodeOutputElements.Count; i++)
            {
                VisualElement item = nodeOutputElements[i];

                if (item is PortBase port)
                {
                    // save ports textField text
                    foreach (VisualElement portElement in port.Children())
                        if (portElement is TextField tf)
                            choices.Add(tf.text);
                }
            }
        }
#endif

        #endregion


        public override void Parse(EventGraphParser parser)
        {
            List<ChoiceAction> choiceActions = new List<ChoiceAction>();
            for (int i = 0; i < this.choices.Count; i++)
            {
                var index = i;
                choiceActions.Add(new ChoiceAction
                {
                    choice = choices[i],
                    callback = () =>
                    {
                        if (edges.Count <= index)
                        {
                        //Debug.LogWarning("ChoiceNode with edge at index doesn't go anywhere: " + index);
                        parser.StopParsing();
                            return;
                        }

                        parser.curNodeGuid = edges[index].toNodeGuid;
                        parser.Next();
                    }
                });
            }

            UIDialogue.instance.ShowMessage(
                message,
                choiceActions,
                // You can load your audio clips however you want to.
                // I simply use a manager for this example.
                EventGraph.Audio.AudioManager.GetAudioClip(voiceClipName),
                characterFoldoutData
            );
        }

    }

}