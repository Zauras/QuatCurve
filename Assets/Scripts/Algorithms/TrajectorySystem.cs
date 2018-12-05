using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

namespace Master
{
    using H = Master.LibQuaternionAritmetics;

    [UpdateAfter(typeof(CurveGeneratorSystem))]
    public class TrajectorySystem : ComponentSystem
    {
        struct PathGroup
        {
            //Using as a filter & dataEntries (no actual Enteties)
            public readonly int Length;
            public ComponentDataArray<PathID> goID;
            public ComponentDataArray<UpdateIndicator> isChanged;
        }
        [Inject] PathGroup pathGroup;

        private static float4 DisplacementMovement(DualQuaternion c)
        {
            // c=[p;q]; x=[0, x,y,z]
            // c:x -> (q*x - 2*p)*Inv(q);
            //float4 p = c[0], q = c[1];
            //float4 newPoint = H.Mult(H.Mult(p, x) + 1.0f * q, H.Invers(p));

            // TAIP IRGI VEIKIA
            float4 curvePoint = H.Mult(c.Ft, H.Invers(c.Wt)); //H.Mult(Ft, H.Invers(Wt)) = making path of movement
            return curvePoint;
        }

        private static float4 DisplacementRotation(DualQuaternion c, float4 x)
        {
            // c=[p;q] atitinka [Wt,Ft] ; x=[0, x,y,z]
            // c:x -> p*x*inv(p); just rotation
            float4 p = c.Wt, q = c.Ft; // p - tik su svoriais, q - su taskai+svoriais
                                       //Vector4 newPoint = H.Mult(H.Mult(p, x) + 1.0f*q, H.Invers(p));
            //float4 newPoint = H.Mult(H.Mult(p, x), H.Invers(p)); // tik posukis
            float4 newPoint = H.Mult(H.Mult(p, x), H.Invers(p)); // tik posukis

            return newPoint;
        }


        private static void SetDataToPath(int pathID)
        {
            DualQuaternion[] pathsCDH = Database.PathsDisplacementQuaternions[pathID];
            DisplacementData[] movementPath = new DisplacementData[pathsCDH.Length];
            float3[] spline = new float3[pathsCDH.Length];

            for (int i = 0; i < pathsCDH.Length; i++)
            {
                float4 movementPoint = DisplacementMovement(pathsCDH[i]);
                float4 rotationPoint = DisplacementRotation(pathsCDH[i], movementPoint);
                spline[i] = new float3(movementPoint.x, movementPoint.y, movementPoint.z);
                movementPath[i] = new DisplacementData(
                    spline[i],
                    new quaternion(pathsCDH[i].Wt)
                    //(rotationPoint.x, rotationPoint.y, rotationPoint.z)
                    );
            }
            // Conncet Traveler with Trajectory
            if (Database.ObjectMovementPath.ContainsKey(pathID)) { Database.ObjectMovementPath[pathID] = movementPath; }
            else {
                Database.MovementProgress.Add(pathID, 0);
                Database.ObjectMovementPath.Add(pathID, movementPath);
            }
            // Update LineRenderer
            LineRendererSystem.SetPolygonPoints(pathID, spline);
        }


        protected override void OnUpdate()
        {
            for (int i = 0; i < pathGroup.Length; i++)
            {
                if (pathGroup.isChanged[i].Value == 1) //Because bool is not blittable ;((
                {
                    pathGroup.isChanged[i] = new UpdateIndicator(0);
                    // SET/Reset Data
                    SetDataToPath(pathGroup.goID[i].Value);
                }
            }

        }
    }
}
