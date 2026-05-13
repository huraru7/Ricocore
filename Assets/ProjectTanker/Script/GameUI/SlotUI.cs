using System.Collections.Generic;
using UnityEngine;

public class SlotUI : MonoBehaviour
{
    //:装備スロット一覧UI　Presenterからスロットデータを受け取って表示する。開閉はInventoryUIと同じパネル親で管理する。
    [SerializeField] private SlotItemUI[] slotItems; //:7つをInspectorで設定

    /// <summary>
    /// スロット一覧を更新する　Presenterから呼ばれる
    /// </summary>
    public void UpdateDisplay(IReadOnlyList<ModuleData> slots)
    {
        for (int i = 0; i < slotItems.Length; i++)
        {
            ModuleData data = i < slots.Count ? slots[i] : null;
            slotItems[i].Setup(data);
        }
    }
}
