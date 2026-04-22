using UnityEngine;

/// <summary>
/// フィールドに配置されたモジュールアイテム。
/// プレイヤーが触れると TankModuleManager.AcquireModule() を呼び、
/// ModuleDefinition (元データ) から新しい Module インスタンスを生成してインベントリへ追加する。
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ModulePickup : MonoBehaviour
{
    [SerializeField] private ModuleDefinition moduleData;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<TankModuleManager>(out var manager)) return;

        if (moduleData == null)
        {
            Debug.LogWarning($"[ModulePickup] '{gameObject.name}': ModuleDefinition が未設定です。");
            return;
        }

        if (manager.AcquireModule(moduleData))
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"[ModulePickup] インベントリが満杯のため '{moduleData.moduleName}' を拾えません。");
        }
    }
}
