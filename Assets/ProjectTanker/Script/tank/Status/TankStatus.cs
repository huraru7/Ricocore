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
    public SerializableReactiveProperty<int> getHP => HP;
    public SerializableReactiveProperty<int> getMaxHP => maxHP;
    public SerializableReactiveProperty<int> getMagazineCapacity => magazineCapacity;
    public SerializableReactiveProperty<int> getMovementSpeed => movementSpeed;
    public SerializableReactiveProperty<int> getTurnRate => turnRate;
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

    /// <summary>
    /// ステータスのリセット処理(モジュール再計算時に呼び出す)
    /// </summary>
    public void ResetStatus()
    {
        HP.Value = data.maxHP;
        maxHP.Value = data.maxHP;
        movementSpeed.Value = data.movementSpeed;
        turnRate.Value = data.turnRate;
        magazineCapacity.Value = data.magazineCapacity;
    }

    public void DealDamage(int amount)
    {
        if (amount <= 0) return;
        HP.Value = Mathf.Clamp(HP.Value - amount, 0, maxHP.Value);
    }
}