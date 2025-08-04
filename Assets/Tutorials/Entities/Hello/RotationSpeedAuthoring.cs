using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tutorials.Entities.Hello
{
    public class RotationSpeedAuthoring : MonoBehaviour
    {
        public float DegreesPerSecond;

        // A baker for RotatationSpeedAuthoring.
        // When a subscene is baked, the Bake method will be invoked for 
        // every GameObject with RotationSpeedAuthoring.
        class Baker : Baker<RotationSpeedAuthoring>
        {
            public override void Bake(RotationSpeedAuthoring authoring)
            {
                // Get the GameObject's entity.
                // The TransformUsageFlags value specifies what transform 
                // components (if any) the entity should be given.
                // An entity marked 'Dynamic' will be given a 
                // LocalTransform component.  
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            
                // Add the RotationSpeed component to the entity, using 
                // the DegreesPerSecond from the authoring MonoBehaviour 
                // converted to radians.
                var rotationSpeed = new RotationSpeed
                {
                    Value = math.radians(authoring.DegreesPerSecond)
                };
                AddComponent(entity, rotationSpeed);
            }
        }
    }
    public struct RotationSpeed : IComponentData
    {
        public float Value;
    }
}