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
    public class CurveGeneratorSystem : ComponentSystem
    {

        protected override void OnStartRunning()
        {
            Debug.Log("5. CurveGeneratorSystem: OnStartRunning()");

            foreach (var pathEntry in Database.worldPaths)
            {
                UpdatePath(pathEntry.Key);
            }
        }

        /// <summary>
        /// MUST INJECT PATHS THEN ITERATE THEM
        /// </summary>
        protected override void OnUpdate()
        {
            foreach (var pathEntry in Database.worldPaths)
            {
                var goPath = Database.ID_GOmap[pathEntry.Key];

                for (int i = 0; i < goPath.transform.childCount; i++)
                {
                    Transform curveTransform = goPath.transform.GetChild(i);
                    bool changed = false;
                    for (int c = 0; c < curveTransform.childCount; c++)
                    {
                        // Jei imanoma perkelti i JOB
                        if (curveTransform.GetChild(c).hasChanged)
                        {
                            //Debug.Log("Updating Path");
                            changed = true;
                            curveTransform.GetChild(c).hasChanged = false;
                            break;
                        }
                    }
                    if (changed)
                    {
                        changed = false;
                        UpdatePath(pathEntry.Key);
                    }
                }
            }
        }


        private void UpdatePath(int pathID)
        {
            EntityManager em = World.Active.GetOrCreateManager<EntityManager>();

           // Debug.Log("Updating Path: ID-" + pathEntity.Index + "-" + pathEntity.Version);

            var pathCurvesBuffer = em.GetBufferFromEntity<CurveElement>()[Database.worldPaths[pathID]];    

            //int curvesCount = em.GetBufferFromEntity<CurveElement>()[Database.worldPaths[pathID]].Length;
            //float3[] pathPoints = new float3[curvesCount * (BootStrap.Settings.pathResolution + 1)];
            //int pathPointer = 0;

            float4 lastWeight = new float4(0f, 0f, 0f, 1f); // (x,y,z,w)  
            DualQuaternion[] pathsCDH = new DualQuaternion[pathCurvesBuffer.Length * (BootStrap.Settings.pathResolution+1)];
            DualQuaternion[] temp = { new DualQuaternion(new float4(), new float4()) }; // dummy object

            int index = 0;
            for (int i = 0; i < pathCurvesBuffer.Length; i++)
            {
                Entity curveEntity = pathCurvesBuffer[i];
                // Dep on Curve Type: // Calc Deltas // Calc Weigth
                DualQuaternion[] curveCDH = temp;
                byte curveType = em.GetComponentData<CurveInfo>(curveEntity).type;
                if (curveType == CurveTypes.p5) {
                    curveCDH = Calc5pCurveParams.GenerateCurvePoints(lastWeight, curveEntity);
                } else if (curveType == CurveTypes.p3v2) {
                    curveCDH = CalcVecCurveParams.GenerateCurvePoints(lastWeight, curveEntity);
                }
                
                for (int j = 1; j <= curveCDH.Length; j++)
                {
                    pathsCDH[index++] = curveCDH[j - 1]; // ATEITI GAL DETI I ENTITY.Buffer
                    //Debug.Log(pathsCDH[i * j].Wt + " " + pathsCDH[i * j].Ft);
                   // Debug.Log((index) + " " + (j - 1));
                }
               
                lastWeight = em.GetComponentData<LastWeight>(curveEntity).Value;
                //foreach (var v in curve) pathPoints[pathPointer++] = v;
            }

            if (Database.PathsDisplacementQuaternions.ContainsKey(pathID))
            {
               Database.PathsDisplacementQuaternions[pathID] = pathsCDH;
            } else {
               Database.PathsDisplacementQuaternions.Add(pathID, pathsCDH);
            }
            //Debug.Log(Database.PathsDisplacementQuaternions.ContainsKey(pathID));
            //LineRendererSystem.SetPolygonPoints(pathID, pathPoints);

        }

    }
}

