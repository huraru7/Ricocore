using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private TMP_Text ammoText;

    void Update()
    {
        ammoText.text = $"{playerStats.CurrentAmmo}/{playerStats.MaxAmmo}";
    }
}
