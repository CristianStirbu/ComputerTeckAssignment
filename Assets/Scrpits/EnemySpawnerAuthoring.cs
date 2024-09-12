using Unity.Mathematics;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public float spwanCooldown = 1;
    public Vector2 cameraSize;
    public List<EnemySO> enemySO;

    public class EnemySpawnBaker : Baker<EnemySpawnerAuthoring> 
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            Entity enemySpawnerAuthoring = GetEntity(TransformUsageFlags.None);

            AddComponent(enemySpawnerAuthoring, new EnemySpawnerComponent
            {
                spawnCooldown = authoring.spwanCooldown,
                cameraSize = authoring.cameraSize,
            });

            List<EnemyData> enemyData = new List<EnemyData>();

            foreach(EnemySO e in authoring.enemySO)
            {
                enemyData.Add(new EnemyData
                {
                    level = e.level,
                    moveSpeed = e.moveSpeed,
                    prefab = GetEntity(e.prefab, TransformUsageFlags.None)
                });
            }

            AddComponentObject(enemySpawnerAuthoring,new EnemyDataContainer { enemies = enemyData});

        }
    }
}
