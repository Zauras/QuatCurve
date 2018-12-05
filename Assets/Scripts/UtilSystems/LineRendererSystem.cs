using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;
using Unity.Collections;
using System.Collections.Generic;



namespace Master
{
    public class LineRendererSystem
    {

        private static void AddLineToPathGO(GameObject path)
        {
            //GameObject line = new GameObject("Line");
            //line.transform.SetParent(path.transform);
            AddLineRenderer(path, BootStrap.Settings.lineWide, BootStrap.Settings.lineColor);
            //SetPolygonPoints(path, pointList);
        }

        private static void AddLineRenderer(GameObject lineGO, float widness, Color color)
        {
            LineRenderer lineRenderer = lineGO.AddComponent<LineRenderer>();
            //lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            lineRenderer.material = new Material(BootStrap.Settings.lineRendererMaterial);
            // A simple 2 color gradient with a fixed alpha of 1.0f.
            float alpha = 1.0f;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
            lineRenderer.colorGradient = gradient;

            lineRenderer.widthMultiplier = widness;
            lineRenderer.SetPosition(0, new Vector3(0, 0, 0));
        }



        public static void SetPolygonPoints(int pathID, float3[] pointList)
        {
            GameObject pathGO = Database.ID_GOmap[pathID];
            LineRenderer lineRenderer = pathGO.GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                AddLineToPathGO(pathGO);
                lineRenderer = pathGO.GetComponent<LineRenderer>();
            }

            
            lineRenderer.positionCount = pointList.Length;
            for (int i = 0; i < pointList.Length; i++)
            {
                lineRenderer.SetPosition(i, pointList[i]);
            }
            
        }

    }
}
