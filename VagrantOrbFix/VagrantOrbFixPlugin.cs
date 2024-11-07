using BepInEx;
using RoR2;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace VagrantOrbFix
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class VagrantOrbFixPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Gorakh";
        public const string PluginName = "VagrantOrbFix";
        public const string PluginVersion = "1.0.0";

        internal static VagrantOrbFixPlugin Instance { get; private set; }

        void Awake()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            Instance = SingletonHelper.Assign(Instance, this);

            Log.Init(Logger);

            AsyncOperationHandle<GameObject> scorchlingBombLoad = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Scorchling/ScorchlingBombProjectile.prefab");
            scorchlingBombLoad.Completed += handle =>
            {
                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    Log.Fatal("Failed to load ScorchlingBombProjectile prefab");
                    return;
                }

                GameObject prefab = handle.Result;

                Transform model = prefab.transform.Find("Model");
                if (!model)
                {
                    Log.Fatal("Failed to find ScorchlingBombProjectile Model");
                    return;
                }

                ModelLocator modelLocator = prefab.GetComponent<ModelLocator>();
                if (!modelLocator)
                {
                    Log.Fatal("ScorchlingBombProjectile is missing ModelLocator");
                }

                modelLocator._modelTransform = model;
            };

            stopwatch.Stop();
            Log.Message_NoCallerPrefix($"Initialized in {stopwatch.Elapsed.TotalMilliseconds:F0}ms");
        }

        void OnDestroy()
        {
            Instance = SingletonHelper.Unassign(Instance, this);
        }
    }
}
