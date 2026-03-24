using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム内で入手可能な全モジュール定義を保持するデータベース。
/// レベルアップ報酬の抽選など、ランダムなモジュール選択に使用する。
///
/// 使い方:
///   Assets > Create > TankGame > Module > ModuleDatabase
///   作成したアセットの Modules 配列に ModuleDefinition アセットを登録する。
/// </summary>
[CreateAssetMenu(fileName = "ModuleDatabase", menuName = "TankGame/Module/ModuleDatabase")]
public class ModuleDatabase : ScriptableObject
{
    [Tooltip("ゲーム内で入手可能な全モジュール定義を登録する")]
    public ModuleDefinition[] modules;

    /// <summary>
    /// count 個のユニークなランダムモジュールを返す。
    /// modules の数が count 未満の場合は全件を返す。
    /// </summary>
    public ModuleDefinition[] GetRandom(int count)
    {
        if (modules == null || modules.Length == 0)
            return System.Array.Empty<ModuleDefinition>();

        count = Mathf.Min(count, modules.Length);

        // Fisher-Yates シャッフルで先頭 count 件を選ぶ
        var indices = new List<int>(modules.Length);
        for (int i = 0; i < modules.Length; i++) indices.Add(i);

        for (int i = 0; i < count; i++)
        {
            int j = Random.Range(i, indices.Count);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        var result = new ModuleDefinition[count];
        for (int i = 0; i < count; i++)
            result[i] = modules[indices[i]];

        return result;
    }
}
