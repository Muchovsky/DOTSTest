using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Tutorials.Entities.Hello
{
    public partial struct SpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Spawner>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var prefab = SystemAPI.GetSingleton<Spawner>().CubePrefab;
            var instances = state.EntityManager.Instantiate(prefab, 10, Allocator.Temp);

            // randomly set the positions of the new cubes
            var random = new Random(123);
            foreach (var entity in instances)
            {
                var transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                transform.ValueRW.Position = random.NextFloat3(new float3(10, 10, 10));
            }
        }
    }
}