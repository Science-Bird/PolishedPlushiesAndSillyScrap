using System.ComponentModel;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace PolishedPlushiesAndSillyScrap.Patches
{

    [HarmonyPatch]
    public class ScrapHUDPatch
    {
        public static Dictionary<string, float> overrides = new Dictionary<string, float> { { "Frieren", -0.09f }, { "Freddy", -0.05f }, { "Pou", -0.04f }, { "Pukeko", -0.03f }, { "Miku", -0.06f }, { "Teto", -0.06f }, { "Neco-Arc", -0.06f }, { "Pomni", -0.05f }, { "Rei", -0.06f }, { "Reimu", -0.06f }, { "Seal", 0.09f }, { "Nuko", 0.08f } };
        public static bool[] checkArray = [true, true, true];
        public static bool flag;
        public static string passedName;
        public static int passedIndex;

        [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.DisplayNewScrapFound))]
        [HarmonyPrefix]
        public static void ScrapDisplaySetup(HUDManager __instance)
        {
            if (__instance.itemsToBeDisplayed.Count <= 0)
            {
                return;
            }
            if (__instance.itemsToBeDisplayed[0] == null || __instance.itemsToBeDisplayed[0].itemProperties.spawnPrefab == null)
            {
                return;
            }
            passedIndex = __instance.nextBoxIndex;
            if (__instance.itemsToBeDisplayed[0].gameObject.GetComponent<WeightedNoisemakerProp>() || __instance.itemsToBeDisplayed[0].gameObject.GetComponent<IdleAnimatingProp>())
            {
                flag = true;
                if (__instance.itemsToBeDisplayed[0].itemProperties != null)
                {
                    passedName = __instance.itemsToBeDisplayed[0].itemProperties.spawnPrefab.name;
                }
            }
        }

        [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.DisplayNewScrapFound))]
        [HarmonyPostfix]
        public static void ScrapDisplayPatch(HUDManager __instance)
        {
            if (!flag || passedName == null || !overrides.ContainsKey(passedName))
            {
                checkArray[passedIndex] = false;
                return;
            }
            GameObject parent = __instance.ScrapItemBoxes[passedIndex].itemObjectContainer.gameObject;
            GameObject obj = GameObject.Find($"{parent.name}/{passedName}(Clone)");
            if (obj != null)
            {
                if (overrides.TryGetValue(passedName, out float value))
                {
                    if (checkArray[passedIndex])
                    {
                        obj.transform.position += new Vector3(0f, value, 0f);
                    }
                    else
                    {
                        obj.transform.position += new Vector3(0f, value/10f, 0f);
                    }
                    checkArray[passedIndex] = false;
                }
            }
        }
    }
}
