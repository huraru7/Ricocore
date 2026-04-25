using R3;
using Unity.VisualScripting;
using UnityEngine;

public class TankStatus : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private TankData data;

    [Header("Status")]
    public ReactiveProperty<int> HP { get; private set; }
    public int maxHP;
    public ReactiveProperty<int> movementSpeed { get; private set; }
    public ReactiveProperty<int> turnRate { get; private set; }
    public ReactiveProperty<int> magazineCapacity { get; private set; }

    void Awake()
    {
        if (data == null)
        {
            Debug.LogError($"TankDataが割り当てられていません。", this);
            return;
        }

        HP = new ReactiveProperty<int>(data.maxHP);
        maxHP = data.maxHP;
        movementSpeed = new ReactiveProperty<int>(data.movementSpeed);
        turnRate = new ReactiveProperty<int>(data.turnRate);
        magazineCapacity = new ReactiveProperty<int>(data.magazineCapacity);
    }

    public void dealDamage(int amount)
    {
        if (amount <= 0) return;
        HP.Value = Mathf.Clamp(HP.Value - amount, 0, data.maxHP);
    }
}