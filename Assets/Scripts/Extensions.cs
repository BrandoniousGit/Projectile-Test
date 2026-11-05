using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
    public static class Extensions
    {
        public static void RemoveAtAndDestroy(this List<GameObject> objects, int index)
        {
            GameObject.Destroy(objects[index]);
            objects.RemoveAt(index);
        }

        public static void RemoveAtAndDestroy(this List<Tile> objects, int index)
        {
            GameObject.Destroy(objects[index].gameObject);
            objects.RemoveAt(index);
        }

        public static bool Vec3Approximately(this Vector3 startingVec, Vector3 targetVec)
        {
            return Mathf.Approximately(startingVec.x, targetVec.x) && Mathf.Approximately(startingVec.y, targetVec.y) && Mathf.Approximately(startingVec.z, targetVec.z);
        }
    }
}