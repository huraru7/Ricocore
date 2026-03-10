using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ModulePickup : MonoBehaviour
{
    // TODO: 将来的にモジュールの種類・効果を定義する
    // [SerializeField] private ModuleDefinition moduleData;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerStats>(out _)) return;

        // TODO: モジュールシステム実装後に moduleData.bonus を PlayerStats に適用する
        // 現時点では効果未実装のため Warning を出してアイテムを消す（拾得検出の確認用）
        Debug.LogWarning($"[ModulePickup] '{gameObject.name}' を拾いましたが、モジュール効果はまだ未実装です。");

        Destroy(gameObject);
    }
}
