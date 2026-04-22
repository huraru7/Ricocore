using R3;
using System;

/// <summary>
/// プレイヤーの状態をリアクティブに公開するステートクラス。
///
/// 設計:
///   - PlayerStats / ExperienceSystem が値を書き込む（GameLogic 側）
///   - UI スクリプトが Subscribe して値の変化を受け取る（UI 側）
///   - 直接参照ではなくストリーム経由にすることで UI とゲームロジックを分離する
/// </summary>
public class PlayerState : IDisposable
{
    // --- HP ---
    public ReactiveProperty<int> CurrentHp = new(0);
    public ReactiveProperty<int> MaxHp     = new(0);

    // --- 弾薬 ---
    public ReactiveProperty<int> CurrentAmmo = new(0);
    public ReactiveProperty<int> MaxAmmo     = new(0);

    // --- 経験値 / レベル ---
    public ReactiveProperty<int> Level    = new(1);
    public ReactiveProperty<int> Exp      = new(0);
    public ReactiveProperty<int> ExpToNext = new(100);

    public void Dispose()
    {
        CurrentHp.Dispose();
        MaxHp.Dispose();
        CurrentAmmo.Dispose();
        MaxAmmo.Dispose();
        Level.Dispose();
        Exp.Dispose();
        ExpToNext.Dispose();
    }
}
