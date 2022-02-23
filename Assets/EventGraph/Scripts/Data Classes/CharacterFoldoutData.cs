using EventGraph.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterFoldoutData
{

    public string characterName;
    public string expression;
    public DialoguePosition dialoguePosition;

    public CharacterFoldoutData(ChoiceNode node)
    {
        if (node == null) return;

        this.characterName = node?.character?.name;
        this.expression = node?.expression;
        this.dialoguePosition = node.dialoguePosition;
    }

}