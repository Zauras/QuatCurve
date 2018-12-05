using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Master
{
    public class CalcVecCurveParams : AbstractCurveAlgorithm
    {
        public static DualQuaternion[] GenerateCurvePoints(float4 w0, Entity curveEntity)
        {
            EntityManager em = World.Active.GetOrCreateManager<EntityManager>();
            var pointEntBuffer = em.GetBufferFromEntity<ControlPointElement>()[curveEntity];

            float4[] points = new float4[pointEntBuffer.Length-2];
            for (int i = 1; i < pointEntBuffer.Length-1; i++)
            {
                float3 v = em.GetComponentData<PointPosition>(pointEntBuffer[i].Value).Value;
                points[i-1] = new float4(v, 1f);
            }
            float4 frontVector = new float4(em.GetComponentData<PointPosition>(pointEntBuffer[0].Value).Value, 1f);
            float4 backVector  = new float4(em.GetComponentData<PointPosition>(
                                            pointEntBuffer[pointEntBuffer.Length - 1].Value).Value, 1f);

            // Gal geriau Entity neturetu issaugomu pointuEnt, o updeit tiesiogiai is GO
            float4 delta40 = GetDelta(points[2], points[0]);
            float4 delta20 = GetDelta(points[1], points[0]);
            float4 delta24 = GetDelta(points[1], points[2]);

            float4 wx = new float4(0f, 0f, 0f, 1f); // IGNOR w0 (experiment) !!!
            float4 w1 = GenerateWeight(-8.0f, -2.0f, 16.0f, -1.0f,
                                        delta40, delta20, backVector, delta24,
                                        delta40, frontVector, backVector, delta40);
            float4 w2 = GenerateWeight(-1.0f, -4.0f, 4.0f, 1.0f,
                                        delta20, delta40, delta24, backVector,
                                        delta20, frontVector, delta24, delta40);
            em.SetComponentData(curveEntity, new LastWeight(w2));

            float4[] weights = { wx, w1, w2 };
            DualQuaternion[] curveCHD = GenerateCurve(points, weights);

            return curveCHD;
        }
    }
}
