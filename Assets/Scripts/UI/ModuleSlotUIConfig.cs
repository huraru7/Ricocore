using UnityEngine;

/// <summary>
/// ModuleSlotUI に渡す設定をまとめたクラス。
/// スロット UI を初期化する際はこの1つのオブジェクトを渡す。
///
/// 設計意図:
///   Initialize() に多数の引数を直接渡す代わりに、
///   この「設定の箱」を1つ渡すことで呼び出し元のコードをシンプルに保つ。
/// </summary>
public class ModuleSlotUIConfig
{
    /// <summary>紐づけるモジュールスロット（必須）</summary>
    public ModuleSlot Slot;

    /// <summary>ホバー時に情報を表示するパネル（必須）</summary>
    public ModuleInfoPanel InfoPanel;

    /// <summary>スロットをクリックしたときの処理</summary>
    public System.Action<ModuleSlot> OnClick;

    /// <summary>スロットのラベル文字列（省略可）</summary>
    public string Label = "";

    /// <summary>ホバー時に情報パネルに表示するボタンのラベル（"装着" or "取り外し"）</summary>
    public string ActionButtonLabel = "装着";

    /// <summary>スロット枠の色。Color.clear を指定すると変更しない</summary>
    public Color SlotColor = Color.clear;
}
