using UnityEngine;

[CreateAssetMenu(menuName = "Data/Create ModuleData")]
public class ModuleData : ScriptableObject
{
    [Tooltip("モジュールの属性")] public ModuleElementEnum moduleElement;
}

public enum ModuleElementEnum
{
    None,
    Earth,
    Water,
    Fire,
    Wind
}