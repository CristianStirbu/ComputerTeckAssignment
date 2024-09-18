using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;
using UnityEngine;
using Unity.Physics;
using Unity.Collections;
using static UnityEngine.EventSystems.EventTrigger;

public partial class EnemySpawnSystem : SystemBase
{
    private EnemySpawnerComponent enemySpawnerComponent;
    private EnemyDataContainer enemyDataContainerComponent;
    private Entity enemySpawnEntity;
    public float WaveTime = 0;
    public bool Wave;
    private float nextSpawnTime;
    private Random random;

    protected override void OnCreate()
    {
        random = Random.CreateFromIndex((uint)enemySpawnerComponent.GetHashCode());
    }

    protected override void OnStartRunning()
    {
        Wave = false;
    }

    protected override void OnUpdate()
    {
        WaveTime += SystemAPI.Time.DeltaTime;

        if (!SystemAPI.TryGetSingletonEntity<EnemySpawnerComponent>(out enemySpawnEntity))
        {
            return;
        }

        enemySpawnerComponent = EntityManager.GetComponentData<EnemySpawnerComponent>(enemySpawnEntity);
        enemyDataContainerComponent = EntityManager.GetComponentObject<EnemyDataContainer>(enemySpawnEntity);
        

        if(WaveTime > 5)
        {
            Wave = true;
        }

        if (WaveTime > 10 )
        {
            WaveTime = 0;
            Wave = false;
        }

        if (Wave == true)
        {
            if (SystemAPI.Time.ElapsedTime > nextSpawnTime)
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        int level = 2;
        List<EnemyData> availbleEnemies = new List<EnemyData>();
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        EntityManager entityManager = EntityManager;
        NativeArray<Entity> allEntities = entityManager.GetAllEntities();

        foreach (EnemyData enemyData in enemyDataContainerComponent.enemies)
        {
            if(enemyData.level <= level)
            {
                availbleEnemies.Add(enemyData);
            }
        }

        int index = random.NextInt(availbleEnemies.Count);

        Entity newEnemy = EntityManager.Instantiate(availbleEnemies[index].prefab);
        EntityManager.SetComponentData(newEnemy, new LocalTransform
        {
            Position = GetPositionOutsideOfCaneraRange(),
            Rotation = quaternion.identity,
            Scale = 1
        });

        
        EntityManager.AddComponentData(newEnemy, new EnemyComponent { currentHealth = availbleEnemies[index].health });

        nextSpawnTime = (float)SystemAPI.Time.ElapsedTime + enemySpawnerComponent.spawnCooldown;

       /*  foreach(Entity entity in allEntities)
        {
            if (entityManager.HasComponent<EnemyComponent>(entity))
            {
                 LocalTransform enemyTransform = entityManager.GetComponentData<LocalTransform>(entity);
                EnemyComponent enemyComponent = entityManager.GetComponentData<EnemyComponent>(entity);

                NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);
                physicsWorld.SphereCastAll(enemyTransform.Position, enemyComponent.Size / 2, float3.zero, 1,
                        ref hits, new CollisionFilter { BelongsTo = (uint)CollisionLayer.Default, CollidesWith = (uint)CollisionLayer.Bullet });

                foreach (ColliderCastHit hit in hits)
                {
                    entityManager.DestroyEntity(entity);
                }

                hits.Dispose();
            }

           
        }*/


    }

    private float3 GetPositionOutsideOfCaneraRange()
    {
        float3 position = new float3(random.NextFloat2(-enemySpawnerComponent.cameraSize * 1, enemySpawnerComponent.cameraSize * 3), 0);

        while (position.x < enemySpawnerComponent.cameraSize.x && position.x > -enemySpawnerComponent.cameraSize.x
            && position.y < enemySpawnerComponent.cameraSize.y && position.y > -enemySpawnerComponent.cameraSize.y)
        {
            position = new float3(random.NextFloat2(-enemySpawnerComponent.cameraSize * 1, enemySpawnerComponent.cameraSize * 3), 0); 
        }

        position += new float3(Camera.main.transform.position.x, Camera.main.transform.position.y,0);

        return position;
    }
}
