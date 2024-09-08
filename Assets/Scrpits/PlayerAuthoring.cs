
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    public float speed;

    public GameObject ProjectilePrefab;

    class PlayerAuthoringBajer: Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity playerEntiy = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<PlayerTag>(playerEntiy);
            AddComponent<PlayerMoveInput>(playerEntiy);

            AddComponent(playerEntiy, new PlayerMoveSpeed { Value = authoring.speed });

            AddComponent<FireProjectileTag>(playerEntiy);
            SetComponentEnabled<FireProjectileTag>(playerEntiy, false);

            //AddComponent(playerEntiy, new ProjectillePrefab { Value = GetEntity(authoring.ProjectilePrefab,TransformUsageFlags.Dynamic)});
        }
    }
}

public struct PlayerMoveInput: IComponentData
{
    public float2 Value;
}

public struct PlayerMoveSpeed: IComponentData
{
    public float Value;
}

public struct PlayerTag : IComponentData
{

}

public struct ProjectillePrefab : IComponentData
{
    public float Value;
}

public struct ProjectileMoveSpeed : IComponentData
{
    public float Value;
}

public struct FireProjectileTag : IComponentData, IEnableableComponent
{

}
