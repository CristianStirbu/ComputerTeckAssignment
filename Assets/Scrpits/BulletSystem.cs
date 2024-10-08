using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using UnityEngine;

public partial struct BulletSystem : ISystem
{
    private void OnUpdate(ref SystemState state)
    {
        EntityManager entityManager = state.EntityManager;
        NativeArray<Entity> allEntities = entityManager.GetAllEntities();

        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        foreach(Entity entity in allEntities)
        {
            if(entityManager.HasComponent<BulletComponent>(entity))
            {
                LocalTransform bulletTransform = entityManager.GetComponentData<LocalTransform>(entity);
                BulletComponent bulletComponent = entityManager.GetComponentData<BulletComponent>(entity);
                bulletTransform.Position += bulletComponent.speed * SystemAPI.Time.DeltaTime * bulletTransform.Right();
                entityManager.SetComponentData(entity, bulletTransform);

                NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);
                physicsWorld.SphereCastAll(bulletTransform.Position, bulletComponent.Size / 2, float3.zero, 1,
                    ref hits, new CollisionFilter { BelongsTo = (uint)CollisionLayer.Default, CollidesWith = (uint)CollisionLayer.Enemy });

                foreach(ColliderCastHit hit in hits)
                {
                    entityManager.SetEnabled(hit.Entity, false);
                    entityManager.DestroyEntity(entity);
                }

                hits.Dispose();
            }
        }
    }
}


public enum CollisionLayer
{
    Default = 1 << 0,
    Enemy = 1 << 6,
    Bullet = 1 << 6
}


