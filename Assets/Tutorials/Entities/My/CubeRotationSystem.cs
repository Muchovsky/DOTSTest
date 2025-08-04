using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Tutorials.Entities.My
{
    public partial struct CubeRotationSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            
            var deltaTime = SystemAPI.Time.DeltaTime;
         
            foreach (var (transform, rotationSpeed) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotationSpeed>>())
            {
                // Rotate the transform around the Y axis. 
                var radians = rotationSpeed.ValueRO.RadiansPerSecond * deltaTime;
                transform.ValueRW = transform.ValueRW.RotateY(radians);
            }
        }
    }
}