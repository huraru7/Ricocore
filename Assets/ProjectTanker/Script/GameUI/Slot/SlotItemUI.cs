using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ProjectTanker.UI;

public class SlotItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image accentLine;
    [SerializeField] private TextMeshProUGUI slotNumber;
    [SerializeField] private ThemeColor theme;

    public ModuleData Data { get; private set; }

    public void Setup(ModuleData data, int slotIndex)
    {
        Data = data;

        if (slotNumber != null)
            slotNumber.text = (slotIndex + 1).ToString();

        bool isEmpty = data == null;
        iconImage.sprite = isEmpty ? null : data.icon;
        nameText.text = isEmpty ? "" : data.moduleName;

        if (accentLine != null && theme != null)
            accentLine.color = isEmpty ? theme.border : GetElementColor(data.moduleElement);

        var c = iconImage.color;
        iconImage.color = new Color(c.r, c.g, c.b, isEmpty ? 0f : 1f);
    }

    private Color GetElementColor(ModuleElementEnum element) => element switch
    {
        ModuleElementEnum.Earth => theme.earth,
        ModuleElementEnum.Water => theme.water,
        ModuleElementEnum.Fire  => theme.fire,
        ModuleElementEnum.Wind  => theme.wind,
        _                       => theme.none,
    };
}
