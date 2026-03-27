using R3;
using TMPro;
using UnityEngine;

/// <summary>
/// レベルアップ時に 3 択のモジュール選択画面を表示するコントローラ。
/// Canvas 直下にアタッチすること（常にアクティブな親に置く必要がある）。
///
/// PlayerState.Level を Subscribe し、レベルアップ時にパネルを表示する。
/// LitMotion でパネル出現アニメを再生（Time.timeScale = 0 に対応）。
///
/// フロー:
///   1. PlayerState.Level が更新される（ExperienceSystem が書き込む）
///   2. .Skip(1) で初期値（Lv.1）をスキップ
///   3. ModuleDatabase.GetRandom(3) でランダムな選択肢を取得
///   4. 3 枚のカードに情報をセットして root パネルを表示
///   5. Time.timeScale = 0（ゲームを一時停止）、LitMotion でポップイン
///   6. プレイヤーがカードを選択
///   7. TankModuleManager.AcquireModule() でインベントリに追加
///   8. パネルを非表示にして Time.timeScale = 1（ゲーム再開）
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
    [Header("UI")]
    [SerializeField] private GameObject           root;
    [SerializeField] private TextMeshProUGUI      levelText;
    [SerializeField] private ModuleRewardCardUI[] cards;

    // -------------------------------------------------------

    void Awake()
    {
        root.SetActive(false);
    }

    void Start()
    {
        PlayerSystemHub.Instance.PlayerState.Level
            .Skip(1)  // 初期値（Lv.1）をスキップ。変化した時だけ反応する
            .Subscribe(ShowLevelUp)
            .AddTo(this);
        // AddTo(this) で OnDestroy 時に自動解除 — 手動購読解除不要
    }

    // -------------------------------------------------------

    private void ShowLevelUp(int newLevel)
    {
        var database = PlayerSystemHub.Instance.ModuleDatabase;
        if (database == null || database.modules == null || database.modules.Length == 0)
        {
            Debug.LogWarning("[LevelUpUI] ModuleDatabase にモジュールが登録されていません。");
            return;
        }

        var choices = database.GetRandom(3);

        if (levelText != null)
            levelText.text = $"Level Up!  Lv.{newLevel}";

        int n = Mathf.Min(cards.Length, choices.Length);
        for (int i = 0; i < n; i++)
        {
            cards[i].gameObject.SetActive(true);
            cards[i].Initialize(choices[i], OnCardSelected);
        }
        for (int i = n; i < cards.Length; i++)
            cards[i].gameObject.SetActive(false);

        root.SetActive(true);
        Time.timeScale = 0f;

        // timeScale = 0 のままでも動作するパネル出現アニメ
        UIAnimations.PanelOpen(root.transform);
    }

    private void OnCardSelected(ModuleDefinition def)
    {
        PlayerSystemHub.Instance.ModuleManager.AcquireModule(def);
        root.SetActive(false);
        Time.timeScale = 1f;
    }
}
