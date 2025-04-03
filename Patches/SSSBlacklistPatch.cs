using HarmonyLib;

namespace PolishedPlushiesAndSillyScrap.Patches
{

    public class SSSBlacklistPatch
    {
        public static void DoPatching()
        {
            PolishedPlushiesAndSillyScrap.Harmony?.Patch(AccessTools.Method(AccessTools.TypeByName("SelfSortingStorage.Config"), "BlacklistValidation"), postfix: new HarmonyMethod(typeof(SSSConfig).GetMethod("ConfigOverride")));
        }
    }

    public class SSSConfig
    {
        public static void ConfigOverride(GameNetcodeStuff.PlayerControllerB player, ref bool __result)
        {
            var item = player.currentlyHeldObjectServer;
            if (item != null && item.itemProperties.itemName == "Nuko")
            {
                __result = false;
            }
        }
    }
}
