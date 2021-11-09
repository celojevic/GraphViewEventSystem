using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogue : MonoBehaviour
{

    public static UIDialogue instance;

    [SerializeField] private TMP_Text _messageText = null;
    [SerializeField] private Transform _choiceHolder = null;

    [Header("Prefabs")]
    [SerializeField] private Button _choicePrefab = null;

    private void Awake()
    {
        instance = this;
    }

    public void ShowMessage(string message, List<ChoiceAction> choices)
    {
        _messageText.text = message;

        _choiceHolder.DestroyChildren();
        for (int i = 0; i < choices.Count; i++)
        {
            var index = i;
            Button prefab = Instantiate(_choicePrefab, _choiceHolder);
            prefab.GetComponentInChildren<TMP_Text>().text = choices[i].choice;
            prefab.onClick.AddListener(() => choices[index].callback?.Invoke());
        }
    }


}
