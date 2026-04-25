using System.Collections.Generic;
using UnityEngine;

public class TankBulletManager : MonoBehaviour
{
    //弾システムを作る

    //do:総弾システム　リロード 召喚部分　オブジェクトプール　ダメージ処理
    [Tooltip("TankStatus")]
    [SerializeField] private TankStatus _tankStatus;

    [Tooltip("Setting")]
    [SerializeField] private GameObject bullet;

    [Tooltip("PoolSize")]
    [SerializeField] private int _poolSize = 5;

    private Queue<Bullet> _pool = new Queue<Bullet>();

    void Awake()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            var obj = Instantiate(bullet, transform.position, Quaternion.identity);
            obj.SetActive(false);
            _pool.Enqueue(obj.GetComponent<Bullet>());
        }
    }

    public void TakeDamage(int damage)
    {
        _tankStatus.dealDamage(damage);
        //do:ダメージ演出や効果音はここから呼び出せる。
    }

    void Update()
    {
        //リロード処理
    }

    /// <summary>
    /// 弾の召喚
    /// </summary>
    /// <param name="direction">発射方向</param>
    public void spawnBullet(Vector2 direction)
    {
        Bullet b;
        if (_pool.Count > 0)
        {
            b = _pool.Dequeue();
        }
        else
        {
            var obj = Instantiate(bullet, transform.position, Quaternion.identity);
            b = obj.GetComponent<Bullet>();
        }

        b.transform.position = transform.position;
        b.gameObject.SetActive(true);
        b.Initialize(direction, this);
    }

    /// <summary>
    /// 弾をプールに返却する
    /// </summary>
    public void ReturnBullet(Bullet b)
    {
        b.gameObject.SetActive(false);
        _pool.Enqueue(b);
    }
}