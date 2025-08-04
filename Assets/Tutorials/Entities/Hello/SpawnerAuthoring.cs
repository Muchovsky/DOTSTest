using Unity.Entities;
using UnityEngine;

namespace Tutorials.Entities.Hello
{
    public class SpawnerAuthoring : MonoBehaviour
    {
        public GameObject CubePrefab;

        class Baker : Baker<SpawnerAuthoring>
        {
            public override void Bake(SpawnerAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                var spawner = new Spawner
                {
                    CubePrefab = GetEntity(authoring.CubePrefab, TransformUsageFlags.Dynamic)
                };
                AddComponent(entity, spawner);	
            }
        }
    }

    struct Spawner : IComponentData
    {
        public Entity CubePrefab;
    }
}