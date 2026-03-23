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
