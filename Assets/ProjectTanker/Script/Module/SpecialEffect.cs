using UnityEngine;

[CreateAssetMenu(fileName = "Module", menuName = "Data/Create SpecialEffect")]
public abstract class SpecialEffect : ScriptableObject
{
    public abstract void Apply(TankStatus _status, int stackCount);
    public abstract void Remove(TankStatus _status);
}