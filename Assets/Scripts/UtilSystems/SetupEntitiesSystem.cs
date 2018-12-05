using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Master
{
    public class SetupEntitiesSystem
    {
        private static EntityManager em;
       
        public static void EntitiseGOHierarchy()
        {
            Debug.Log("4. SetupEntitiesSystem: Surenkam GO & gaminam Enities Hierarchy");

            em = World.Active.GetOrCreateManager<EntityManager>();
            SetupPaths();
        }

        /// <summary>
        /// Setup paths: 
        /// Creating Pure Entity
        /// Adding Buffer of Curve Entities
        /// Maping PathEntity with PathGO ir Database
        /// </summary>
        private static void SetupPaths()
        {
            GameObject[] pathsGO = GetGameObjectsByTag("Path");
            foreach (var path in pathsGO)
            {   
                // Setup Traveler object-entity
                GameObject travelerGO = path.GetComponent<TravelerSetter>().traveler;
                Entity travelerEnt = GameObjectEntity.AddToEntityManager(em, travelerGO);
                em.AddComponentData(travelerEnt, new PathID(path.GetInstanceID()));
                em.AddSharedComponentData(travelerEnt, new TravelerMarker { });
                Database.Travelers.Add(path.GetInstanceID(), travelerEnt);

                // Setup Path Entity
                Entity pathEntity = em.CreateEntity(Archetypes.PathSplineArchetype);
                em.SetComponentData(pathEntity, new Traveler(travelerEnt));
                em.SetComponentData(pathEntity, new PathID(path.GetInstanceID()));
                em.SetComponentData(pathEntity, new UpdateIndicator(1));

                List<Entity> tempCurvesEntities = SetupCurves(path);
                em.SetComponentData(pathEntity, new Traveler());

                

                // Saving curves into path
                DynamicBuffer<CurveElement> pathCurvesBuffer = em.GetBufferFromEntity<CurveElement>()[pathEntity];
                foreach (var ce in tempCurvesEntities)
                {
                    pathCurvesBuffer.Add(new CurveElement(ce));
                }

                Database.worldPaths.Add(path.GetInstanceID(), pathEntity);
                Database.ID_GOmap.Add(path.GetInstanceID(), path);
            }
        }

        /// <summary>
        /// Setup Curves of Path: 
        /// Find worlds curveGO & Creating Pure Entity
        /// Setting CurveType depended on children of PointGO
        /// Creating Buffer of PointEntities
        /// Returning List<CurveEntities>
        /// </summary>
        private static List<Entity> SetupCurves(GameObject path)
        {
            List<GameObject> curvesGO = GetChildrenByTag(path, "Trajectory"); // Find all curves in path
            List<Entity> tempCurvesEntities = new List<Entity>();

            foreach (var curve in curvesGO)
            {
                Database.ID_GOmap.Add(curve.GetInstanceID(), curve);
                Entity curveEntity = em.CreateEntity(Archetypes.TrajectoryArchetype); // Create CurveEntity
                tempCurvesEntities.Add(curveEntity); // Add CurveEntity to tempList

                List<GameObject> pointsGO = GetChildrenByTag(curve, "Point"); // Find all points in curve
                                                                              // Set Curve type:
                byte curveType = 0;
                if (pointsGO.Count == 5) { curveType = CurveTypes.p5; }
                else if (pointsGO.Count == 3) {
                    curveType = CurveTypes.p3v2;
                    List<GameObject> vecGO = GetChildrenByTag(curve, "VecPoint");
                    pointsGO.Insert(0, vecGO[0]);
                    pointsGO.Add(vecGO[1]);
                }
                em.SetComponentData(curveEntity, new CurveInfo(path.GetInstanceID(), curveType));
                List<Entity> tempPointEntities = SetupControlPoints(path.GetInstanceID(), pointsGO);
                // Saving points into curve
                var curvePoitnsBuffer = em.GetBufferFromEntity<ControlPointElement>()[curveEntity];
                foreach (var pe in tempPointEntities)
                {
                    curvePoitnsBuffer.Add(new ControlPointElement(pe));
                }
            }
            return tempCurvesEntities;
        }

        /// <summary>
        /// Setup PointEntities: 
        /// Find worlds pointGO & Creating Pure Entity
        /// Set Position & GameObjectID as componentData
        /// Returning List<PointEtities>
        /// </summary>
        private static List<Entity> SetupControlPoints(int pathID, List<GameObject> pointsGO)
        {
            List<Entity> tempPointEntities = new List<Entity>();
            foreach (var p in pointsGO)
            {
                Database.ID_GOmap.Add(p.GetInstanceID(), p);
               // var pointEntity = GameObjectEntity.AddToEntityManager(em, p);

                Entity pointEntity = em.CreateEntity(Archetypes.ControlPointArchetype);
                float3 pos = new float3( p.transform.position.x, p.transform.position.y, p.transform.position.z );
                em.SetComponentData(pointEntity, new PointPosition(pos));
                em.SetComponentData(pointEntity, new ControlPointID(p.GetInstanceID()));
                em.SetComponentData(pointEntity, new PathID(pathID));
                //em.GetComponentData<Position>(pointEntity).Value = p.transform.position;
                //em.AddSharedComponentData(pointEntity, new ControlPointMarker { });
                tempPointEntities.Add(pointEntity);
            }
            return tempPointEntities;
        }


        // ####### Utilities ########

        /// <summary>
        /// Get ChildrenObject by Tag
        /// </summary>
        public static List<GameObject> GetChildrenByTag(GameObject gameObject, string childTag)
        {
            List<GameObject> childGOList = new List<GameObject>();
            Transform parentTrans = gameObject.transform;
            foreach (Transform child in parentTrans)
            {
                if (child.tag == childTag)
                {
                    GameObject point = child.gameObject;
                    childGOList.Add(point);
                }
            }
            return childGOList;
        }

        /// <summary>
        /// Get GameObject by Tag
        /// </summary> 
        public static GameObject[] GetGameObjectsByTag(string tag)
        {
            // if not spline find trajectory
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag)
                                                .OrderBy(GO => GO.name).ToArray();
            return gameObjects;
        }

    }
}


