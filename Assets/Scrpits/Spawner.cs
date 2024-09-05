using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Spawner : IComponentData
{
    public Entity Prefabs;
    public float2 SpawnPosition;
    public float NextSpawnTime;
    public float SpawnRate;
}
    
