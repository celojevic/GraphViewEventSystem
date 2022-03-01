using EventGraph.Characters;
using EventGraph.Database;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogue : MonoBehaviour
{

    public static UIDialogue instance;

    [SerializeField] private GameObject _panel = null;
    [SerializeField] private TMP_Text _messageText = null;
    [SerializeField] private Transform _choiceHolder = null;
    [SerializeField] private Image _portraitImage = null;

    [Header("Prefabs")]
    [SerializeField] private Button _choicePrefab = null;

    [Header("Settings")]
    [Tooltip("True if the portrait should always be replaced, even when it's null.")]
    [SerializeField] private bool _replacePortrait = false;

    private AudioSource _audioSource;

    private void Awake()
    {
        instance = this;
        _audioSource = GetComponent<AudioSource>();
        Hide();
    }

    public void ShowMessage(string message, List<ChoiceAction> choices, AudioClip voiceClip = null, CharacterFoldoutData character=null)
    {
        _panel.SetActive(true);
        _messageText.text = message;

        _choiceHolder.DestroyChildren();
        for (int i = 0; i < choices.Count; i++)
        {
            var index = i;
            Button prefab = Instantiate(_choicePrefab, _choiceHolder);
            prefab.GetComponentInChildren<TMP_Text>().text = choices[i].choice;
            prefab.onClick.AddListener(() => choices[index].callback?.Invoke());
        }

        // play voice line
        if (voiceClip != null)
        {
            _audioSource.Stop();
            _audioSource.clip = voiceClip;
            _audioSource.Play();
        }

        // show character portrait and setup its position
        if (character != null)
        {
            var so = Database.GetCharacter(character.characterGuid);
            if (so == null) return;

            var exp = so.GetExpression(character.expression);
            if (exp == null)
            {
                if (_replacePortrait)
                    _portraitImage.sprite = null;
                return;
            }

            _portraitImage.sprite = exp.Sprite;
        }

    }

    public void Hide()
    {
        _panel.SetActive(false);
    }


}
