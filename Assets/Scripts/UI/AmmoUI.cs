using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private TextMeshProUGUI ammoText;

    void Update()
    {
        ammoText.text = $"{playerStats.CurrentAmmo}/{playerStats.MaxAmmo}";
    }
}