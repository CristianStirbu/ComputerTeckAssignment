using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    public float Movespeed = 5;
    public float ShootCooldown = 1;
    public GameObject BulletPrefab;

    public class PlayerBaker :  Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity playerEntity = GetEntity(TransformUsageFlags.None);

            AddComponent(playerEntity, new PlayerComponent
            {
                MoveSpeed = authoring.Movespeed,
                ShootCooldown = authoring.ShootCooldown,
                BulletPrefab = GetEntity(authoring.BulletPrefab, TransformUsageFlags.None)
            });
        }
    }
}
