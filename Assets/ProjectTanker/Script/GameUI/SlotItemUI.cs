using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotItemUI : MonoBehaviour
{
    //:装備スロット1つ分のUI
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject emptyIndicator; //:空スロット時に表示するオブジェクト

    public ModuleData Data { get; private set; } //:ドラッグドロップ実装時に参照する

    public void Setup(ModuleData data)
    {
        Data = data;
        bool isEmpty = data == null;
        emptyIndicator.SetActive(isEmpty);
        iconImage.gameObject.SetActive(!isEmpty);
        nameText.text = isEmpty ? "" : data.moduleName;
        if (!isEmpty) iconImage.sprite = data.icon;
    }
}
