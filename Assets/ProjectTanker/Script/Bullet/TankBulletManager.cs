using UnityEngine;

public class TankBulletManager : MonoBehaviour
{
    //弾システムを作る

    //do:総弾システム　リロード 召喚部分　オブジェクトプール　ダメージ処理
    [SerializeField] GameObject bullet;
    [SerializeField] TankStatus _tankStatus;

    public void TakeDamage(int damage)
    {
        _tankStatus.dealDamage(damage);

        //do:ダメージ演出や効果音はここから呼び出せる。
    }
}
