using R3;
using UnityEngine;

public class TankPresenter : MonoBehaviour
{
    [SerializeField] private TankStatus tankStatus;

    void Start()
    {
        //!現在未完成、実装予定。
        //UI部分の実装が完了次第、tankstatusをUIのシステムへ通知するようにします。

        // tankStatus.HP.Subscribe(hp => view.UpdateHP(hp, tankStatus.maxHP)).AddTo(this);
    }
}
