using YAU;
using UnityEngine;
using UnityEngine.Networking;
using YAU.Attributes;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;

namespace YAU.AddressableUtils {
    public static class RuntimePrefabManager {
        internal static GameObject PrefabParent;

        [AutoRun]
        internal static void Setup() {
            PrefabParent = new("YAUPrefabParent");
            PrefabParent.SetActive(false);
            GameObject.DontDestroyOnLoad(PrefabParent);
        }

        public static GameObject CreatePrefab(this GameObject gameObject, string name) {
            GameObject clone = GameObject.Instantiate(gameObject, PrefabParent.transform);
            clone.name = name;
            if (clone.GetComponent<NetworkIdentity>()) {
                MakeNetworkPrefab(clone);
            }
            return clone;
        }

        public static void MakeNetworkPrefab(GameObject gameObject) {
            if (!gameObject.GetComponent<NetworkIdentity>()) {
                YAU.ModLogger.LogError("Could not mark " + gameObject.name + " as a Network Prefab as it contains no NetworkIdentity.");
            }

            NetworkIdentity identity = gameObject.GetComponent<NetworkIdentity>();
            NetworkHash128 hash = NetworkHash128.Parse(CreateHash(gameObject, new StackFrame(2)));
            identity.m_AssetId = hash;
            YAU.YAUContent.RegisterNetworkedObject(gameObject);
        }

        internal static string CreateHash(GameObject obj, StackFrame frame) {
            string identifier = obj.name + frame.GetMethod().DeclaringType.Name;
            MD5 hash = MD5.Create();
            byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(identifier));
            StringBuilder builder = new();
            for (int i = 0; i < bytes.Length; i++) {
                builder.Append(bytes[i].ToString("x2"));
            }

            return builder.ToString();
        }
    }
}