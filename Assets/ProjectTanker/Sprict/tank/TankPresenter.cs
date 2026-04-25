using R3;
using UnityEngine;

public class TankPresenter : MonoBehaviour
{
    [SerializeField] private TankStatus tankStatus;

    void Start()
    {
        // tankStatus.HP.Subscribe(hp => view.UpdateHP(hp, tankStatus.maxHP)).AddTo(this);
    }
}
