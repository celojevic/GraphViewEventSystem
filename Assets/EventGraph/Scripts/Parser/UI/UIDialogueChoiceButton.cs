using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EventGraph.Runtime.UI
{

    public class UIDialogueChoiceButton : MonoBehaviour
    {

        public Button button;
        public TMP_Text tmpText;

        public ChoiceAction choiceAction;

        public string text
        {
            get => tmpText.text;
            set => tmpText.text = value;
        }

        public void OnClick()
        {
            choiceAction.callback.Invoke();
            button.enabled = false;
        }

        public void Setup(ChoiceAction choiceAction)
        {
            this.choiceAction = choiceAction;
            this.text = choiceAction.choice;
        }

    }

}
