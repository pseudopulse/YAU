using RoR2.Navigation;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;

namespace YAU.Helpers {
    public class VectorUtils {
        ///<summary>Returns a list of all safe nodes within the specified radius</summary>
        ///<param name="center">the center position</param>
        ///<param name="distance">the max distance</param>
        ///<returns>the list of positions</returns>
        public static Vector3[] GetSafePositionsWithinDistance(Vector3 center, float distance) {
            if (SceneInfo.instance && SceneInfo.instance.groundNodes) {
                NodeGraph graph = SceneInfo.instance.groundNodes;
                List<Vector3> valid = new();
                foreach (NodeGraph.Node node in graph.nodes) {
                    if (Vector3.Distance(node.position, center) <= distance) {
                        valid.Add(node.position);
                    }
                }
                return valid.ToArray();
            }
            else {
                return new Vector3[] { center };
            }
        }

        ///<summary>Returns the Forward vector that would make an object face another object</summary>
        ///<param name="self">the object that you want to look from</param>
        ///<param name="target">the object you want to look at</param>
        public static Vector3 FindLookRotation(GameObject self, GameObject target) {
            return (target.transform.position - self.transform.position).normalized;
        }
    }
}