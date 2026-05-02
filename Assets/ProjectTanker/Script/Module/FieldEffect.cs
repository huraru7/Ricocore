using System.Xml.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "Module", menuName = "Data/Create FieldEffect")]
public abstract class FieldEffect : ScriptableObject
{
    public float duration; //:持続時間
    public Vector2 position; //:設置位置
    public abstract void Place(Vector2 _position, float _duration);
}