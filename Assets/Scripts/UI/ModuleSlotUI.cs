using R3;
using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ModuleSlotUI に渡す設定をまとめたクラス。変更なし。
/// </summary>
public class ModuleSlotUIConfig
{
    /// <summary>紐づけるモジュールスロット（必須）</summary>
    public ModuleSlot Slot;

    /// <summary>ホバー時に情報を表示するパネル（null でも動作する）</summary>
    public ModuleInfoPanel InfoPanel;

    /// <summary>スロットをクリックしたときの処理</summary>
    public Action<ModuleSlot> OnClick;

    /// <summary>スロットのラベル文字列（省略可）</summary>
    public string Label = "";

    /// <summary>ホバー時に情報パネルに表示するボタンのラベル（"装着" or "取り外し"）</summary>
    public string ActionButtonLabel = "装着";

    /// <summary>スロット枠の色。Color.clear を指定すると変更しない</summary>
    public Color SlotColor = Color.clear;
}

// -------------------------------------------------------

/// <summary>
/// モジュールスロット 1 つ分のコントローラ（UI Toolkit 版）。
/// インベントリスロット・部位スロットの両方に使用する統一クラス。
///
/// UXML: Assets/UI/UXML/ModuleSlot.uxml
/// USS:  Assets/UI/USS/Common.uss  (.slot / .slot__* クラス)
///
/// uGUI 版との主な違い:
///   MonoBehaviour → IDisposable な plain class
///   IPointerEnterHandler → RegisterCallback＜PointerEnterEvent＞
///   hoverHighlight.SetActive → AddToClassList("slot--hovered")
///   UIAnimations.Pop(transform) → UIToolkitAnimations.Pop(VisualElement)
///   AddTo(this) → AddTo(CompositeDisposable)、DetachFromPanel 時に自動 Dispose
///   emptySprite → backgroundImage = StyleKeyword.None（スプライト不要）
/// </summary>
public class ModuleSlotUI : IDisposable
{
    /// <summary>InventoryUI / ModuleMenuUI がコンテナに追加する対象</summary>
    public readonly VisualElement Root;

    private readonly VisualElement iconElement;
    private readonly Label         labelElement;

    private ModuleSlot              slot;
    private ModuleInfoPanel         infoPanel;
    private Action<ModuleSlot>      onClickCallback;
    private string                  actionButtonLabel = "装着";
    private bool                    hadModule;

    private readonly CompositeDisposable disposables = new CompositeDisposable();

    // -------------------------------------------------------

    /// <summary>
    /// CloneTree() で生成した VisualElement を受け取り、子要素を取得する。
    /// </summary>
    public ModuleSlotUI(VisualElement root)
    {
        Root         = root;
        iconElement  = root.Q<VisualElement>("slot-icon");
        labelElement = root.Q<Label>("slot-label");

        // VisualElement がパネルから切り離されたとき自動で Dispose（購読解除）
        root.RegisterCallback<DetachFromPanelEvent>(_ => Dispose());
    }

    // -------------------------------------------------------

    /// <summary>
    /// スロットの情報と動作をセットする。
    /// InventoryUI / ModuleMenuUI から呼ばれる。
    /// </summary>
    public void Initialize(ModuleSlotUIConfig config)
    {
        slot              = config.Slot;
        infoPanel         = config.InfoPanel;
        onClickCallback   = config.OnClick;
        actionButtonLabel = config.ActionButtonLabel;

        if (labelElement != null)
            labelElement.text = config.Label;

        // 部位スロットの色分け（Color.clear = 変更なし）
        // UI Toolkit は borderColor 一括指定がないため4辺を個別に設定する
        if (config.SlotColor != Color.clear)
        {
            var c = new StyleColor(config.SlotColor);
            Root.style.borderTopColor    = c;
            Root.style.borderBottomColor = c;
            Root.style.borderLeftColor   = c;
            Root.style.borderRightColor  = c;
        }

        // ポインターイベント登録
        Root.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
        Root.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
        Root.RegisterCallback<ClickEvent>(OnClick);

        // R3 購読（Dispose() 時に自動解除）
        slot.Changed
            .Subscribe(_ => OnSlotChanged())
            .AddTo(disposables);

        Refresh();
    }

    // -------------------------------------------------------

    public void Dispose()
    {
        disposables.Dispose();
        Root.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
        Root.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
        Root.UnregisterCallback<ClickEvent>(OnClick);
    }

    // -------------------------------------------------------
    // ポインターイベント

    private void OnPointerEnter(PointerEnterEvent _)
    {
        Root.AddToClassList("slot--hovered");

        if (slot.HasModule && infoPanel != null)
            infoPanel.Show(slot.Module, () => onClickCallback?.Invoke(slot), actionButtonLabel);
    }

    private void OnPointerLeave(PointerLeaveEvent _)
    {
        Root.RemoveFromClassList("slot--hovered");
        infoPanel?.Hide();
    }

    private void OnClick(ClickEvent _)
    {
        onClickCallback?.Invoke(slot);
    }

    // -------------------------------------------------------
    // 内部処理

    private void OnSlotChanged()
    {
        bool wasHadModule = hadModule;
        Refresh();
        // モジュールが新たに追加された時だけポップアニメ
        if (hadModule && !wasHadModule && iconElement != null)
            UIToolkitAnimations.Pop(iconElement);
    }

    private void Refresh()
    {
        hadModule = slot.HasModule;
        if (iconElement == null) return;

        if (slot.HasModule && slot.Module.Icon != null)
            iconElement.style.backgroundImage = new StyleBackground(slot.Module.Icon);
        else
            iconElement.style.backgroundImage = StyleKeyword.None;
    }
}
