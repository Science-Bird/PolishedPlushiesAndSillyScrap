using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace PolishedPlushiesAndSillyScrap.Patches
{

    [HarmonyPatch]
    public class RandomResetPatch
    {
        public static bool initialCheck = false;

        public static void ResetRandoms(int seed)
        {
            IdleAnimatingProp[] idleProps = Object.FindObjectsOfType<IdleAnimatingProp>();
            WeightedNoisemakerProp[] weightedProps = Object.FindObjectsOfType<WeightedNoisemakerProp>();
            foreach (IdleAnimatingProp idleProp in idleProps)
            {
                NetworkObject networkObj = idleProp.gameObject.GetComponent<NetworkObject>();
                if (networkObj != null)
                {
                    //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{idleProp.itemProperties.itemName} - Network ID: {(int)networkObj.NetworkObjectId}");
                    //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{idleProp.itemProperties.itemName} - Using seed: {StartOfRound.Instance.randomMapSeed + 85 + (int)networkObj.NetworkObjectId}");
                    idleProp.animationRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 85 + (int)networkObj.NetworkObjectId);
                }
                else
                {
                    //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{idleProp.itemProperties.itemName} - No network object. Using seed: {StartOfRound.Instance.randomMapSeed + 85}");
                    idleProp.animationRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 85);
                }
                idleProp.currentInterval = 0f;
                idleProp.lastIntervalTime = 0f;
            }
            foreach (WeightedNoisemakerProp weightedProp in weightedProps)
            {
                NetworkObject networkObj = weightedProp.gameObject.GetComponent<NetworkObject>();
                if (networkObj != null)
                {
                    //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{weightedProp.itemProperties.itemName} - Network ID: {(int)networkObj.NetworkObjectId}");
                    //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{weightedProp.itemProperties.itemName} - Using seed: {StartOfRound.Instance.randomMapSeed + 85 + (int)networkObj.NetworkObjectId}");
                    weightedProp.noisemakerRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 85 + (int)networkObj.NetworkObjectId);
                }
                else
                {
                    //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{weightedProp.itemProperties.itemName} - No network object. Using seed: {StartOfRound.Instance.randomMapSeed + 85}");
                    weightedProp.noisemakerRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 85);
                }
            }
        }

        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.SyncScrapValuesClientRpc))]
        [HarmonyPostfix]
        public static void OnScrapSync(RoundManager __instance)
        {
            ResetRandoms(StartOfRound.Instance.randomMapSeed);
        }

        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnPlayerConnectedClientRpc))]
        [HarmonyPostfix]
        public static void OnConnect(StartOfRound __instance)
        {
            initialCheck = true;
        }

        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.Update))]
        [HarmonyPostfix]
        public static void OnUpdate(StartOfRound __instance)
        {
            if (initialCheck)
            {
                ResetRandoms(__instance.randomMapSeed);
                initialCheck = false;
            }
        }
    }
}
