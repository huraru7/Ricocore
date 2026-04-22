using R3;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// レベルアップ時に 3 択のモジュール選択画面を表示するコントローラ（UI Toolkit 版）。
///
/// UXML: Assets/UI/UXML/LevelUpPanel.uxml
/// USS:  Assets/UI/USS/Common.uss  (.levelup-panel / .levelup-panel__* クラス)
///
/// PlayerState.Level を Subscribe し、レベルアップ時にパネルを表示する。
/// LitMotion でパネル出現アニメを再生（Time.timeScale = 0 に対応）。
///
/// フロー:
///   1. PlayerState.Level が更新される（ExperienceSystem が書き込む）
///   2. .Skip(1) で初期値（Lv.1）をスキップ
///   3. ModuleDatabase.GetRandom(3) でランダムな選択肢を取得
///   4. VisualTreeAsset からカードを 3 枚複製してコンテナに追加
///   5. Time.timeScale = 0、UIToolkitAnimations でポップイン
///   6. プレイヤーがカードを選択
///   7. TankModuleManager.AcquireModule() でインベントリに追加
///   8. パネルを非表示にして Time.timeScale = 1
///
/// uGUI 版との主な違い:
///   [SerializeField] GameObject root     → UIDocument + Q("levelup-root")
///   [SerializeField] ModuleRewardCardUI[] cards → VisualTreeAsset を CloneTree() で動的生成
///   root.SetActive()                     → panel.style.display
///   UIAnimations.PanelOpen(root.transform) → UIToolkitAnimations.PanelOpen(panel)
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class LevelUpUI : MonoBehaviour
{
    [Header("カードのテンプレート（ModuleRewardCard.uxml を設定）")]
    [SerializeField] private VisualTreeAsset cardTemplate;

    // ---- VisualElement 参照 ----
    private VisualElement panel;
    private Label         levelText;
    private VisualElement cardContainer;

    // 生成したカードコントローラを再利用するためのキャッシュ
    private readonly ModuleRewardCardUI[] cards = new ModuleRewardCardUI[3];

    // -------------------------------------------------------

    void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        panel         = root.Q<VisualElement>("levelup-root");
        levelText     = root.Q<Label>("level-text");
        cardContainer = root.Q<VisualElement>("card-container");

        // カードを事前に 3 枚生成してコンテナに追加しておく
        for (int i = 0; i < cards.Length; i++)
        {
            var instance = cardTemplate.CloneTree();
            cardContainer.Add(instance);
            cards[i] = new ModuleRewardCardUI(instance);
        }

        Hide();
    }

    void Start()
    {
        PlayerSystemHub.Instance.PlayerState.Level
            .Skip(1)  // 初期値（Lv.1）をスキップ。変化した時だけ反応する
            .Subscribe(ShowLevelUp)
            .AddTo(this);
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
            cards[i].Root.style.display = DisplayStyle.Flex;
            cards[i].Initialize(choices[i], OnCardSelected);
        }
        // 選択肢が 3 未満の場合は余ったカードを非表示
        for (int i = n; i < cards.Length; i++)
            cards[i].Root.style.display = DisplayStyle.None;

        panel.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;

        // timeScale = 0 のままでも動作するパネル出現アニメ
        UIToolkitAnimations.PanelOpen(panel);
    }

    private void OnCardSelected(ModuleDefinition def)
    {
        PlayerSystemHub.Instance.ModuleManager.AcquireModule(def);
        Hide();
        Time.timeScale = 1f;
    }

    private void Hide() => panel.style.display = DisplayStyle.None;
}
