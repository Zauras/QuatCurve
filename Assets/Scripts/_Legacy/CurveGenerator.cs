using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CurveGenerator : MonoBehaviour {

	protected Lib_QuaternionAritmetics H;
	//public GameObject pointPrefab;
	public Vector4 weight0;
	public List<GameObject> CPList;
	public List<Vector4> points;
	public List<Vector4> weights;
	public List<Vector4> curve;

	public List<Vector4[]> cDH_List;
	public List<float> TList;
	public float resolution = 70f;
	List<float> timeList;
	protected LineManager LM;


	protected void Initialization() {
		H = new Lib_QuaternionAritmetics();
		weights = new List<Vector4>(){ new Vector4(0, 0, 0, 1.0f) };

		cDH_List = new List<Vector4[]>();

		TList = new List<float>() {0.0f, 2.0f, 4.0f};
		timeList = getTimeList();

		CPList = Create_FirstPointList("Point");
		points = Convert_Vec3_toVec4(FromGO_toVectorList(CPList));

		LM = GetComponent<LineManager>();
		LM.AddCurve();
	}

	protected void StartGenerator() {
		curve = GenerateCurve();
		print(cDH_List);
		LM.UpdateCurve (Convert_Vec4_toVec3(curve));
		//MakeVexPaths();
		print("Curve.Count: "+ curve.Count);
		
	}


	//!!!!!!!! MAKE IT flat -> from x2for to x1for and make it parallelible (is it possible?)
	// make sense first make as a JobFor, second - Execute() stuff because for is not an independent
	public List<Vector4> GenerateCurve() {
		// qt-pwf(t); pt-wf(t);
		// Visis C(t) bus Img(H)
		List<Vector4> curve = new List<Vector4>();
		Vector4 Ft, Wt;
		for (int t=0; t < timeList.Count; t++) {
			Ft = new Vector4(0,0,0,0); // q(t)
			Wt = new Vector4(0,0,0,0); // p(t)

			List<float> fiList = getFiList(timeList[t]);
			
			for (int i=0; i < TList.Count; i++) {
				//t yra rezoliucijos delta step
				if (points.Count == 5) 
					 Ft += H.Mult(points[i*2], weights[i]) * fiList[i];
				else Ft += H.Mult(points[i], weights[i]) * fiList[i];
				Wt += weights[i] * fiList[i];
			}
			//print(Wt+" "+Ft);

			// Su sitais kazkaip veike:
			/*
			if (true) {
				Vector4[] cDH_Mesh = {Ft, H.Invers(Wt)};
				Vector4[] cDH_Obj = {Wt,Ft};
				cDH_List_Mesh.Add(cDH_Mesh);
				cDH_List_Obj.Add(cDH_Obj);
			}
			*/
			Vector4[] cDH = {Wt,Ft};
			cDH_List.Add(cDH);

			//Vector4[] cDH = {Wt,Ft};
			curve.Add( H.Mult(Ft, H.Invers(Wt)) );
			//curve.Add(Displacement(cDH, new Vector4(0,0,0,0)));
		}
		return curve;
	}


	public Vector4 GetDelta(Vector4 iPoint, Vector4 jPoint) {
		return iPoint-jPoint;
	}
	
	public Vector4 GenerateWeight(float kof11, float kof12, float kof21, float kof22,
								 Vector4 delta1, Vector4 delta2, Vector4 delta3, Vector4 delta4,
								  Vector4 delta5, Vector4 delta6, Vector4 delta7, Vector4 delta8) {
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
		// Darant Spline - kaupiasi paklaida!
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
		return fList;
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


/*
	IEnumerator Wait_aBit()
	{  yield return new WaitForSeconds(0.2f); }


	
	public Vector4[] MobiusTransformation(Vector4 q1, Vector4 q2, Vector4 a, Vector4 b, Vector4 c, Vector4 d) {
		Vector4 h0 = H.Mult(a, q1) + H.Mult(b, q2);
		Vector4 h1 = H.Mult(c, q1) + H.Mult(d, q2);
		Vector4[] DH = {h0, h1};
		return DH;
	}
	 */
