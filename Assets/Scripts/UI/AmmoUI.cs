using TMPro;
using UnityEngine;

/// <summary>
/// 弾数を表示する HUD。
/// PlayerStats は PlayerSystemHub.Instance から自動取得するため SerializeField 設定不要。
/// </summary>
public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;

    private int lastAmmo    = -1;
    private int lastMaxAmmo = -1;

    void Update()
    {
        if (PlayerSystemHub.Instance == null) return;
        var ps = PlayerSystemHub.Instance.PlayerStats;

        int current = ps.CurrentAmmo;
        int max     = ps.MaxAmmo;
        if (current == lastAmmo && max == lastMaxAmmo) return;
        lastAmmo    = current;
        lastMaxAmmo = max;
        ammoText.text = $"{current}/{max}";
    }
}