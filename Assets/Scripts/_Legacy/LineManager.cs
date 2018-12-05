using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LineManager : MonoBehaviour {

	public Color curveColor;
	public float curveWide = 1.0f;

	private GameObject kreive;
	private List<GameObject> kreives;
	//private List<GameObject> polygonList;
	private List<GameObject> curvePoint_List;

	void Awake () {
		Init();
		//print("Kreive init: "+ gameObject.transform.name +" "+ kreive);
	}

	public void Init() {
		kreives = new List<GameObject>();
	}

	public void AddCurve() {
		kreive = new GameObject ();
		//kreive.transform.parent = gameObject.transform;
		kreive.name = "kreive";
		AddLineRenderer (kreive, curveWide, curveColor);
		kreives.Add(kreive);
		//SetPolygonPoints (kreive, pointList);
	}

	public void AddManyCurves(int amount) {
		for  (int i = 0; i < amount; i++) {
			AddCurve();
		}
	}

	// Local privates Methods:
	private void AddLineRenderer (GameObject polygon, float widness, Color color) {
		LineRenderer lineRenderer = polygon.AddComponent<LineRenderer> ();
		lineRenderer.material = new Material(Shader.Find ("Particles/Additive"));
		// A simple 2 color gradient with a fixed alpha of 1.0f.
		float alpha = 1.0f;
		Gradient gradient = new Gradient ();
		gradient.SetKeys (
			new GradientColorKey[] { new GradientColorKey (color, 0.0f), new GradientColorKey (color, 1.0f) },
			new GradientAlphaKey[] { new GradientAlphaKey (alpha, 0.0f), new GradientAlphaKey (alpha, 1.0f) }
		);
		lineRenderer.colorGradient = gradient;

		lineRenderer.widthMultiplier = widness;
		lineRenderer.SetPosition (1, new Vector3 (0, 0, 0));
	}

	private void SetPolygonPoints (GameObject polygon, List<Vector3> pointList) {
		//print(polygon);
		LineRenderer lineRenderer = polygon.GetComponent<LineRenderer> ();
		lineRenderer.positionCount = pointList.Count;
		for (int i = 0; i < pointList.Count; i++) {
			lineRenderer.SetPosition (i, pointList[i]);
		}
	}

	public void UpdateCurve (List<Vector3> curvePoints) {
		UpdateCurve (0, curvePoints);
	}

	
	public void UpdateCurve (int curveIndx, List<Vector3> curvePoints) {
		SetPolygonPoints (kreives[curveIndx], curvePoints);
	}

	public void UpdateAllCurves (List<List<Vector3>> curvesList) {

		//print(kreives.Count);
		for (int i = 0; i < kreives.Count; i++) {
			//print(curvesList[i].Count);
			UpdateCurve (i, curvesList[i]);
		}
	}

}