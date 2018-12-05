using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FivePointCurve : CurveGenerator {

	void Start (){
		base.Initialization();

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
			//drone.transform.position = points[0];
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
		LM.UpdateCurve (Convert_Vec4_toVec3(curve));

	}
}
