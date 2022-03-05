using EventGraph.Characters;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EventGraph.Runtime.UI
{
    public class UIDialogueBox : MonoBehaviour
    {

        [SerializeField] private Image _image = null;
        public Image portrait => _image;

        [SerializeField] private TMP_Text _text = null;
        [SerializeField] private Transform _choiceHolder = null;

        [SerializeField] private UIDialogueChoiceButton _choicePrefab = null;

        public void Setup(string message, List<ChoiceAction> choices, CharacterFoldoutData character = null)
        {
            _image.enabled = true;

            _text.text = message;

            _choiceHolder.DestroyChildren();
            for (int i = 0; i < choices.Count; i++)
            {
                int index = i;
                UIDialogueChoiceButton prefab = Instantiate(_choicePrefab, _choiceHolder);
                prefab.Setup(choices[index]);
            }

            // show character portrait and setup its position
            if (character != null)
            {
                Character so = EventGraph.Databases.Database.GetCharacter(character.characterGuid);
                if (so == null) return;

                CharacterExpression exp = so.GetExpression(character.expression);
                if (exp == null) return;

                _image.sprite = exp.Sprite;
            }
        }

    }
}