/*
 *  public static void CreateTrajectories()
        {
            Debug.Log("4. SetupEntitiesSystem: Surenkam GO & gaminam Enities Hierarchy");

            var em = World.Active.GetOrCreateManager<EntityManager>();
            worldCurvesGO = new List<List<GameObject>>();


            /// #### PATHS: ############################
            worldPaths = new Dictionary<Entity, GameObject>();
            GameObject[] pathsGO = GetGameObjectsByTag("Path");
            foreach (var path in pathsGO)
            {
               Entity pathEntity = em.CreateEntity(Archetypes.PathSplineArchetype);
                var curvesEntities = new List<Entity>();



                /// #### CURVES: ############################
                List<GameObject> curvesGO = GetChildrenByTag(path, "Trajectory");
                foreach (var curve in curvesGO)
                {
                    //Get Curve Enityty & curve cpList:s
                    Entity curveEntity = em.CreateEntity(Archetypes.TrajectoryArchetype);
                    curvesEntities.Add(curveEntity);
                    
                    List<GameObject> pointsGO = GetChildrenByTag(curve, "Point");
                    // Set Curve type:
                    int curveType = 0;
                    if (pointsGO.Count == 5) { curveType = CurveTypes.p5; }
                    else if (pointsGO.Count == 3) { curveType = CurveTypes.p3v2; }
                    em.SetComponentData(curveEntity, new CurveInfo(curveType));

                    List<Entity> pointsEntities = new List<Entity>();
                    worldCurvesGO.Add(pointsGO);


                    
                    /// #### CONTROL POINTS: ##########################
                    foreach (var p in pointsGO)
                    {
                        Entity pointEntity = em.CreateEntity(Archetypes.PathSplineArchetype);

                        em.AddComponentData(pointEntity, new Position {
                            Value = new float3(p.transform.position.x, p.transform.position.y, p.transform.position.z)
                        });

                        var f = em.GetComponentData<Position>(pointEntity);
                        //Debug.Log(f.Value);

                        pointsEntities.Add(pointEntity);
                    }

                    /// Saving points into curve
                    var curvePoitnsBuffer = em.GetBufferFromEntity<CurveElement_ControlPoint>()[curveEntity];
                    foreach (var pe in pointsEntities)
                    {
                        curvePoitnsBuffer.Add(new CurveElement_ControlPoint(pe));
                    }
                }

                /// Saving curves into path
                var pathCurvesBuffer = em.GetBufferFromEntity<PathElement_Curve>()[pathEntity];
                foreach (var ce in curvesEntities)
                {
                    pathCurvesBuffer.Add(new PathElement_Curve(ce));
                }
                worldPaths.Add(pathEntity, path);
            }
            
        }
 */
