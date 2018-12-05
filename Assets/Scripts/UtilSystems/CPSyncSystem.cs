using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Master {
    public class CPSyncSystem : ComponentSystem
    {
        public struct Point
        {
            public readonly int Length;
            public ComponentDataArray<PointPosition> posEntList;
            //public EntityArray entities;
            public ComponentDataArray<ControlPointID> pointsIDs;
            public ComponentDataArray<PathID> pathID;
            //[ReadOnly] public SharedComponentDataArray<ControlPointMarker> marker;
        }

        public struct Path
        {
            public readonly int Length;
            public ComponentDataArray<PathID> pathID;
            public ComponentDataArray<UpdateIndicator> isChanged;
            [ReadOnly] public SharedComponentDataArray<PathMarker> marker;
        }

        [Inject] Point pointGroup;
        [Inject] Path pathGroup;

        protected override void OnUpdate()
        {
            EntityManager em = World.Active.GetOrCreateManager<EntityManager>();

            for (int i = 0; i < pointGroup.Length; i++)
            {
                //float3 posE = pointGroup.posEntList[i].Value;
                //var posGO = Database.ID_GOmap[pointGroup.idList[i].Value].transform.position;

                Vector3 posGO = Database.ID_GOmap[pointGroup.pointsIDs[i].Value].transform.position;
                float3 posEnt = pointGroup.posEntList[i].Value;

                if (posGO.x != posEnt.x || posGO.y != posEnt.y || posEnt.z != posGO.z)
                {
                    pointGroup.posEntList[i] = new PointPosition(new float3(posGO.x, posGO.y, posGO.z));
                    for (int j = 0; j < pathGroup.Length; j++)
                    {
                        if (pathGroup.pathID[j].Value == pointGroup.pathID[i].Value)
                            pathGroup.isChanged[j] = new UpdateIndicator(1);
                    }
                }
            }
                
               // pointGroup.posEntList[i].Value = Database.ID_GOmap[pointGroup.pointsIDs[i].Value].transform.position;
                //em.SetComponentData(pointGroup.entities[i], new Position { Value = new float3(posGO.x, posGO.y, posGO.z) });


            

            
            //em.GetComponentObject
            
            //posEnt.Value = pointGO.transform.position;
            //Debug.Log(pointGO.transform.position +" entity:"+ posEnt.Value );
        }
    }
}
