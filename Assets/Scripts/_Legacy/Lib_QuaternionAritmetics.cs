using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lib_QuaternionAritmetics : MonoBehaviour {

	void Start() {
		Vector4 a = new Vector4(1, 2, 3, 4);
		Vector4 b = new Vector4(2, 3, 4, 5);

		//print (Mult(a, b));
		//print (Div(a, b));

	}

// this[int] - Access the x, y, z, w components using [0], [1], [2], [3] respectively.

	public Vector4 Mult (Vector4 a, Vector4 b) {
		float w = (b.w*a.w - b.x*a.x - b.y*a.y - b.z*a.z);
		float x = (b.w*a.x + b.x*a.w - b.y*a.z + b.z*a.y);
		float y = (b.w*a.y + b.x*a.z + b.y*a.w - b.z*a.x);
		float z = (b.w*a.z - b.x*a.y + b.y*a.x + b.z*a.w);
		return new Vector4 (x, y, z, w);
	}

	public Vector4 Div (Vector4 a, Vector4 b) { // a/b laikom, kad a = w, x, y, z; Daryti w pradzioje ar gale ???
		float daliklis = (b.w*b.w + b.x*b.x + b.y*b.y + b.z*b.z);

		float w = (b.w*a.w + b.x*a.x + b.y*a.y + b.z*a.z) / daliklis;
		float x = (b.w*a.x - b.x*a.w - b.y*a.z + b.z*a.y) / daliklis;
		float y = (b.w*a.y + b.x*a.z - b.y*a.w - b.z*a.x) / daliklis;
		float z = (b.w*a.z - b.x*a.y + b.y*a.x - b.z*a.w) / daliklis;
		return new Vector4 (x, y, z, w);
	}

	public Vector4 Conjungate(Vector4 quaternion) {
		float xNeg = quaternion.x * (-1.0f);
		float yNeg = quaternion.y * (-1.0f);
		float zNeg = quaternion.z * (-1.0f);
		return new Vector4 (xNeg, yNeg, zNeg, quaternion.w);
	}

	public Vector4 Abs(Vector4 quaternion) {
		float xx = Mathf.Sqrt (quaternion.x * quaternion.x);
		float yy = Mathf.Sqrt (quaternion.y * quaternion.y);
		float zz = Mathf.Sqrt (quaternion.z * quaternion.z);
		float ww = Mathf.Sqrt (quaternion.w * quaternion.w);
		return new Vector4 (xx, yy, zz, ww);
	}

	public Vector4 Rise_2Deg (Vector4 quat) { // arba kitaip Norm()
		Vector4 risedQuat = new Vector4(quat.x*quat.x, quat.y*quat.y, quat.z*quat.z, quat.w*quat.w);
		return risedQuat;
	}

	public Vector4 Invers(Vector4 q) { // Quaternion Invers
		float n = q.x*q.x + q.y*q.y + q.z*q.z + q.w*q.w;
		return new Vector4(-1.0f*q.x/n, -1.0f*q.y/n, -1.0f*q.z/n, q.w/n);
		//Quaternion quat = new Quaternion(q.x, q.y, q.z, q.w);
		//quat = Quaternion.Inverse(quat);
		//return new Vector4(quat.x, quat.y, quat.z, quat.w);
	}

	public Vector4 Normalize(Vector4 quaternion) {
		Vector4 normalizedQuat = Div(quaternion, Abs(quaternion));
		return normalizedQuat;
	}

	public Quaternion Rotation(Quaternion quaternionT, float angle ) {
		//if (angle > 720) { } Apsauga, kad angle == [0,720];
		Vector4 quaternion = Convert_Quat_toVec4(quaternionT);

		Vector4 normQuat = Normalize(quaternion);
		float magnitude = quaternion.sqrMagnitude;
		Vector3 unitQuat = new Vector4 (quaternion.x / magnitude,
										quaternion.y / magnitude, 
										quaternion.z / magnitude);
		Vector3 unitQuatRot = unitQuat * Mathf.Sin(angle/2.0f);
		Vector4 rotationQuat = new Vector4 (unitQuatRot.x, unitQuatRot.y, unitQuatRot.z, Mathf.Cos(angle/2.0f)); // x,y,z,w
		
		Vector4 rotetedQuat = Mult(Mult(rotationQuat, quaternion), Invers(quaternion));
		return Convert_Vec4_toQuat(rotetedQuat);;
	}

	public Vector4 Convert_Quat_toVec4(Quaternion q) {
		return new Vector4(q.x, q.y, q.z, q.w);
	}

	public Quaternion Convert_Vec4_toQuat(Vector4 vec4) {
		return new Quaternion(vec4.x, vec4.y, vec4.z, vec4.w);
	}



}
