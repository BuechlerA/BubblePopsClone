
using UnityEngine;

[CreateAssetMenu(fileName = "String List", menuName = "ScriptableObjects/StringListScriptableObject", order = 2)]
public class StringList : ScriptableObject
{
    public string[] messages = new string[10];
}
