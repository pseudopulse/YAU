using System.Reflection;
using System.Collections;
using System;
using System.Linq;
using BepInEx.Configuration;
using HG.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace YAU.Attributes {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class StubShadersAttribute : SearchableAttribute {
        
    }

    public class StubbedShadersAttributeHandler {
        [AutoRun]
        internal static void Hook() {
            On.RoR2.RoR2Application.OnLoad += (orig, self) => {
                Initialize();
                return orig(self);
            };
        }
        internal static void Initialize() {
            List<StubShadersAttribute> instances = new();
            SearchableAttribute.GetInstances<StubShadersAttribute>(instances);

            foreach (StubShadersAttribute attr in instances) {
                FieldInfo info = attr.target as FieldInfo;
                if (!info.IsStatic) {
                    continue;
                }

                object val = info.GetValue(null);
                if (val is AssetBundle) {
                    AssetBundle bundle = val as AssetBundle;
                    foreach (Material mat in bundle.LoadAllAssets<Material>()) {
                        Shader shader = mat.shader.name switch {
                            "StubbedShader/deferred/hgstandard" => Assets.Shader.HGStandard,
                            "StubbedShader/fx/hgcloudremap" => Assets.Shader.HGCloudRemap,
                            "StubbedShader/fx/hgintersectioncloudremap" => Assets.Shader.HGIntersectionCloudRemap,
                            _ => null
                        };

                        if (shader) {
                            mat.shader = shader;
                        }
                    }
                }
            }
        }
    }
}