using EventGraph.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventGraph.Database
{

    public class Database : MonoBehaviour
    {

        [SerializeField] private List<Character> _characters = new List<Character>();
        public static Dictionary<string, Character> characters;

        [SerializeField] private List<ScriptableGuidObject> _variables = new List<ScriptableGuidObject>();
        public static Dictionary<string, ScriptableGuidObject> variables;

        private void Awake()
        {
            InitDicts();
        }

        private void InitDicts()
        {
            characters = new Dictionary<string, Character>();
            foreach (var item in _characters)
                characters.Add(item.guid, item);

            variables = new Dictionary<string, ScriptableGuidObject>();
            foreach (var item in _variables)
                variables.Add(item.guid, item);
        }

        public static ScriptableGuidObject GetVariable(string guid)
        {
            variables.TryGetValue(guid, out ScriptableGuidObject var);
            return var;
        }

        public static Character GetCharacter(string name)
        {
            characters.TryGetValue(name, out Character clip);
            return clip;
        }


#if UNITY_EDITOR

        private void OnValidate()
        {

            _variables = EventGraphEditorUtils.FindScriptableObjects<ScriptableGuidObject>();

        }

#endif


    }

}
