using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct PlayerComponent : IComponentData
{
    public float MoveSpeed;
    public float ShootCooldown;
    public Entity BulletPrefab;
}
    

