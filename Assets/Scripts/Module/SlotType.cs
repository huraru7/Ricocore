using UnityEngine;

/// <summary>
/// スロットの種別を表す列挙型。
/// TankSlot を置き換える。数値は TankSlot と互換を保つ。
/// </summary>
public enum SlotType
{
    None              = 0,  // ALLフィルタ用
    Turret            = 1,  // 砲塔
    Engine            = 2,  // エンジン
    RightCaterpillar  = 3,  // 右キャタピラー
    LeftCaterpillar   = 4,  // 左キャタピラー
    // 後々追加予定
}

// -------------------------------------------------------

/// <summary>
/// SlotType に対応する UI カラー定義。
/// InventoryUI / ModuleMenuUI など複数箇所から参照する共通カラーパレット。
/// </summary>
public static class SlotTypeColor
{
    public static readonly Color Turret           = new Color(0.29f, 0.56f, 0.85f); // 青
    public static readonly Color Engine           = new Color(0.96f, 0.65f, 0.14f); // 黄
    public static readonly Color RightCaterpillar = new Color(0.82f, 0.01f, 0.11f); // 赤
    public static readonly Color LeftCaterpillar  = new Color(0.49f, 0.82f, 0.13f); // 緑

    /// <summary>SlotType に対応する色を返す。対応がない場合は白。</summary>
    public static Color Get(SlotType slot) => slot switch
    {
        SlotType.Turret           => Turret,
        SlotType.Engine           => Engine,
        SlotType.RightCaterpillar => RightCaterpillar,
        SlotType.LeftCaterpillar  => LeftCaterpillar,
        _                         => Color.white
    };
}
