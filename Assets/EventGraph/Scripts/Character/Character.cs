using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventGraph.Characters
{

    [CreateAssetMenu(menuName = "EventGraph/Character")]
    public class Character : ScriptableGuidObject
    {

        public CharacterExpression[] Expressions;

        public CharacterExpression GetExpression(string name)
        {
            foreach (var item in Expressions)
                if (item.Expression == name)
                    return item;
            return null;
        }

    }

    [System.Serializable]
    public class CharacterExpression
    {
        public string Expression;
        public Sprite Sprite;
    }

    public enum DialoguePosition
    {
        Left,Right
    }

}
