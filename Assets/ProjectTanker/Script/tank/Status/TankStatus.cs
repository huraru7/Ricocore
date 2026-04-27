using R3;
using UnityEngine;

public class TankStatus : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private TankData data;

    [Header("Status")]
    [SerializeField] private SerializableReactiveProperty<int> HP;
    [SerializeField] private SerializableReactiveProperty<int> maxHP;
    [SerializeField] private SerializableReactiveProperty<int> movementSpeed;
    [SerializeField] private SerializableReactiveProperty<int> turnRate;
    [SerializeField] private SerializableReactiveProperty<int> magazineCapacity;

    void Awake()
    {
        if (data == null)
        {
            Debug.LogError($"TankDataが割り当てられていません。", this);
            return;
        }

        HP = new(data.maxHP);
        maxHP = new(data.maxHP);
        movementSpeed = new(data.movementSpeed);
        turnRate = new(data.turnRate);
        magazineCapacity = new(data.magazineCapacity);
    }

    public void dealDamage(int amount)
    {
        if (amount <= 0) return;
        HP.Value = Mathf.Clamp(HP.Value - amount, 0, maxHP.Value);
    }
}