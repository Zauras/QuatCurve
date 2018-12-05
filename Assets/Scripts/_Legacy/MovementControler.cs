using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MovementControler : MonoBehaviour {

	public bool renderMeshPath = true;
	protected Lib_QuaternionAritmetics H;
	GameObject[] curveObjsArr;
	CurveGenerator[] curveGenArr;
	//List<Vector3>[] curvesMeshVex;
	List<List<Vector4>> meshPath;
	List<List<Vector3>> verteciesPaths;
	List<Vector4> objPath;	
	List<Vector3> meshInitVexList;
	Drone droneScrp;
	public GameObject droneObj;
	private LineManager LM;
	static int updCounter = 0;

	public float animationSpeed = 1f;
	bool animationMove = false;
	bool inStep = false;
	Vector3 presentPosition;

	void Start () {
		H = new Lib_QuaternionAritmetics();
		droneScrp = droneObj.GetComponent<Drone>();//gameObject.GetComponent<Drone>();//droneObj.GetComponent<Drone>();
		meshInitVexList = droneScrp.vexDic.Keys.ToList();
		

		objPath = new List<Vector4>();
		meshPath = new List<List<Vector4>>(); // Su salyga, kad neatsiras daugiau vex meshe eigoje
		foreach (var vex in meshInitVexList) meshPath.Add(new List<Vector4>());
		

		curveObjsArr = GetCurves("Trajectory");
		curveGenArr = ExtractSrpts_fromObjs(curveObjsArr);
		GenerateSplinePath();
		
		LM = droneObj.GetComponent<LineManager>();
		if (renderMeshPath) {
		verteciesPaths = CreateMeshPathCurves();
		LM.AddManyCurves(verteciesPaths.Count);
		LM.UpdateAllCurves(verteciesPaths);
		}
	}

	private void FixedUpdate() {
		//meshPath = new List<List<Vector4>>(); // Su salyga, kad neatsiras daugiau vex meshe eigoje
		//foreach (var vex in meshInitVexList) meshPath.Add(new List<Vector4>());

		//curveObjsArr = GetCurves("Trajectory");
		//curveGenArr = ExtractSrpts_fromObjs(curveObjsArr);
		//GenerateSplinePath();

		//verteciesPaths = CreateMeshPathCurves();
		//LM.UpdateAllCurves(verteciesPaths);

		if(animationMove) ConstantMoveDrone();
	}

	public Vector4 Displacement(Vector4[] c, Vector4 x) {
		// c=[p;q]; x=[0, x,y,z]
		// c:x -> (q*x - 2*p)*Inv(q);
		Vector4 p = c[0], q = c[1];
		Vector4 newPoint = H.Mult(H.Mult(p, x) + 1.0f*q, H.Invers(p));
		return newPoint;
	}

	public Vector4 DisplacementRotation(Vector4[] c, Vector4 x) {
		// c=[p;q] atitinka [Wt,Ft] ; x=[0, x,y,z]
		// c:x -> p*x*inv(p); just rotation
		Vector4 p = c[0], q = c[1]; // p - tik su svoriais, q - su taskai+svoriais
		//Vector4 newPoint = H.Mult(H.Mult(p, x) + 1.0f*q, H.Invers(p));
		Vector4 newPoint = H.Mult(H.Mult(p, x), H.Invers(p)); // tik posukis
		return newPoint;
	}

	private void GenerateSplinePath() {
		List<Vector3> initVexList = meshInitVexList;
		
		for (int s=0; s < curveGenArr.Length; s++) { // loop per curves
			List<Vector3> lastMeshPosition = new List<Vector3>(); // last positions of vertecies 
			for (int v=0; v < initVexList.Count; v++) { // loop per vertecies
				List<Vector4> vexPath = new List<Vector4>();
				// reikia posukio pratesimui naujoje kreiveje
				for (int cdh=0; cdh < curveGenArr[s].cDH_List.Count; cdh++) { // loop per kof.of cuve fragments
					Vector4 initVex = initVexList[v];
					Vector4[] cDH = curveGenArr[s].cDH_List[cdh];
					vexPath.Add(DisplacementRotation(cDH, initVex));
				}
				meshPath[v].AddRange(vexPath);
				lastMeshPosition.Add(vexPath.Last()); // susirenkam paskutinius vex
			}
			objPath.AddRange(curveGenArr[s].curve); // obj postumis == kreives fragmentas
			//SetNext initMeshVex
			initVexList = lastMeshPosition; // praeitos kreives mesho pozicija => sekancios mesho startas
		}
	}

	private List<List<Vector3>> CreateMeshPathCurves() {
		var meshPathCurve = new List<List<Vector3>>();
		foreach (var vexPath in meshPath){
			var vexPathCurve = new List<Vector3>();
			for (int i=0; i < vexPath.Count; i++){	
				vexPathCurve.Add(vexPath[i] + objPath[i]); // posukis + postumis	
			}
			meshPathCurve.Add(vexPathCurve);
		}
		return meshPathCurve;
	}


	public void MoveDrone () {
		droneObj.transform.position = objPath[updCounter];

		for (int i=0; i < meshInitVexList.Count; i++) {
			meshInitVexList[i] = meshPath[i][updCounter];
			//print(meshInitVexList[i]);
		}
		droneScrp.UpdateAllVex(meshInitVexList);
		updCounter++;

		if (updCounter >= objPath.Count) {
			updCounter = 0;
			print("...RESETING...");
			meshInitVexList = droneScrp.vexDic.Keys.ToList();
		} 
	}

	public void ConstantMoveDrone () {
		if (updCounter == 0) droneObj.transform.position = objPath[updCounter];
		if (animationMove) {
			float step = animationSpeed * Time.deltaTime;
			droneObj.transform.position = Vector3.MoveTowards(droneObj.transform.position, objPath[updCounter], step);
			Vector3 futurePosition =  objPath[updCounter];
			for (int i=0; i < meshInitVexList.Count; i++) {
				meshInitVexList[i] = Vector3.MoveTowards(meshInitVexList[i], meshPath[i][updCounter], step/4.5f);
			}
			droneScrp.UpdateAllVex(meshInitVexList);

			if (droneObj.transform.position == futurePosition) updCounter++;
			if (updCounter >= objPath.Count) {
				animationMove = false;
				updCounter = 0;
			} 
		}


	}

	public void SwitchToogleAnimation() {
		if(animationMove) animationMove = false;
		if(!animationMove) animationMove = true;
	}



	// ####################### Utilities #########################

	private GameObject[] GetCurves (string tag) {
		// if not spline find trajectory
		GameObject[] curvesObjs = GameObject.FindGameObjectsWithTag(tag)
											.OrderBy (go => go.name).ToArray ();
		return curvesObjs;
	}

	private CurveGenerator[] ExtractSrpts_fromObjs(GameObject[] objs) {
		CurveGenerator[] scripts = new CurveGenerator[objs.Length];
		for (int i=0; i < objs.Length; i++) {
			scripts[i] = objs[i].GetComponent<CurveGenerator>();
		}
		return scripts;
	}

}
