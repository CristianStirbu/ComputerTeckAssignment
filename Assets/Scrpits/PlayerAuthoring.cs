
using Unity.Entities;
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
        }
    }
}
