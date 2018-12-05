using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

namespace Master
{
    public sealed class Archetypes
    {
        public static EntityArchetype PathArch;

        public static EntityArchetype PathSplineArchetype;
        public static EntityArchetype TrajectoryArchetype;
        public static EntityArchetype ControlPointArchetype;
        public static EntityArchetype VectorPointArchetype;


        public static void SetArchetype()
        {
            var em = World.Active.GetOrCreateManager<EntityManager>();

            PathSplineArchetype = em.CreateArchetype(
                    typeof(PathMarker),
                    typeof(Traveler),
                    typeof(CurveElement),
                    typeof(UpdateIndicator),
                    typeof(PathID)
                    
                );

            TrajectoryArchetype = em.CreateArchetype(
                    typeof(CurveMarker),
                    typeof(CurveInfo),
                    //typeof(CPTransform)
                    typeof(ControlPointElement),
                    typeof(LastWeight)

                );

            ControlPointArchetype = em.CreateArchetype(
                    typeof(ControlPointMarker),
                    typeof(PathID),
                    typeof(ControlPointID),
                    typeof(PointPosition)
                //typeof(Position)
                );



        }
    }
}
