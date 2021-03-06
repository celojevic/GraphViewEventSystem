using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventGraph
{
    public static class EventGraphUtils
    {

        public static void DestroyChildren(this Transform transform)
        {
            if (transform.childCount == 0) return;

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
        }


    }
}