using System.ComponentModel;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace PolishedPlushiesAndSillyScrap.Patches
{

    [HarmonyPatch]
    public class MeshVariantsPatch
    {
        public static System.Random AltMeshRandom;

        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.SyncScrapValuesClientRpc))]
        [HarmonyPostfix]
        public static void AlternateMeshHandling(RoundManager __instance, NetworkObjectReference[] spawnedScrap)
        {
            AltMeshRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 210);
            WeightedNoisemakerProp[] specialProps = Object.FindObjectsOfType<WeightedNoisemakerProp>();
            foreach (WeightedNoisemakerProp prop in specialProps)
            {
                if (prop.altMeshVariants.Length > 0 && prop.targetFilter != null && !prop.scrapPersistedThroughRounds && !prop.isInShipRoom && !prop.isInElevator && !prop.isHeld && prop.isInFactory)
                {
                    int randomIndex = AltMeshRandom.Next(0, prop.altMeshVariants.Length);
                    prop.targetFilter.mesh = prop.altMeshVariants[randomIndex];
                    if (randomIndex < prop.altMaterialVariants.Length && prop.targetRenderer != null)
                    {
                        prop.targetRenderer.sharedMaterial = prop.altMaterialVariants[randomIndex];
                    }
                }
            }
        }

    }
}
