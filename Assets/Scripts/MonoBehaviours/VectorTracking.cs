using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;
using Unity.Collections;
using System.Collections.Generic;
using System;

namespace Master
{
    public class VectorTracking : MonoBehaviour
    {
        public Transform vectorPrefab;

        private Transform begVector;
        private Transform begPoint;
        private Transform endPoint;
        private Transform endVector;

        private Transform begVectorArrow;
        private Transform endVectorArrow;

        private Vector3 vecRot1;
        private Vector3 vecRot2;


        private void Start()
        {
            begVector = transform.GetChild(0);
            begPoint = transform.GetChild(1);
            endPoint = transform.GetChild(3);
            endVector = transform.GetChild(4);

            vecRot1 = begVector.position - begPoint.position; // target - shooter = direction
            vecRot2 = endVector.position - endPoint.position;

            //Object Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent);
            begVectorArrow = Instantiate(vectorPrefab, begPoint.position, Quaternion.LookRotation(vecRot1)); //from point to vecPonter
            begVectorArrow.parent = transform;
            begVectorArrow.name = begVectorArrow.name + " Begining";

            endVectorArrow = Instantiate(vectorPrefab, endVector.position, Quaternion.LookRotation(vecRot2));
            endVectorArrow.parent = transform;
            endVectorArrow.name = endVectorArrow.name + " Ending";

            InvokeRepeating("SlowUpdate", 0.0f, 0.1f);
        }
    

        private void SlowUpdate()
        {
            if (begVector.hasChanged || begPoint.hasChanged || endPoint.hasChanged || endVector.hasChanged)
            {
                //Debug.Log("changed");
                vecRot1 = begVector.position - begPoint.position;
                vecRot2 = endVector.position - endPoint.position;

                begVectorArrow.rotation = Quaternion.LookRotation(vecRot1);
                begVectorArrow.position = begPoint.position;
                endVectorArrow.rotation = Quaternion.LookRotation(vecRot2);
                endVectorArrow.position = endPoint.position;

            }
        }




}
}
