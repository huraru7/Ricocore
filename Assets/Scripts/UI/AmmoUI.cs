using LitMotion;
using R3;
using TMPro;
using UnityEngine;

/// <summary>
/// 弾数を表示する HUD。
///
/// PlayerStats は PlayerSystemHub.Instance.PlayerState から Subscribe するため
/// SerializeField 設定不要・Update ポーリングなし。
/// 弾数変化時に LitMotion で数値カウントアニメーションを再生する。
/// </summary>
public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;

    private MotionHandle _motion;
    private int _displayedAmmo = -1;

    void Start()
    {
        var state = PlayerSystemHub.Instance.PlayerState;

        // 弾数変化 → アニメーション付きで更新
        state.CurrentAmmo
            .Subscribe(current => AnimateAmmo(current, state.MaxAmmo.Value))
            .AddTo(this);

        // 最大弾数変化（モジュール装備で変わる）→ テキストを即時更新
        state.MaxAmmo
            .Subscribe(max => ammoText.text = $"{state.CurrentAmmo.Value}/{max}")
            .AddTo(this);
    }

    private void AnimateAmmo(int target, int max)
    {
        if (_motion.IsActive()) _motion.Cancel();

        int from = _displayedAmmo < 0 ? target : _displayedAmmo;
        _displayedAmmo = target;

        _motion = UIAnimations.NumberCount(
            from, target, 0.25f,
            x => ammoText.text = $"{Mathf.RoundToInt(x)}/{max}");
    }
}
