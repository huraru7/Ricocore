using UnityEngine;

/// <summary>
/// モジュール獲得フローを担うコンポーネント。
/// Player GameObject にアタッチする。
///
/// 責務: AcquireModule のみ（拾った時に Module を生成して EquipSystem へ渡す）
/// 装備ロジック → EquipSystem
/// ステータス計算 → StatsSystem
/// </summary>
[RequireComponent(typeof(EquipSystem))]
[RequireComponent(typeof(PlayerStats))]
public class TankModuleManager : MonoBehaviour
{
    private EquipSystem equipSystem;

    void Awake()
    {
        equipSystem = GetComponent<EquipSystem>();
    }

    /// <summary>
    /// プレイヤーがモジュールを新規獲得する。
    /// ModuleDefinition から Module インスタンスを生成し、空きインベントリスロットへ格納する。
    /// インベントリが満杯なら false を返す。
    /// </summary>
    public bool AcquireModule(ModuleDefinition definition)
    {
        if (definition == null) return false;
        return equipSystem.PlaceInInventory(new Module(definition));
    }
}
