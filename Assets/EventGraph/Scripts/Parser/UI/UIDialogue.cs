using EventGraph.Characters;
using EventGraph.Components;
using EventGraph.Databases;
using EventGraph.Runtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EventGraph.Runtime.UI
{
    public class UIDialogue : MonoBehaviour
    {

        public static UIDialogue instance;

        [SerializeField] private GameObject _panel = null;

        [EnumNameArray(typeof(DialoguePosition))]
        [SerializeField] private UIDialogueBox[] _dialogueBoxes = new UIDialogueBox[(int)DialoguePosition.Count];
        public UIDialogueBox[] dialogueBoxes => _dialogueBoxes;

        private AudioSource _audioSource;

        private void Awake()
        {
            instance = this;
            _audioSource = GetComponent<AudioSource>();
            Hide();
        }

        public UIDialogueBox GetActiveDialogueBox()
        {
            for (int i = 0; i < _dialogueBoxes.Length; i++)
                if (_dialogueBoxes[i].gameObject.activeInHierarchy)
                    return _dialogueBoxes[i];

            return null;
        }

        public void ShowMessage(string message, List<ChoiceAction> choices, AudioClip voiceClip = null, CharacterFoldoutData character = null)
        {
            _panel.SetActive(true);

            // play voice line
            if (voiceClip != null)
            {
                _audioSource.Stop();
                _audioSource.clip = voiceClip;
                _audioSource.Play();
            }

            int index = character != null ? (int)character.dialoguePosition : 0;

            for (int i = 0; i < _dialogueBoxes.Length; i++)
            {
                if (i == index)
                {
                    _dialogueBoxes[i].gameObject.SetActive(true);
                    _dialogueBoxes[i].Setup(message, choices, character);
                }
                else
                {
                    _dialogueBoxes[i].gameObject.SetActive(false);
                }
            }

        }


        public void Hide()
        {
            foreach (var item in _dialogueBoxes)
            {
                Shaker shaker = item.portrait.GetComponent<Shaker>();
                if (shaker != null)
                    Destroy(shaker);
            }

            _panel.SetActive(false);
        }


    }
}