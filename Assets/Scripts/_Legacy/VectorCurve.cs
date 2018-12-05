using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VectorCurve : CurveGenerator {
	public GameObject vectorPrefab;
	public List<GameObject> VecPointers;
	GameObject begVectorArrow, endVectorArrow;
	Vector3 vecRot1, vecRot2;
	List<Vector4> vectors;

	// Use this for initialization
	void Start (){
		base.Initialization();
			Vector4 delta40 = GetDelta(points[2], points[0]);
			Vector4 delta20 = GetDelta(points[1], points[0]);
			Vector4 delta24 = GetDelta(points[1], points[2]);
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

		base.StartGenerator();
		
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{	// Check if control points changed
		List<Vector4> pointsNew = Convert_Vec3_toVec4(FromGO_toVectorList(CPList));
		bool pointsChanged = false;

		for (int i = 0; i < pointsNew.Count; i++) {
			pointsChanged = points[i] != pointsNew[i];
			if (pointsChanged == true) {
				points = pointsNew;
				break;
			}
		}
		bool vecChanged = false;
			List<Vector4> vectorsNew = Convert_Vec3_toVec4(FromGO_toVectorList(VecPointers));
			vecChanged = (vectorsNew[0] != vectors[0]) || (vectorsNew[1] != vectors[1]);
			if (vecChanged) vectors = vectorsNew;

		if (pointsChanged || vecChanged) {

			vecRot1 =  vectors[0] - points[0];
			vecRot2 = vectors[1]- points.Last();

			Quaternion rotation = Quaternion.LookRotation(vecRot1);
			begVectorArrow.transform.rotation = rotation;
			begVectorArrow.transform.position = points[0];
			rotation = Quaternion.LookRotation(vecRot2);
			endVectorArrow.transform.rotation = rotation;
			endVectorArrow.transform.position = points.Last();
			
			Vector4 delta40 = GetDelta(points[2], points[0]);
			Vector4 delta20 = GetDelta(points[1], points[0]);
			Vector4 delta24 = GetDelta(points[1], points[2]);

			weights[1] = GenerateWeight(-8.0f, -2.0f, 16.0f, -1.0f, delta40, delta20, vectors[1], delta24, delta40, vectors[0], vectors[1], delta40);
			weights[2] = GenerateWeight(-1.0f, -4.0f, 4.0f, 1.0f, delta20, delta40, delta24, vectors[1], delta20, vectors[0], delta24, delta40);

			curve = GenerateCurve();
			LM.UpdateCurve (Convert_Vec4_toVec3(curve));
		}


		
	}
}
