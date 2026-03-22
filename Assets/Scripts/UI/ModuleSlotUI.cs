using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// モジュールスロット1つ分の UI コンポーネント。
/// インベントリスロット・部位スロットの両方に使用する統一クラス。
///
/// 機能:
///   - アイコン表示（モジュールあり／なしで切り替え）
///   - ホバー時に ModuleInfoPanel へモジュール情報を送信
///   - クリック時に外部から注入されたコールバックを呼ぶ
///   - ModuleSlot.OnChanged を購読し、変化時に自動で表示を更新
/// </summary>
public class ModuleSlotUI : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("UI 部品")]
    [SerializeField] private Image           iconImage;
    [SerializeField] private TextMeshProUGUI labelText;       // スロット名や番号（任意）
    [SerializeField] private GameObject      hoverHighlight;  // ホバー時に表示するオーバーレイ（任意）
    [SerializeField] private Image           borderImage;     // スロット枠（色分け用, 任意）

    [Header("アセット")]
    [SerializeField] private Sprite emptySprite;

    // ---- ランタイム状態 ----
    private ModuleSlot      slot;
    private ModuleInfoPanel infoPanel;
    private System.Action<ModuleSlot> onClickCallback;
    private string actionButtonLabel = "装着";

    // -------------------------------------------------------

    /// <summary>
    /// 初期化。設定をまとめた <see cref="ModuleSlotUIConfig"/> を1つ受け取る。
    /// インベントリスロット・部位スロットどちらにも使用できる。
    /// </summary>
    public void Initialize(ModuleSlotUIConfig config)
    {
        this.slot              = config.Slot;
        this.infoPanel         = config.InfoPanel;
        this.onClickCallback   = config.OnClick;
        this.actionButtonLabel = config.ActionButtonLabel;

        if (labelText != null)
            labelText.text = config.Label;

        if (hoverHighlight != null)
            hoverHighlight.SetActive(false);

        if (borderImage != null && config.SlotColor != Color.clear)
            borderImage.color = config.SlotColor;

        // スロットの変化を購読して自動更新
        slot.OnChanged += OnSlotChanged;
        Refresh();
    }

    void OnDestroy()
    {
        if (slot != null)
            slot.OnChanged -= OnSlotChanged;
    }

    // -------------------------------------------------------
    // IPointerEnterHandler / IPointerExitHandler / IPointerClickHandler

    public void OnPointerEnter(PointerEventData _)
    {
        if (hoverHighlight != null)
            hoverHighlight.SetActive(true);

        if (slot.HasModule)
            infoPanel.Show(slot.Module, () => onClickCallback?.Invoke(slot), actionButtonLabel);
    }

    public void OnPointerExit(PointerEventData _)
    {
        if (hoverHighlight != null)
            hoverHighlight.SetActive(false);

        infoPanel.Hide();
    }

    public void OnPointerClick(PointerEventData _)
    {
        onClickCallback?.Invoke(slot);
    }

    // -------------------------------------------------------
    // 内部処理

    private void OnSlotChanged(ModuleSlot _) => Refresh();

    private void Refresh()
    {
        if (iconImage == null) return;

        iconImage.sprite = slot.HasModule && slot.Module.Icon != null
            ? slot.Module.Icon
            : emptySprite;
    }
}
