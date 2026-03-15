using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private TextMeshProUGUI ammoText;

    private int lastAmmo    = -1;
    private int lastMaxAmmo = -1;

    void Update()
    {
        int current = playerStats.CurrentAmmo;
        int max     = playerStats.MaxAmmo;
        if (current == lastAmmo && max == lastMaxAmmo) return;
        lastAmmo    = current;
        lastMaxAmmo = max;
        ammoText.text = $"{current}/{max}";
    }
}