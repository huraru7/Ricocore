using UnityEngine;

//:敵に対して効果を与えるための規定クラス
[CreateAssetMenu(fileName = "Module", menuName = "Data/Create StatusEffect")]
public abstract class StatusEffect : ScriptableObject
{
    public float duration; // 効果の持続時間
    public abstract void ApplyTo(TankStatus _status);
}