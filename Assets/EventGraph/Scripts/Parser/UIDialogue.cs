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
    [SerializeField] private Image[] _portraits = new Image[(int)DialoguePosition.Count];

    [Header("Prefabs")]
    [SerializeField] private Button _choicePrefab = null;

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
            int index = i;
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
            Character so = Database.GetCharacter(character.characterGuid);
            if (so == null) return;

            CharacterExpression exp = so.GetExpression(character.expression);
            if (exp == null) return;

            SetPortrait(character.dialoguePosition, exp.Sprite);
        }

    }

    void SetPortrait(DialoguePosition pos, Sprite sprite)
    {
        for (DialoguePosition i = 0; i < DialoguePosition.Count; i++)
        {
            _portraits[(int)i].enabled = i == pos;
            if (i==pos)
                _portraits[(int)i].sprite = sprite;
        }
    }

    public void Hide()
    {
        _panel.SetActive(false);
    }


}
