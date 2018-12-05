using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Drone : MonoBehaviour {

	Mesh mesh;
	public Vector3[] meshVexArr; // A Copy of real vertecies
	public Dictionary<Vector3, List<int>> vexDic; // dynamicDic <realVex, indexesOfVex_inMeshVexArr> 
	private Dictionary<Vector3, List<int>> initVexDic; // staticDic for reseting Dic


	// Use this for initialization
	void Awake () {
		vexDic = new Dictionary <Vector3, List<int>>();
		initVexDic = new Dictionary <Vector3, List<int>>();
		mesh = GetComponent<MeshFilter>().mesh;
		//meshVexArr = mesh.vertices;
		meshVexArr = new Vector3[mesh.vertices.Length];

		print(meshVexArr.Length + "Length of meshVexArr");

		for(int i=0; i < meshVexArr.Length; i++) {
			meshVexArr[i] = gameObject.transform.TransformVector(mesh.vertices[i]);
			//print(meshVexArr[i]);
		}
		PopulateVexDic();	
	}

	void FixedUpdate() { // geriau daryti tik tada, kada daromas zingsnis
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		//meshVexArr = mesh.vertices;
	}

	private void PopulateVexDic() {

		foreach (Vector3 targetVex in meshVexArr.Distinct()) {
			List<int> repIndx = new List<int>();
			// Gather indexes of targetVex in Mesh:
			for (int i=0; i < meshVexArr.Length; i++) 
				if (meshVexArr[i]==targetVex) repIndx.Add(i);
			vexDic.Add(targetVex, repIndx);
			initVexDic.Add(targetVex, repIndx);
		}
	}

	// BOTTLE-NECK !!!
	public void UpdateAllVex(List<Vector3> newValues) {
		//Vector3[] newArr = new Vector3[meshVexArr.Length];
		Vector3[] localVertCoord = new Vector3[meshVexArr.Length];
		for (int i=0; i < newValues.Count; i++) {
			List<int> indxes = vexDic.Values.ElementAt(i);
			foreach (int ind in indxes) {     //!!! MAKE IT JOB-FOR & flatten DOULBE FOR
				//print(newValues[i]);
				meshVexArr[ind] = newValues[i];
				//print(meshVexArr[ind]);
				localVertCoord[ind] = gameObject.transform.InverseTransformVector(newValues[i]);	
			}
		}
		mesh.vertices = localVertCoord;
		//vexDic = new Dictionary<Vector3, List<int>>();
		//initVexDic = new Dictionary<Vector3, List<int>>();
		//PopulateVexDic();
		//print(vexDic.Count);
	}

    public void UpdateVex(Vector3 target, Vector3 newValue) {
		List<int> indxes = vexDic[target];
		foreach (int i in indxes) {
			meshVexArr[i] = newValue;
		}
		vexDic.Remove(target);
		vexDic.Add(newValue, indxes);
		mesh.vertices = meshVexArr;
	}

	public void ResetMeshVertecies() {
		vexDic = new Dictionary<Vector3, List<int>>();
		Vector3[] newVexArr = new Vector3[meshVexArr.Length];
		foreach (KeyValuePair<Vector3, List<int>> pair in initVexDic) {
			vexDic.Add(pair.Key, pair.Value);
			foreach (int i in pair.Value) newVexArr[i] = pair.Key;
		}
	}

}
