using R3;
using UnityEngine;

public class TankPresenter : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] private TankStatus tankStatus;
    [SerializeField] private TankModuleManager tankModuleManager;

    [Header("View")]
    [SerializeField] private GetModuleSelectUI getModuleSelectUI;
    [SerializeField] private SlotUI slotUI;

    void Start()
    {
        // Model→View: 3択候補が生成されたらUIに表示
        tankModuleManager.OnModuleCandidatesGenerated
            .Subscribe(candidates => getModuleSelectUI.ShowOptions(candidates))
            .AddTo(this);

        // View→Model: 選択されたモジュールを空きスロットへ自動装備
        getModuleSelectUI.OnModuleSelected
            .Subscribe(selected => tankModuleManager.TryAutoEquip(selected))
            .AddTo(this);

        // 初期表示
        slotUI.UpdateDisplay(tankModuleManager.Slots);

        // Model→View: スロットが変化したら一覧を更新
        tankModuleManager.OnSlotsChanged
            .Subscribe(slots => slotUI.UpdateDisplay(slots))
            .AddTo(this);

        tankModuleManager.ModuleEarn(); // デバッグ用
    }
}
