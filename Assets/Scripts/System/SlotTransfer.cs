/// <summary>
/// スロット間のモジュール移動を担う静的ユーティリティ。
///
/// Inventory → Part / Part → Inventory / Part → Part
/// すべて同じメソッドで実現できる。
/// </summary>
public static class SlotTransfer
{
    /// <summary>
    /// from のモジュールを to へ移動する。
    /// to に既存モジュールがある場合は from へ押し出す（自動入れ替え）。
    /// </summary>
    public static void Move(ModuleSlot from, ModuleSlot to)
    {
        var moduleToMove = from.Remove();

        if (to.HasModule)
        {
            var displaced = to.Remove();
            from.Set(displaced); // 押し出されたモジュールを元の from スロットへ返却
        }

        to.Set(moduleToMove);
    }

    /// <summary>
    /// to が空の場合のみ from → to へ移動する。
    /// to にモジュールがある場合は何もせず false を返す。
    /// </summary>
    public static bool MoveIfEmpty(ModuleSlot from, ModuleSlot to)
    {
        if (to.HasModule) return false;
        to.Set(from.Remove());
        return true;
    }
}
