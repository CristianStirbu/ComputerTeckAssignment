using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
using Unity.Physics;
using UnityEngine.Scripting.APIUpdating;

public partial struct PlayerSystem : ISystem
{
    private Entity playerEntity;
    private Entity inputEntity;
    private EntityManager entityManager;
    private PlayerComponent playerComponent;
    private InputComponent inputComponent;

    public void OnUpdate(ref SystemState state)
    {
        entityManager = state.EntityManager;
        playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        inputEntity = SystemAPI.GetSingletonEntity<InputComponent>();

        playerComponent = entityManager.GetComponentData<PlayerComponent>(playerEntity);
        inputComponent = entityManager.GetComponentData<InputComponent>(inputEntity);

        Move(ref state);
        Shoot(ref state);

        NativeArray<Entity> allEntities = entityManager.GetAllEntities();
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        foreach (Entity entity in allEntities)
        {
            if (entityManager.HasComponent<PlayerComponent>(entity))
            {
                LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(entity);
                PlayerComponent playerComponent = entityManager.GetComponentData<PlayerComponent>(entity);

                NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);
                physicsWorld.SphereCastAll(playerTransform.Position, playerComponent.Size / 1, float3.zero, 1,
                   ref hits, new CollisionFilter { BelongsTo = (uint)CollisionLayer.Default, CollidesWith = (uint)CollisionLayer.Enemy });

                entityManager.SetComponentData(entity, playerTransform);

                foreach (ColliderCastHit hit in hits)
                {
                    entityManager.DestroyEntity(entity);
                }

                hits.Dispose();

            }
        }
    }

    private void Move(ref SystemState state)
    {
        LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);

        playerTransform.Position += new float3(inputComponent.movement * playerComponent.MoveSpeed * SystemAPI.Time.DeltaTime, 0);

        Vector2 dir = (Vector2)inputComponent.mousePos - (Vector2)Camera.main.WorldToScreenPoint(playerTransform.Position);
        float angle = math.degrees(math.atan2(dir.y, dir.x));
        playerTransform.Rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        entityManager.SetComponentData(playerEntity, playerTransform);
    }

    private float nextShootTime;

    private void Shoot(ref SystemState state)
    {
        if(inputComponent.pressingLMB && nextShootTime < SystemAPI.Time.ElapsedTime)
        {
            EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.Temp);
            Entity bulletEntity = entityManager.Instantiate(playerComponent.BulletPrefab);

            ECB.AddComponent(bulletEntity, new BulletComponent { speed = 10, Size = 0.3f});

            LocalTransform bulletTranform = entityManager.GetComponentData<LocalTransform>(bulletEntity);
            bulletTranform.Rotation = entityManager.GetComponentData<LocalTransform>(playerEntity).Rotation;
            LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);
            bulletTranform.Position = playerTransform.Position + playerTransform.Right() + playerTransform.Up() * -0.45f;
            ECB.SetComponent(bulletEntity, bulletTranform);

            ECB.Playback(entityManager);

            nextShootTime = (float)SystemAPI.Time.ElapsedTime + playerComponent.ShootCooldown;
        }
    }
}
