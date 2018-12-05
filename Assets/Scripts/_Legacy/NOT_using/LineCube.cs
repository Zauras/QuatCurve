using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LineCube : MonoBehaviour {

	public float length = 1f;

	 // Vexters (Lower/Upper, Front/Back, Right/Left)
	public Vector3 LFR, LFL, LBR, LBL, UFR, UFL, UBR, UBL;
	protected LineManager LM;
	List<Vector3> cube;

	void Start() {
		LM = GetComponent<LineManager>();
		CalcVexters();
		UpdateCube();
	}


	private void CalcVexters() { // Init pagal LFR (Lower Front Right vex)
		LFR = gameObject.transform.position;
		LFL = new Vector3( LFR.x-length, LFR.y		 , LFR.z		);
		LBR = new Vector3( LFR.x       , LFR.y		 , LFR.z-length );
		LBL = new Vector3( LFR.x-length, LFR.y 		 , LFR.z-length );	
		UFR = new Vector3( LFR.x       , LFR.y+length, LFR.z		);
		UFL = new Vector3( LFR.x-length, LFR.y+length, LFR.z		);
		UBR = new Vector3( LFR.x 	   , LFR.y+length, LFR.z-length );
		UBL = new Vector3( LFR.x-length, LFR.y+length, LFR.z-length );
	}

	public void UpdateCube() {
		cube = new List<Vector3>() { LFR, LFL, LBL, LBR, LFR,
										UFR, UFL, UBL, UBR, UFR,
										UFL, LFL, LBL, UBL, UBR, LBR
									};
		LM.UpdateCurve (cube);
	}

	void FixedUpdate() {
		UpdateCube();
	}


	

}
