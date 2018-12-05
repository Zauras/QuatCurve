using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;



namespace Master
{
	//using H = Master.LibQuaternionAritmetics;
	
	public class Calc5pCurveParams : AbstractCurveAlgorithm
	{
		public static DualQuaternion[] GenerateCurvePoints(float4 w0, Entity curveEntity)
		{
			EntityManager em = World.Active.GetOrCreateManager<EntityManager>();
			var pointEntBuffer = em.GetBufferFromEntity<ControlPointElement>()[curveEntity];

			float4[] points = new float4[pointEntBuffer.Length];
			for (int i = 0; i < points.Length; i++)
			{
				float3 v = em.GetComponentData<PointPosition>(pointEntBuffer[i].Value).Value;
				points[i] = new float4(v, 1f);
			}
			// Gal geriau Entity neturetu issaugomu pointuEnt, o updeit tiesiogiai is GO

			float4 delta14 = GetDelta(points[1], points[4]);
			float4 delta12 = GetDelta(points[1], points[2]);
			float4 delta34 = GetDelta(points[3], points[4]);
			float4 delta32 = GetDelta(points[3], points[2]);

			float4 delta41 = GetDelta(points[4], points[1]);
			float4 delta43 = GetDelta(points[4], points[3]);
			float4 delta10 = GetDelta(points[1], points[0]);
			float4 delta30 = GetDelta(points[3], points[0]);
            //Debug.Log(delta14 + " " + delta12 + " " + delta34);
            //Debug.Log(delta32 + " " + delta41 + " " + delta43);
            //Debug.Log(delta10 + " " + delta30);

            float4 wx = new float4(0f, 0f, 0f, 1f);
			float4 w1 = GenerateWeight(-9.0f, -3.0f, 9.0f, -1.0f,
										delta14, delta12, delta34, delta32,
										delta41, delta10, delta43, delta30);
			float4 w2 = GenerateWeight(-1.0f, -3.0f, -3.0f, -1.0f,
										delta12, delta14, delta32, delta34,
										delta12, delta10, delta32, delta30);
			em.SetComponentData(curveEntity, new LastWeight(w2));

			float4[] weights = { wx, w1, w2 };
			
			
			DualQuaternion[] curveCHD = GenerateCurve(points, weights);

			return curveCHD;
		}


	}

}

