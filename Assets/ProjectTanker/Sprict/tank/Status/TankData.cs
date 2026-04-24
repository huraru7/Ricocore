using System.Collections;
using UnityEditor.EditorTools;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Create TankData")]
public class TankData : ScriptableObject
{
    [Tooltip("最大HP")] public int maxHP;
    [Tooltip("移動速度")] public int movementSpeed;
    [Tooltip("旋回速度")] public int turnRate;
}