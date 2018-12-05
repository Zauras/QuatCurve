using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.UI;

namespace Master
{
  //  [UpdateAfter(typeof(TrajectorySystem))]
    public class MovementControllerSystem : ComponentSystem
    {
        /*
        struct Travaler
        {
            //Using as a filter & dataEntries (no actual Enteties)
            public readonly int Length;
            public ComponentDataArray<PathID> pathID;
            public ComponentArray<Transform> transform;
           // [ReadOnly] public TravelerMarker marker;
        }

        [Inject] Travaler travelerGroup;
        */
        public Button NewGameButton;



        public static void MovementStep()
        {
            EntityManager em = World.Active.GetOrCreateManager<EntityManager>();
            var dicTrajectory = Database.ObjectMovementPath;
            var dicTravalers = Database.Travelers;
            foreach (var travalerEntry in dicTravalers)
            {
                
                int pathID = travalerEntry.Key;
                Entity traveler = travalerEntry.Value;
                int progressCounter = Database.MovementProgress[pathID];

                DisplacementData[] trajectory = dicTrajectory[pathID];

                var entTrasnform = em.GetComponentObject<Transform>(traveler);
                if (progressCounter >= (Database.ObjectMovementPath[pathID].Length-1))
                {
                    Database.MovementProgress[pathID] = 0;

                    entTrasnform.position = Database.ObjectMovementPath[pathID][0].position;
                    entTrasnform.rotation = Database.ObjectMovementPath[pathID][0].rotation;
                } else
                {
                    entTrasnform.position = Database.ObjectMovementPath[pathID][++progressCounter].position;
                    entTrasnform.rotation = Database.ObjectMovementPath[pathID][++progressCounter].rotation;
                    Database.MovementProgress[pathID] = progressCounter;
                }

                Debug.Log(progressCounter);
            }
                
        }

        protected override void OnStartRunning()
        {
            NewGameButton = GameObject.Find("StepButton").GetComponent<Button>();
            //HealthText = GameObject.Find("HealthText").GetComponent<Text>();

            NewGameButton.onClick.AddListener(MovementStep);

            //NewGameButton.gameObject.SetActive(true);
            Debug.Log("SetupButton");
        }


        protected override void OnUpdate()
        {
            /*
            for (int i = 0; i < travelerGroup.Length; i++)
            {
                int pathID = travelerGroup.pathID[i].Value;
                var x = travelerGroup.transform[i].position;
               // Debug.Log(x);
                travelerGroup.transform[i].position = Database.ObjectMovementPath[pathID][0];

            }
            */
        }
    }
}







/*
 * 
 * 
 *         public  void MovementStep()
        {
            var dicTrajectory = Database.ObjectMovementPath;
            for (int i = 0; i < travelerGroup.Length; i++)
            {
                int pathID = travelerGroup.pathID[i].Value;
                int progressCounter = Database.MovementProgress[pathID];
                float3[] trajectory = dicTrajectory[pathID];

                
                if (progressCounter >= Database.ObjectMovementPath[pathID].Length)
                {
                    Database.MovementProgress[pathID] = 0;
                    travelerGroup.transform[i].position = Database.ObjectMovementPath[pathID][0];
                } else
                {
                    travelerGroup.transform[i].position = Database.ObjectMovementPath[pathID][++progressCounter];
                }
            }
                
        }
 * /


/*
private List<List<Vector3>> CreateMeshPathCurves()
{
    var meshPathCurve = new List<List<Vector3>>();
    foreach (var vexPath in meshPath)
    {
        var vexPathCurve = new List<Vector3>();
        for (int i = 0; i < vexPath.Count; i++)
        {
            vexPathCurve.Add(vexPath[i] + objPath[i]); // posukis + postumis	
        }
        meshPathCurve.Add(vexPathCurve);
    }
    return meshPathCurve;
}


public void MoveDrone()
{
    droneObj.transform.position = objPath[updCounter];

    for (int i = 0; i < meshInitVexList.Count; i++)
    {
        meshInitVexList[i] = meshPath[i][updCounter];
        //print(meshInitVexList[i]);
    }
    droneScrp.UpdateAllVex(meshInitVexList);
    updCounter++;

    if (updCounter >= objPath.Count)
    {
        updCounter = 0;
        print("...RESETING...");
        meshInitVexList = droneScrp.vexDic.Keys.ToList();
    }
}

public void ConstantMoveDrone()
{
    if (updCounter == 0) droneObj.transform.position = objPath[updCounter];
    if (animationMove)
    {
        float step = animationSpeed * Time.deltaTime;
        droneObj.transform.position = Vector3.MoveTowards(droneObj.transform.position, objPath[updCounter], step);
        Vector3 futurePosition = objPath[updCounter];
        for (int i = 0; i < meshInitVexList.Count; i++)
        {
            meshInitVexList[i] = Vector3.MoveTowards(meshInitVexList[i], meshPath[i][updCounter], step / 4.5f);
        }
        droneScrp.UpdateAllVex(meshInitVexList);

        if (droneObj.transform.position == futurePosition) updCounter++;
        if (updCounter >= objPath.Count)
        {
            animationMove = false;
            updCounter = 0;
        }
    }


}

public void SwitchToogleAnimation()
{
    if (animationMove) animationMove = false;
    if (!animationMove) animationMove = true;
}
*/

/*
struct Travaler
{
//Using as a filter & dataEntries (no actual Enteties)
// public int Length;
// public P pathID;
public Transform transform;
// [ReadOnly] public TravelerMarker marker;
}
*/
