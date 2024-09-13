using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;
using UnityEngine;

public partial class EnemySpawnSystem : SystemBase
{
    private EnemySpawnerComponent enemySpawnerComponent;
    private EnemyDataContainer enemyDataContainerComponent;
    private Entity enemySpawnEntity;
    private float nextSpawnTime;
    private Random random;

    protected override void OnCreate()
    {
        random = Random.CreateFromIndex((uint)enemySpawnerComponent.GetHashCode());
    }

    protected override void OnUpdate()
    {
        if(!SystemAPI.TryGetSingletonEntity<EnemySpawnerComponent>(out enemySpawnEntity))
        {
            return;
        }

        enemySpawnerComponent = EntityManager.GetComponentData<EnemySpawnerComponent>(enemySpawnEntity);
        enemyDataContainerComponent = EntityManager.GetComponentObject<EnemyDataContainer>(enemySpawnEntity);

        if(SystemAPI.Time.ElapsedTime > nextSpawnTime)
        {
            SpawnEnemy();
        }

    }

    private void SpawnEnemy()
    {
        int level = 2;
        List<EnemyData> availbleEnemies = new List<EnemyData>();

        foreach(EnemyData enemyData in enemyDataContainerComponent.enemies)
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
    }

    private float3 GetPositionOutsideOfCaneraRange()
    {
        float3 position = new float3(random.NextFloat2(-enemySpawnerComponent.cameraSize * 2, enemySpawnerComponent.cameraSize * 2), 0);

        while (position.x < enemySpawnerComponent.cameraSize.x && position.x > -enemySpawnerComponent.cameraSize.x
            && position.y < enemySpawnerComponent.cameraSize.y && position.y > -enemySpawnerComponent.cameraSize.y)
        {
            position = new float3(random.NextFloat2(-enemySpawnerComponent.cameraSize * 2, enemySpawnerComponent.cameraSize * 2), 0); 
        }

        position += new float3(Camera.main.transform.position.x, Camera.main.transform.position.y,0);

        return position;
    }
}
