using System.Collections;
using UnityEngine;

public class TankStatus : MonoBehaviour
{
    [SerializeField] private TankData data;

    public int HP { get; private set; }
    public int movementSpeed { get; private set; }
    public int turnRate { get; private set; }

    void Start()
    {
        HP = data.maxHP;
        movementSpeed = data.movementSpeed;
        turnRate = data.turnRate;
    }
}