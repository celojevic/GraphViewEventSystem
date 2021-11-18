using UnityEngine;

public abstract class VariableBase<T> : ScriptableGuidObject
{

    public T value;


}

public class ScriptableGuidObject : ScriptableObject
{
    [HideInInspector]
    public string guid;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(guid))
            guid = System.Guid.NewGuid().ToString();
    }
}