using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの全モジュールスロットを管理するコンポーネント。
/// Player GameObject に PlayerStats と共にアタッチする。
///
/// 設計方針:
///   - ModuleDefinition (ScriptableObject) は元データ（テンプレート）として扱う
///   - モジュール獲得時に Definition から Module インスタンスを生成してスロットへ格納する
///   - インベントリスロット: 固定長配列 (InventorySlots)
///   - 部位スロット: 各 TankSlot に対応する Dictionary (PartSlots)
///   - スロットが変化すると ModuleSlot.OnChanged が発火し UI が自動更新される
/// </summary>
[RequireComponent(typeof(PlayerStats))]
public class TankModuleManager : MonoBehaviour
{
    [SerializeField, Tooltip("インベントリスロット数")]
    private int inventorySize = 10;

    // インベントリスロット（固定長配列）
    public ModuleSlot[] InventorySlots { get; private set; }

    // 部位スロット (TankSlot → ModuleSlot)
    public Dictionary<TankSlot, ModuleSlot> PartSlots { get; private set; }

    private PlayerStats playerStats;
    private StatBonus   lastBonus;

    /// <summary>モジュール装着/取り外しでボーナスが変化した際に発火する</summary>
    public event System.Action OnModuleChanged;

    /// <summary>現在装着中モジュールによるボーナス合計（TankStatsPanel等で参照）</summary>
    public StatBonus CurrentModuleBonus => lastBonus;

    // -------------------------------------------------------

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();

        // インベントリスロット初期化
        InventorySlots = new ModuleSlot[inventorySize];
        for (int i = 0; i < inventorySize; i++)
        {
            InventorySlots[i] = new ModuleSlot();
            InventorySlots[i].OnChanged += _ => RecalculateBonus();
        }

        // 部位スロット初期化（None 以外の全 TankSlot を登録）
        PartSlots = new Dictionary<TankSlot, ModuleSlot>();
        foreach (TankSlot slot in System.Enum.GetValues(typeof(TankSlot)))
        {
            if (slot == TankSlot.None) continue;
            PartSlots[slot] = new ModuleSlot();
            PartSlots[slot].OnChanged += _ => RecalculateBonus();
        }
    }

    // ============================================================
    // 公開 API
    // ============================================================

    /// <summary>
    /// プレイヤーがモジュールを新規獲得する。
    /// ModuleDefinition から Module インスタンスを生成し、空きインベントリスロットへ格納する。
    /// インベントリが満杯なら false を返す。
    /// </summary>
    public bool AcquireModule(ModuleDefinition definition)
    {
        if (definition == null) return false;

        var emptySlot = FindEmptyInventorySlot();
        if (emptySlot == null) return false;

        emptySlot.Place(new Module(definition));
        return true;
    }

    /// <summary>
    /// インベントリスロットのモジュールを指定部位スロットへ装着する。
    /// 部位スロットに既存モジュールがある場合、インベントリスロットと自動的に入れ替わる。
    /// </summary>
    public bool EquipToPart(ModuleSlot inventorySlot, TankSlot targetPart)
    {
        if (inventorySlot == null || inventorySlot.IsEmpty)  return false;
        if (!inventorySlot.Module.IsCompatible(targetPart))  return false;

        var partSlot      = PartSlots[targetPart];
        var moduleToEquip = inventorySlot.Take(); // インベントリスロットを解放

        // 部位スロットに既存モジュールがあれば、解放されたインベントリスロットへ移動
        if (partSlot.HasModule)
        {
            var displaced = partSlot.Take();
            CallUnequipEffects(displaced);
            inventorySlot.Place(displaced); // 元の位置へ返却
        }

        CallEquipEffects(moduleToEquip);
        partSlot.Place(moduleToEquip);
        return true;
    }

    /// <summary>
    /// 互換スロットの中から最適な部位スロットへ自動装着する。
    /// 空きスロットがあれば優先し、なければ最初の互換スロットへ入れ替える。
    /// </summary>
    public bool AutoEquip(ModuleSlot inventorySlot)
    {
        if (inventorySlot == null || inventorySlot.IsEmpty) return false;

        var compatSlots = inventorySlot.Module.CompatibleSlots;
        if (compatSlots == null || compatSlots.Length == 0) return false;

        // まず空きの互換スロットを探す
        foreach (var part in compatSlots)
        {
            if (PartSlots.TryGetValue(part, out var ps) && ps.IsEmpty)
                return EquipToPart(inventorySlot, part);
        }

        // 空きがなければ先頭の互換スロットへ入れ替え
        return EquipToPart(inventorySlot, compatSlots[0]);
    }

    /// <summary>
    /// 指定部位スロットのモジュールを外し、空きインベントリスロットへ戻す。
    /// インベントリが満杯なら false を返す。
    /// </summary>
    public bool UnequipFromPart(TankSlot part)
    {
        var partSlot = PartSlots[part];
        if (partSlot.IsEmpty) return false;

        var emptyInv = FindEmptyInventorySlot();
        if (emptyInv == null) return false;

        var module = partSlot.Take();
        CallUnequipEffects(module);
        emptyInv.Place(module);
        return true;
    }

    // ============================================================
    // 内部処理
    // ============================================================

    private ModuleSlot FindEmptyInventorySlot()
    {
        foreach (var slot in InventorySlots)
            if (slot.IsEmpty) return slot;
        return null;
    }

    private void RecalculateBonus()
    {
        var total = StatBonus.Zero;

        foreach (var kvp in PartSlots)
        {
            if (kvp.Value.IsEmpty) continue;
            var b = kvp.Value.Module.GetTotalStatBonus();
            total.moveSpeed    += b.moveSpeed;
            total.turnSpeed    += b.turnSpeed;
            total.fireCooldown += b.fireCooldown;
            total.bulletSpeed  += b.bulletSpeed;
            total.hp           += b.hp;
            total.maxAmmo      += b.maxAmmo;
        }

        lastBonus = total;
        playerStats.SetModuleBonus(total);
        OnModuleChanged?.Invoke();
    }

    private void CallEquipEffects(Module module)
    {
        if (module.Definition.specialEffects == null) return;
        foreach (var effect in module.Definition.specialEffects)
            effect?.OnEquip(this);
    }

    private void CallUnequipEffects(Module module)
    {
        if (module.Definition.specialEffects == null) return;
        foreach (var effect in module.Definition.specialEffects)
            effect?.OnUnequip(this);
    }
}
