using UnityEngine;
using UnityEditor;

public abstract class VariableBase<T> : ScriptableGuidObject
{

    public T value;

}

public class ScriptableGuidObject : ScriptableObject
{
    [SerializeField]
    public string guid;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(guid))
        {
            guid = System.Guid.NewGuid().ToString();
            Debug.Log(guid);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
#endif

    public bool IsGuid(string otherGuid) => otherGuid == guid;

}