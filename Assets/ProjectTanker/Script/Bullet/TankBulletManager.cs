using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankBulletManager : MonoBehaviour
{
    //:弾システムを作る

    //do:総弾システム　リロード 召喚部分　オブジェクトプール　ダメージ処理
    [Header("TankStatus")]
    [SerializeField] private TankStatus _tankStatus;

    [Header("Setting")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private bool isShoot = true;
    [SerializeField] private SerializableReactiveProperty<int> totalRounds;
    public SerializableReactiveProperty<int> getTotalRounds => totalRounds;

    [Header("PoolSize")]
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

    void Start()
    {
        totalRounds.Value = _tankStatus.getMagazineCapacity.Value;
        Debug.Log($"初期弾数: {totalRounds.Value}");
    }

    public void TakeDamage(int damage)
    {
        _tankStatus.dealDamage(damage);
        //do:ダメージ演出や効果音はここから呼び出せる。
    }

    private float currentTime = 0f;

    void Update()
    {
        if (totalRounds.Value < _tankStatus.getMagazineCapacity.Value)
        {
            //リロード処理
            currentTime += Time.deltaTime;

            if (currentTime > 5)//!:この5は後々リロード時間のステータスに紐づける
            {
                totalRounds.Value++;
                currentTime = 0f;
                Debug.Log($"りろーど 弾残量{totalRounds.Value}");
            }
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isShoot && totalRounds.Value >= 1)
        {
            spawnBullet(transform.up);
            Debug.Log($"弾残量{totalRounds.Value}");
        }
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
        totalRounds.Value--;
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