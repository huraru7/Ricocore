using System.Collections.Generic;
using UnityEngine;

public class SlotUI : MonoBehaviour
{
    [SerializeField] private SlotItemUI[] slotItems;

    public void UpdateDisplay(IReadOnlyList<ModuleData> slots)
    {
        for (int i = 0; i < slotItems.Length; i++)
        {
            ModuleData data = i < slots.Count ? slots[i] : null;
            slotItems[i].Setup(data, i);
        }
    }
}
