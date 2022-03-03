using EventGraph.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventGraph
{
    [System.Serializable]
    public class CharacterFoldoutData
    {

        public string characterGuid;
        public string expression;
        public DialoguePosition dialoguePosition;

#if UNITY_EDITOR

        public CharacterFoldoutData(ChoiceNode node)
        {
            if (node == null) return;

            this.characterGuid = node?.character?.guid;
            this.expression = node?.expression;
            this.dialoguePosition = node.dialoguePosition;
        }

#endif

    }
}