/// <summary>
/// 戦車の装着部位を表す列挙型。
/// 将来の部位追加はここに値を追加するだけでよい。
/// </summary>
public enum TankSlot
{
    None             = 0,
    Turret           = 1,  // 砲塔
    Engine           = 2,  // エンジン
    RightCaterpillar = 3,  // 右キャタピラー
    LeftCaterpillar  = 4,  // 左キャタピラー
}
