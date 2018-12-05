using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Master
{
	public sealed class LibQuaternionAritmetics
	{
		public static LibQuaternionAritmetics H;


		public static float4 Mult (float4 a, float4 b) {
			float w = (b.w*a.w - b.x*a.x - b.y*a.y - b.z*a.z);
			float x = (b.w*a.x + b.x*a.w - b.y*a.z + b.z*a.y);
			float y = (b.w*a.y + b.x*a.z + b.y*a.w - b.z*a.x);
			float z = (b.w*a.z - b.x*a.y + b.y*a.x + b.z*a.w);
			return new float4 (x, y, z, w);
		}

		public static float4 Div (float4 a, float4 b) { // a/b laikom, kad a = w, x, y, z; Daryti w pradzioje ar gale ???
			float daliklis = (b.w*b.w + b.x*b.x + b.y*b.y + b.z*b.z);

			float w = (b.w*a.w + b.x*a.x + b.y*a.y + b.z*a.z) / daliklis;
			float x = (b.w*a.x - b.x*a.w - b.y*a.z + b.z*a.y) / daliklis;
			float y = (b.w*a.y + b.x*a.z - b.y*a.w - b.z*a.x) / daliklis;
			float z = (b.w*a.z - b.x*a.y + b.y*a.x - b.z*a.w) / daliklis;
			return new float4 (x, y, z, w);
		}

		public static float4 Conjungate(float4 quaternion) {
			float xNeg = quaternion.x * (-1.0f);
			float yNeg = quaternion.y * (-1.0f);
			float zNeg = quaternion.z * (-1.0f);
			return new float4 (xNeg, yNeg, zNeg, quaternion.w);
		}

		public static float4 Abs(float4 quaternion) {
			
			float xx = math.sqrt (quaternion.x * quaternion.x);
			float yy = math.sqrt (quaternion.y * quaternion.y);
			float zz = math.sqrt (quaternion.z * quaternion.z);
			float ww = math.sqrt (quaternion.w * quaternion.w);
			return new float4 (xx, yy, zz, ww);
		}

		public static float4 Rise_2Deg (Vector4 quat) { // arba kitaip Norm()
			float4 risedQuat = new float4(quat.x*quat.x, quat.y*quat.y, quat.z*quat.z, quat.w*quat.w);
			return risedQuat;
		}

		public static float4 Invers(float4 q) { // Quaternion Invers
			float n = q.x*q.x + q.y*q.y + q.z*q.z + q.w*q.w;
			return new float4(-1.0f*q.x/n, -1.0f*q.y/n, -1.0f*q.z/n, q.w/n);
			//Quaternion quat = new Quaternion(q.x, q.y, q.z, q.w);
			//quat = Quaternion.Inverse(quat);
			//return new Vector4(quat.x, quat.y, quat.z, quat.w);
		}

		public static float4 Normalize(float4 quaternion) {
			float4 normalizedQuat = Div(quaternion, Abs(quaternion));
			return normalizedQuat;
		}

		public static quaternion Rotation(quaternion quaternionT, float angle ) {
			//if (angle > 720) { } Apsauga, kad angle == [0,720];
			Vector4 quat = Convert_Quat_toVec4(quaternionT);

			Vector4 normQuat = Normalize(quat);
			float magnitude = quat.sqrMagnitude;
			Vector3 unitQuat = new Vector4 (quat.x / magnitude,
											quat.y / magnitude, 
											quat.z / magnitude);
			Vector3 unitQuatRot = unitQuat * Mathf.Sin(angle/2.0f);
			Vector4 rotationQuat = new Vector4 (unitQuatRot.x, unitQuatRot.y, unitQuatRot.z, Mathf.Cos(angle/2.0f)); // x,y,z,w
		
			Vector4 rotetedQuat = Mult(Mult(rotationQuat, quat), Invers(quat));
			return Convert_Vec4_toQuat(rotetedQuat);;
		}

		public static float4 Convert_Quat_toVec4(quaternion q) {
			return new float4(q.value.x, q.value.y, q.value.z, q.value.w);
		}

		public static quaternion Convert_Vec4_toQuat(float4 vec4) {
			return new quaternion(vec4.x, vec4.y, vec4.z, vec4.w);
		}

	}

}
