using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Module", menuName = "Data/Create ModuleData")]
public class ModuleData : ScriptableObject
{
    [Header("基本データ")]
    public string moduleName;
    public string description;
    public Sprite icon;
    [Tooltip("モジュールの属性")] public ModuleElementEnum moduleElement;

    [Header("特殊効果")]
    public List<SpecialEffect> specialEffects;
}

public enum ModuleElementEnum
{
    None,
    Earth,
    Water,
    Fire,
    Wind
}