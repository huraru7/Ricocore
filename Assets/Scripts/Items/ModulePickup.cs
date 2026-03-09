using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ModulePickup : MonoBehaviour
{
    // TODO: 将来的にモジュールの種類・効果を定義する
    // [SerializeField] private ModuleDefinition moduleData;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerStats>(out _)) return;

        // TODO: playerStats.SetModuleBonus(moduleData.bonus); などでモジュールを適用する

        Destroy(gameObject);
    }
}
