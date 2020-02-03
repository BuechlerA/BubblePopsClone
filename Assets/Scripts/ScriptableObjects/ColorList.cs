
using UnityEngine;

[CreateAssetMenu(fileName = "Color List", menuName = "ScriptableObjects/ColorListScriptableObject", order = 1)]
public class ColorList : ScriptableObject
{
    public Color[] Colors = new Color[11];
}
