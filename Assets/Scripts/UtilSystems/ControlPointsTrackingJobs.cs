
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace Master
{
    public class ControlPointsTrackingJob : JobComponentSystem
    {
        /*
        public struct Point
        {
            public readonly int Length;
            public ControlPointMarker marker;
            public Position position;
        }

        [Inject] Point pointGroup;

        public struct SyncPoints : IJobParallelFor
        {
            public IDictionary<>
                

            public void Execute(int i) // Kiekvienam pointui ...
            {
               // Debug.Log();
                // Buffers[i].Append(i * 3);
            }
        }


        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new SyncPoints
            {
                //Buffers = m_Data.Buffers
            }.Schedule(pointGroup.Length, 32, inputDeps);
        }
        */
    }
}
