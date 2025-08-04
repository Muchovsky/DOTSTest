using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

// We'll use Unity.Mathematics.float3 instead of Vector3,
// and we'll use Unity.Mathematics.math.distancesq instead of Vector3.sqrMagnitude.

namespace Tutorials.Jobs.My
{
    // Include the BurstCompile attribute to Burst compile the job.

    [BurstCompile]
    public struct FindNearestJob : IJobParallelFor
    {
        // All of the data which a job will access should 
        // be included in its fields. In this case, the job needs
        // three arrays of float3.

        // Array and collection fields that are only read in
        // the job should be marked with the ReadOnly attribute.
        // Although not strictly necessary in this case, marking data  
        // as ReadOnly may allow the job scheduler to safely run 
        // more jobs concurrently with each other.

        [ReadOnly] public NativeArray<float3> TargetPositions;
        [ReadOnly] public NativeArray<float3> SeekerPositions;

        // For SeekerPositions[i], we will assign the nearest 
        // target position to NearestTargetPositions[i].
        public NativeArray<float3> NearestTargetPositions;

        /*// 'Execute' is the only method of the IJob interface.
        // When a worker thread executes the job, it calls this method.
        public void Execute(int index)
        {

            // Compute the square distance from each seeker to every target.
            for (int i = 0; i < SeekerPositions.Length; i++)
            {
                float3 seekerPos = SeekerPositions[i];
                float nearestDistSq = float.MaxValue;
                for (int j = 0; j < TargetPositions.Length; j++)
                {
                    float3 targetPos = TargetPositions[j];
                    float distSq = math.distancesq(seekerPos, targetPos);
                    if (distSq < nearestDistSq)
                    {
                        nearestDistSq = distSq;
                        NearestTargetPositions[i] = targetPos;
                    }
                }
            }
        }*/


        // An IJobParallelFor's Execute() method takes an index parameter and 
        // is called once for each index, from 0 up to the index count:
        public void Execute(int index)
        {
            float3 seekerPos = SeekerPositions[index];

            // Find the target with the closest X coord.
            int startIdx = TargetPositions.BinarySearch(seekerPos, new AxisXComparer { });

            // When no precise match is found, BinarySearch returns the bitwise negation of the last-searched offset.
            // So when startIdx is negative, we flip the bits again, but we then must ensure the index is within bounds.
            if (startIdx < 0) startIdx = ~startIdx;
            if (startIdx >= TargetPositions.Length) startIdx = TargetPositions.Length - 1;

            // The position of the target with the closest X coord.
            float3 nearestTargetPos = TargetPositions[startIdx];
            float nearestDistSq = math.distancesq(seekerPos, nearestTargetPos);

            // Searching upwards through the array for a closer target.
            Search(seekerPos, startIdx + 1, TargetPositions.Length, +1, ref nearestTargetPos, ref nearestDistSq);

            // Search downwards through the array for a closer target.
            Search(seekerPos, startIdx - 1, -1, -1, ref nearestTargetPos, ref nearestDistSq);

            NearestTargetPositions[index] = nearestTargetPos;
        }

        void Search(float3 seekerPos, int startIdx, int endIdx, int step,
            ref float3 nearestTargetPos, ref float nearestDistSq)
        {
            for (int i = startIdx; i != endIdx; i += step)
            {
                float3 targetPos = TargetPositions[i];
                float xdiff = seekerPos.x - targetPos.x;

                // If the square of the x distance is greater than the current nearest, we can stop searching.
                if ((xdiff * xdiff) > nearestDistSq) break;

                float distSq = math.distancesq(targetPos, seekerPos);

                if (distSq < nearestDistSq)
                {
                    nearestDistSq = distSq;
                    nearestTargetPos = targetPos;
                }
            }
        }
    }

    public struct AxisXComparer : IComparer<float3>
    {
        public int Compare(float3 a, float3 b)
        {
            return a.x.CompareTo(b.x);
        }
    }
}