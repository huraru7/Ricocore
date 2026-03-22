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
    /// 初期化。スロット・情報パネル・クリックコールバック・ラベル文字列を受け取る。
    /// label は省略可能（インベントリスロットには番号、部位スロットには「砲塔」等を渡す）。
    /// actionButtonLabel: ホバー時に情報パネルに表示するボタンのラベル（"装着" or "取り外し"）
    /// slotColor: 境界線の色（Color.clear で変更なし）
    /// </summary>
    public void Initialize(
        ModuleSlot                slot,
        ModuleInfoPanel           infoPanel,
        System.Action<ModuleSlot> onClickCallback,
        string                    label             = "",
        string                    actionButtonLabel = "装着",
        Color                     slotColor         = default)
    {
        this.slot              = slot;
        this.infoPanel         = infoPanel;
        this.onClickCallback   = onClickCallback;
        this.actionButtonLabel = actionButtonLabel;

        if (labelText != null)
            labelText.text = label;

        if (hoverHighlight != null)
            hoverHighlight.SetActive(false);

        if (borderImage != null && slotColor != default)
            borderImage.color = slotColor;

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
