using GameNetcodeStuff;
using HarmonyLib;
using SelfSortingStorage.Cupboard;

namespace PolishedPlushiesAndSillyScrap.Patches
{

    public class SSSBlacklist
    {
        public static void AddBlacklistTrigger()
        {
            SmartCupboard.AddTriggerValidation(NukoBlacklist, "[Nuko can't fit]");
        }

        private static bool NukoBlacklist(PlayerControllerB player)
        {
            GrabbableObject item = player.currentlyHeldObjectServer;
            if (item != null && item.itemProperties.itemName == "Nuko")
            {
                return false;
            }
            return true;
        }
    }
}
