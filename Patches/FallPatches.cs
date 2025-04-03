using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace PolishedPlushiesAndSillyScrap.Patches
{

    [HarmonyPatch]
    public class FallPatches
    {
        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.FallWithCurve))]
        [HarmonyPostfix]
        static void DetectLongFall(GrabbableObject __instance)
        {
            if (__instance == null)
            {
                return;
            }
            if (__instance.itemProperties == null)
            {
                return;
            }
            if (__instance.itemProperties.name == "Nuko")
            {
                IdleAnimatingProp animatingProp = __instance.gameObject.GetComponent<IdleAnimatingProp>();
                if (animatingProp != null && !animatingProp.playedFallAudio)
                {
                    if (__instance.startFallingPosition.y - __instance.targetFloorPosition.y > 5f)
                    {
                        animatingProp.playedFallAudio = true;
                        if (animatingProp.SomeWalkiesAreActive())
                        {
                            animatingProp.PlayOnActiveWalkies(animatingProp.specialNukoClip);
                        }
                        if (__instance.isInElevator || __instance.isInShipRoom)
                        {
                            StartOfRound.Instance.speakerAudioSource.PlayOneShot(animatingProp.specialNukoClip);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.GetPhysicsRegionOfDroppedObject))]
        [HarmonyPrefix]
        static void RaycastFix(GrabbableObject __instance, PlayerControllerB playerDropping)
        {
            if (__instance.gameObject.GetComponent<IdleAnimatingProp>())
            {
                if (__instance.itemProperties.verticalOffset != __instance.gameObject.GetComponent<IdleAnimatingProp>().correctVertical)
                {
                    __instance.itemProperties.verticalOffset = __instance.gameObject.GetComponent<IdleAnimatingProp>().correctVertical;
                }
                Transform transform = null;
                RaycastHit hitInfo;
                if (playerDropping != null && __instance.itemProperties.allowDroppingAheadOfPlayer)
                {
                    Ray ray = new Ray(playerDropping.transform.position + Vector3.up * 0.4f, playerDropping.gameplayCamera.transform.forward);
                    Vector3 vector = ((!Physics.Raycast(ray, out hitInfo, 1.7f, 1342179585, QueryTriggerInteraction.Ignore)) ? ray.GetPoint(1.7f) : ray.GetPoint(Mathf.Clamp(hitInfo.distance - 0.3f, 0.01f, 2f)));
                    if (Physics.Raycast(vector, -Vector3.up, out hitInfo, 80f, 1342179585, QueryTriggerInteraction.Ignore))
                    {
                        if (hitInfo.point.y > __instance.transform.position.y)
                        {
                            PolishedPlushiesAndSillyScrap.Logger.LogDebug("Detected bad raycast! Resolving...");
                            __instance.gameObject.transform.position += new Vector3(0, hitInfo.point.y - __instance.transform.position.y + 0.2f, 0);
                        }
                    }
                }
                else
                {
                    Ray ray = new Ray(__instance.transform.position, -Vector3.up);
                    if (Physics.Raycast(ray, out hitInfo, 80f, 1342179585, QueryTriggerInteraction.Ignore))
                    {
                        if (hitInfo.point.y > __instance.transform.position.y)
                        {
                            PolishedPlushiesAndSillyScrap.Logger.LogDebug("Detected bad raycast! Resolving...");
                            __instance.gameObject.transform.position += new Vector3(0, hitInfo.point.y - __instance.transform.position.y + 0.2f, 0);
                        }
                    }
                }
            }

        }
    }
}
