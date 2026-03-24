using TMPro;
using UnityEngine;

/// <summary>
/// レベルアップ時に 3 択のモジュール選択画面を表示するコントローラ。
/// Canvas 直下にアタッチすること（常にアクティブな親に置く必要がある）。
///
/// フロー:
///   1. ExperienceSystem.OnLevelUp 発火
///   2. ModuleDatabase.GetRandom(3) でランダムな選択肢を取得
///   3. 3 枚のカードに情報をセットして root パネルを表示
///   4. Time.timeScale = 0（ゲームを一時停止）
///   5. プレイヤーがカードを選択
///   6. TankModuleManager.AcquireModule() でインベントリに追加
///   7. パネルを非表示にして Time.timeScale = 1（ゲーム再開）
///
/// シーン構成:
///   Canvas（LevelUpUI をアタッチ）
///   └── LevelUpPanel  ← root にドラッグ
///       ├── LevelText (TextMeshProUGUI) ← levelText にドラッグ
///       └── CardContainer
///           ├── Card_0 (ModuleRewardCardUI) ← cards[0]
///           ├── Card_1 (ModuleRewardCardUI) ← cards[1]
///           └── Card_2 (ModuleRewardCardUI) ← cards[2]
/// </summary>
public class LevelUpUI : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private ExperienceSystem   expSystem;
    [SerializeField] private TankModuleManager  moduleManager;
    [SerializeField] private ModuleDatabase     database;

    [Header("UI")]
    [SerializeField] private GameObject          root;      // 表示/非表示するパネル
    [SerializeField] private TextMeshProUGUI     levelText; // "Level Up!  Lv.X" 表示
    [SerializeField] private ModuleRewardCardUI[] cards;    // 3 枚のカード

    // -------------------------------------------------------

    void Awake()
    {
        root.SetActive(false);

        Debug.Assert(expSystem    != null, "[LevelUpUI] ExperienceSystem が未設定です");
        Debug.Assert(moduleManager != null, "[LevelUpUI] TankModuleManager が未設定です");
        Debug.Assert(database     != null, "[LevelUpUI] ModuleDatabase が未設定です");

        expSystem.OnLevelUp += OnLevelUp;
    }

    void OnDestroy()
    {
        if (expSystem != null)
            expSystem.OnLevelUp -= OnLevelUp;
    }

    // -------------------------------------------------------

    private void OnLevelUp(int newLevel)
    {
        if (database.modules == null || database.modules.Length == 0)
        {
            Debug.LogWarning("[LevelUpUI] ModuleDatabase にモジュールが登録されていません。");
            return;
        }

        var choices = database.GetRandom(3);

        if (levelText != null)
            levelText.text = $"Level Up!  Lv.{newLevel}";

        // カードをセットアップ（choices が 3 未満の場合は余りカードを非表示）
        int n = Mathf.Min(cards.Length, choices.Length);
        for (int i = 0; i < n; i++)
        {
            cards[i].gameObject.SetActive(true);
            cards[i].Initialize(choices[i], OnCardSelected);
        }
        for (int i = n; i < cards.Length; i++)
            cards[i].gameObject.SetActive(false);

        root.SetActive(true);
        Time.timeScale = 0f; // ゲーム一時停止
    }

    private void OnCardSelected(ModuleDefinition def)
    {
        moduleManager.AcquireModule(def);
        root.SetActive(false);
        Time.timeScale = 1f; // ゲーム再開
    }
}
