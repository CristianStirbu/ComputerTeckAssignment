using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
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
            Entity bulletEntity = entityManager.Instantiate(playerComponent.BulletPrefab);
            nextShootTime = (float)SystemAPI.Time.ElapsedTime + playerComponent.ShootCooldown;
        }
    }
}
