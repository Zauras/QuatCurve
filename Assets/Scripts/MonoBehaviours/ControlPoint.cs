using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Master { 
    public class ControlPoint : MonoBehaviour
    {
        private Vector3 oldPos;
        void Start()
        {
            transform.hasChanged = false;
            oldPos = transform.position;
            InvokeRepeating("SlowUpdate", 0.0f, 0.05f);
        }

        void SlowUpdate()
        {
            if (oldPos != transform.position)
            {
                transform.hasChanged = true;
                oldPos = transform.position;
                //var em = World.Active.GetOrCreateManager<EntityManager>();
                //em.GetComponentObject(SetupEntitiesSystem.)


            }
        }
    }
}

