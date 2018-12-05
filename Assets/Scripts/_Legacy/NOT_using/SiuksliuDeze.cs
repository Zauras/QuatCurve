
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CurveGenerator : MonoBehaviour {

	protected Lib_QuaternionAritmetics H;
	public GameObject pointPrefab, vectorPrefab, drone;
	public bool withVectors=false;
	public Vector4 weight0;
	public List<GameObject> CPList, VecPointers;
	public List<Vector4> points, weights, vectors;
	protected List<Vector4> curve;
	List<Vector4[]> cDH_List;
	public List<float> TList;
	public float resolution = 70f;
	List<float> timeList;
	protected LineManager LM;
	GameObject begVectorArrow, endVectorArrow;
	Vector3 vecRot1, vecRot2; //
	static int updCounter = 0;

	List<Vector4> path;

	// Use this for initialization
	void Start () {
		H = new Lib_QuaternionAritmetics();

		cDH_List = new List<Vector4[]>();
		path = new List<Vector4>();

		TList = new List<float>() {0.0f, 2.0f, 4.0f};
		timeList = getTimeList();
		//points  = new List<Vector4>() {new Vector4(0,0,0,0), new Vector4(1,1,1,1), new Vector4(2,2,2,2), new Vector4(3,3,3,3), new Vector4(3,5,5,4)};
		CPList = Create_FirstPointList("Point");
		points = Convert_Vec3_toVec4(FromGO_toVectorList(CPList));

		weight0 = new Vector4(0, 0, 0, 1.0f);
		weights = new List<Vector4>(){weight0};

		if(!withVectors) {
			Vector4 delta14 = GetDelta(points[1], points[4]);
			Vector4 delta12 = GetDelta(points[1], points[2]);
			Vector4 delta34 = GetDelta(points[3], points[4]);
			Vector4 delta32 = GetDelta(points[3], points[2]);

			Vector4 delta41 = GetDelta(points[4], points[1]);
			Vector4 delta43 = GetDelta(points[4], points[3]);
			Vector4 delta10 = GetDelta(points[1], points[0]);
			Vector4 delta30 = GetDelta(points[3], points[0]);
			
			weights.Add(GenerateWeight(-9.0f, -3.0f, 9.0f, -1.0f, delta14, delta12, delta34, delta32, delta41, delta10, delta43, delta30));
			weights.Add(GenerateWeight(-1.0f, -3.0f, -3.0f, -1.0f, delta12, delta14, delta32, delta34, delta12, delta10, delta32, delta30));
		
		} else {
			Vector4 delta40 = GetDelta(points[4], points[0]);
			Vector4 delta20 = GetDelta(points[2], points[0]);
			Vector4 delta24 = GetDelta(points[2], points[4]);
			//vectors = new List<Vector4>() {new Vector4(-0.25f, 0.5f, 0.2f, 0.0f), new Vector4(0.33f, -0.5f, 0.5f, 0.0f)};
			VecPointers = Create_FirstPointList("VectorPointer");
			vectors = Convert_Vec3_toVec4(FromGO_toVectorList(VecPointers));
			//vectors[0] = transform.TransformPoint(vectors[0]);
			//vectors[1] = transform.TransformPoint(vectors[1]);
			vecRot1 =  vectors[0] - points[0]; // target - shooter = direction
			vecRot2 = vectors[1]- points.Last();
			begVectorArrow = Instantiate(vectorPrefab, points[0], Quaternion.LookRotation(vecRot1)); //from point to vecPonter
			begVectorArrow.transform.parent = gameObject.transform;
			endVectorArrow = Instantiate(vectorPrefab, points.Last(), Quaternion.LookRotation(vecRot2));
			endVectorArrow.transform.parent = gameObject.transform;

			weights.Add(GenerateWeight(-8.0f, -2.0f, 16.0f, -1.0f, delta40, delta20, vectors[1], delta24, delta40, vectors[0], vectors[1], delta40));
			weights.Add(GenerateWeight(-1.0f, -4.0f, 4.0f, 1.0f, delta20, delta40, delta24, vectors[1], delta20, vectors[0], delta24, delta40));
			
		}
		curve = GenerateCurve();
		LM = GetComponent<LineManager>();
		LM.UpdateData (Convert_Vec4_toVec3(curve));

		drone.transform.position = points[0];
		MakePath();


		print(curve.Count);
	}

	public void MakePath () {
		
		Vector4 lastPoint = drone.transform.position;
		//Vector4 lastPoint = points[0];
		//Vector4 lastPoints = new Vector(0,0,0,0);
		foreach (Vector4[] cDH in cDH_List) {
			//Vector4 droneCoord = drone.transform.position;
			path.Add(Displacement(cDH, lastPoint));
			//lastPoint = path.Last();
		}
		//drone.transform.position = Vector3.MoveTowards(transform.position, newCoord, step);
		//path = curve;
	}
	public void MoveDrone () {
		float step = 1f * Time.deltaTime;
		if (updCounter >= path.Count){
			updCounter = 0;
			drone.transform.position = points[0];
		} 
		//drone.transform.position = Vector3.MoveTowards(drone.transform.position, path[updCounter], step);
		drone.transform.position = path[updCounter];
		updCounter++;
		//drone.transform.position = Vector3.MoveTowards(transform.position, newCoord, step);
	}

	IEnumerator Wait_aBit()
    {  yield return new WaitForSeconds(0.2f); }
	
	void FixedUpdate() {
		
		// Check if control points changed
		List<Vector4> pointsNew = Convert_Vec3_toVec4(FromGO_toVectorList(CPList));
		bool pointsChanged = false;

		for (int i = 0; i < pointsNew.Count; i++) {
			pointsChanged = points[i] != pointsNew[i];
			if (pointsChanged == true) {
				points = pointsNew;
				break;
			}
		}
		// Check if vectors changed
		bool vecChanged = false;
		if (withVectors) {
			List<Vector4> vectorsNew = Convert_Vec3_toVec4(FromGO_toVectorList(VecPointers));
			vecChanged = (vectorsNew[0] != vectors[0]) || (vectorsNew[1] != vectors[1]);
			if (vecChanged) vectors = vectorsNew;
		}
		// Update if something changed
		if (!withVectors && pointsChanged) {
			UpdateWith_5Points();
		} else if (withVectors && (pointsChanged || vecChanged)) {
			UpdateWith_Vectors();
		}

	}


	private void UpdateWith_5Points() {
		Vector4 delta14 = GetDelta(points[1], points[4]);
		Vector4 delta12 = GetDelta(points[1], points[2]);
		Vector4 delta34 = GetDelta(points[3], points[4]);
		Vector4 delta32 = GetDelta(points[3], points[2]);
		Vector4 delta41 = GetDelta(points[4], points[1]);
		Vector4 delta43 = GetDelta(points[4], points[3]);
		Vector4 delta10 = GetDelta(points[1], points[0]);
		Vector4 delta30 = GetDelta(points[3], points[0]);
		weights[1] = GenerateWeight(-9.0f, -3.0f, 9.0f, -1.0f, delta14, delta12, delta34, delta32, delta41, delta10, delta43, delta30);
		weights[2] = GenerateWeight(-1.0f, -3.0f, -3.0f, -1.0f, delta12, delta14, delta32, delta34, delta12, delta10, delta32, delta30);

		curve = GenerateCurve();
		LM.UpdateData (Convert_Vec4_toVec3(curve));
	}

	private void UpdateWith_Vectors() {
		//vectors[0] = transform.TransformPoint(vectors[0]);
		//vectors[1] = transform.TransformPoint(vectors[1]);
		vecRot1 =  vectors[0] - points[0];
		vecRot2 = vectors[1]- points.Last();

		Quaternion rotation = Quaternion.LookRotation(vecRot1);
		begVectorArrow.transform.rotation = rotation;
		begVectorArrow.transform.position = points[0];
		rotation = Quaternion.LookRotation(vecRot2);
		endVectorArrow.transform.rotation = rotation;
		endVectorArrow.transform.position = points.Last();
		
		Vector4 delta40 = GetDelta(points[4], points[0]);
		Vector4 delta20 = GetDelta(points[2], points[0]);
		Vector4 delta24 = GetDelta(points[2], points[4]);

		weights[1] = GenerateWeight(-8.0f, -2.0f, 16.0f, -1.0f, delta40, delta20, vectors[1], delta24, delta40, vectors[0], vectors[1], delta40);
		weights[2] = GenerateWeight(-1.0f, -4.0f, 4.0f, 1.0f, delta20, delta40, delta24, vectors[1], delta20, vectors[0], delta24, delta40);

		curve = GenerateCurve();
		LM.UpdateData (Convert_Vec4_toVec3(curve));
	}

	protected List<GameObject> Create_FirstPointList (string tag) {
		//GameObject[] initPoints = GameObject.FindGameObjectsWithTag(tag).OrderBy (go => go.name).ToArray ();
		//return initPoints.ToList();
		List<GameObject> pointList = new List<GameObject> ();
		Transform parentTrans = this.gameObject.transform;
		foreach (Transform child in parentTrans) {
			if ( child.tag == tag) {
				GameObject point = child.gameObject;
				pointList.Add (point);
			}
		}
		return pointList;
	}

	public Vector4 GetDelta(Vector4 iPoint, Vector4 jPoint) {
		return iPoint-jPoint;
	}
	
	public Vector4 GenerateWeight(float kof11, float kof12, float kof21, float kof22, Vector4 delta1, Vector4 delta2, Vector4 delta3, Vector4 delta4, Vector4 delta5, Vector4 delta6, Vector4 delta7, Vector4 delta8) {
		Vector4 invDelta1 = H.Invers(delta1);
		Vector4 invDelta3 = H.Invers(delta3);
		Vector4 invDelta5 = H.Invers(delta5);
		Vector4 invDelta7 = H.Invers(delta7);

		Vector4 firstPart = kof11 * H.Mult(invDelta1, delta2) + kof12 * H.Mult(invDelta3, delta4);
		Vector4 secPart = kof21 * H.Mult(invDelta5, delta6) + kof22 * H.Mult(invDelta7, delta8);

		Vector4 weight = H.Mult(H.Invers(firstPart), secPart);
		weight = H.Mult(weight, weights[0]);
		return weight;
	}

	protected List<float> getTimeList() {
		float deltaStep = TList.Last() / resolution;
		// Kaupiasi paklaida!!!!!!!!!!!!!
		List<float> timeList = new List<float>();
		float t = 0.0f;
		//while(t <= TList.Last()) {
		for (int i=0; i<resolution+1; i++) {
			timeList.Add(t);
			t += deltaStep;
		}
		print(timeList.Last());
		return timeList;
	}

	public List<float> getFiList(float t) { // kreives funkcija
		// fi(t) = MultLoop(k!=i): t - t[k]
		List<float> fList = new List<float>();
		for (int i = TList.Count-2; i >= 0; i--) {
			for (int j = TList.Count-1; j >= 0; j--) {
				if (i < j) {
					fList.Add((t-TList[i])*(t-TList[j]));
					//print("i: "+i+" ; j:"+j);
		}	}	}
		//print("##############################");
		return fList;
	}

	public List<Vector4> GenerateCurve() {
		// qt-pwf(t); pt-wf(t);
		// Visis C(t) bus Img(H)
		// gali tekti pasiversti i Vector3 del optimizacijos ir renderinimo
		List<Vector4> curve = new List<Vector4>();
		Vector4 Ft, Wt;
		for (int t=0; t < timeList.Count; t++) {
			Ft = new Vector4(0,0,0,0); // q(t)
			Wt = new Vector4(0,0,0,0); // p(t)
			List<float> fiList = getFiList(timeList[t]);

			for (int i=0; i < TList.Count; i++) {
				//t yra rezoliucijos delta step
				// fiList reiksmes floatai?
				//print ("Var: "+ points[i*2]+" "+weights[i]+" "+fiList[i]);
				Ft += H.Mult(points[i*2], weights[i]) * fiList[i];
				//print(Ft);
				Wt += weights[i] * fiList[i];
				//print(Wt);
			}
			//print (Ft+" Ir "+Wt);
			curve.Add( H.Mult(Ft, H.Invers(Wt)) );
			Vector4[] cDH = {Ft, Wt};
			cDH_List.Add(cDH);
		}
		return curve;
	}

	public Vector4 Displacement(Vector4[] c, Vector4 x) {
		// c=[p;q]; x=[0, x,y,z]
		// c:x -> 
		Vector4 newPoint = H.Mult( H.Mult(c[1], x) - 1.0f*c[1], H.Invers(c[1]) );
		//Vector4 newPoint = H.Mult(c[0], H.Invers(c[1]));
		return newPoint;
	}

	public Vector4[] MobiusTransformation(Vector4 q1, Vector4 q2, Vector4 a, Vector4 b, Vector4 c, Vector4 d) {
		Vector4 h0 = H.Mult(a, q1) + H.Mult(b, q2);
		Vector4 h1 = H.Mult(c, q1) + H.Mult(d, q2);
		Vector4[] DH = {h0, h1};
		return DH;
	}

	protected List<Vector3> FromGO_toVectorList(List<GameObject> GoList) {
		List<Vector3> vecList = new List<Vector3>();
		foreach (GameObject GO in GoList) {
			vecList.Add(GO.transform.position);
		}
		return vecList;
	}

	protected List<Vector4> Convert_Vec3_toVec4(List<Vector3> vec3List) {
		List<Vector4> vec4List = new List<Vector4>();
		foreach (Vector3 vec3 in vec3List ) {
			vec4List.Add(vec3);
		}
		return vec4List;
	}

	protected List<Vector3> Convert_Vec4_toVec3(List<Vector4> vec4List) {
		List<Vector3> vec3List = new List<Vector3>();
		foreach (Vector4 vec4 in vec4List ) {
			vec3List.Add(vec4);
		}
		return vec3List;
	}



}



 */
