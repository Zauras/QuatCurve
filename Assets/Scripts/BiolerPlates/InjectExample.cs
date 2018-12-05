using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;

/// For Entities (Pure ECS)
/*
public class TransformSystem : ComponentSystem
{

    struct Group
    {
        public readonly int Length;
        public EntityArray entities;
        public ComponentDataArray<TestComponent> test;

    }

    [Inject] Group group;

    protected override void OnUpdate()
    {
     
        for (int i = 0; i < group.Length; i++)
        {
            Debug.Log(group.test[i].Value);
        }



    }
}
*/