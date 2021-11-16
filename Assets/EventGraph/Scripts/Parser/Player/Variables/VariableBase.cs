using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableBase<T> : ScriptableObject
{

    public T value;

    #region Get

    private static Dictionary<string, VariableBase<T>> variables = new Dictionary<string, VariableBase<T>>();

    public static VariableBase<T> Get(string objName)
    {
        if (!variables.HasElements())
        {
            var list = EventGraphEditorUtils.FindScriptableObjects<VariableBase<T>>();
            if (list.HasElements())
            {
                foreach (var item in list)
                {
                    variables.Add(item.name, item);
                }
            }
            else
            {
                return null;
            }
        }

        if (variables.ContainsKey(objName))
        {
            return variables[objName];
        }
        else
        {
            Debug.LogError("Could not find data obj with name: " + objName);
            return null;
        }
    }

    #endregion

}
