using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using PolishedPlushiesAndSillyScrap.Patches;

namespace PolishedPlushiesAndSillyScrap
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency(LethalLib.Plugin.ModGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("zigzag.SelfSortingStorage", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.SoftDependency)]
    public class PolishedPlushiesAndSillyScrap : BaseUnityPlugin
    {
        public static ConfigEntry<int> frierenSFX1;
        public static ConfigEntry<int> frierenSFX2;
        public static ConfigEntry<int> pomniSFX1;
        public static ConfigEntry<int> pomniSFX2;
        public static ConfigEntry<int> freddySFX1;
        public static ConfigEntry<int> freddySFX2;
        public static ConfigEntry<int> freddySFX3;
        public static ConfigEntry<int> reimuSFX1;
        public static ConfigEntry<int> reimuSFX2;
        public static ConfigEntry<int> blahajSFX1;
        public static ConfigEntry<int> blahajSFX2;
        public static ConfigEntry<int> pouSFX;
        public static ConfigEntry<int> reiSFX;
        public static ConfigEntry<int> necoSFX1;
        public static ConfigEntry<int> necoSFX2;
        public static ConfigEntry<int> necoSFX3;
        public static ConfigEntry<int> necoSFX4;
        public static ConfigEntry<int> necoSFX5;
        public static ConfigEntry<int> necoSFX6;
        public static ConfigEntry<int> necoSFX7;
        public static ConfigEntry<int> necoSFX8;
        public static ConfigEntry<int> necoSFX9;
        public static ConfigEntry<int> bocchiSFX;
        public static ConfigEntry<int> mikuSFX1;
        public static ConfigEntry<int> mikuSFX2;
        public static ConfigEntry<int> mikuSFX3;
        public static ConfigEntry<int> tetoSFX1;
        public static ConfigEntry<int> tetoSFX2;
        public static ConfigEntry<int> pukekoSFX;
        public static ConfigEntry<int> nikoSFX1;
        public static ConfigEntry<int> nikoSFX2;
        public static ConfigEntry<int> nikoSFX3;

        public static ConfigEntry<bool> nukoTransmitMore;
        public static ConfigEntry<float> nukoAnimFrequency;
        public static ConfigEntry<float> nukoAnimConsistency;
        public static ConfigEntry<bool> sealAttractDogs;
        public static ConfigEntry<float> sealAnimFrequency;
        public static ConfigEntry<float> sealAnimConsistency;

        public static ConfigEntry<bool> onlyLoadEnabled;

        public static List<ConfigEntry<bool>> enablePlushies = new List<ConfigEntry<bool>>();
        public static List<ConfigEntry<int>> plushiesSpawn = new List<ConfigEntry<int>>();

        public static (string,int)[] plushList = [("Frieren",6), ("Pomni",6), ("Freddy",6), ("Reimu",5), ("Blahaj",6),("Pou",4),("Rei",4),("Neco-Arc",4),("Mendako Bocchi",5),("Miku",5),("Teto",5),("Pukeko chick",3),("Niko",1)];

        public static Dictionary<string, int[]> plushDict = new Dictionary<string, int[]>();

        public static PolishedPlushiesAndSillyScrap Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }

        public static PluginInfo pluginInfo;
        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;
            pluginInfo = Info;

            onlyLoadEnabled = base.Config.Bind("!General", "Only Load Enabled", false, "By default, disabling a plushie only sets it to not spawn on any moons, rather than removing it entirely (to prevent ID shifting or other issues when changing mod config). If you need to remove an item entirely by never loading it in the first place (e.g. for a name conflict), enable this setting.");

            for (var i = 0; i < plushList.Length; i++)
            {
                enablePlushies.Add(base.Config.Bind(plushList[i].Item1, $"Enable {plushList[i].Item1}", true, $"Spawn the {plushList[i].Item1} plush"));
                plushiesSpawn.Add(base.Config.Bind(plushList[i].Item1, $"{plushList[i].Item1} Rarity", plushList[i].Item2, new ConfigDescription($"How rare {plushList[i].Item1} should be (lower number means more rare). A common vanilla item will have a value ~15-30.", new AcceptableValueRange<int>(1, 300))));
            }
            enablePlushies.Add(base.Config.Bind("Nuko", "Enable Nuko", true, "Spawn Nuko"));
            plushiesSpawn.Add(base.Config.Bind("Nuko", "Nuko Rarity", 2, new ConfigDescription("How rare Nuko should be (lower number means more rare). A common vanilla item will have a value ~15-30.", new AcceptableValueRange<int>(1, 300))));
            enablePlushies.Add(base.Config.Bind("Seal", "Enable Seal", true, "Spawn the Seal"));
            plushiesSpawn.Add(base.Config.Bind("Seal", "Seal Rarity", 3, new ConfigDescription("How rare the Seal should be (lower number means more rare). A common vanilla item will have a value ~15-30.", new AcceptableValueRange<int>(1, 300))));

            frierenSFX1 = base.Config.Bind("Frieren", "Sound 1 Chance", 5, new ConfigDescription("Percent chance of triggering Frieren's sound effect 1 when squeezed.", new AcceptableValueRange<int>(0, 100)));
            frierenSFX2 = base.Config.Bind("Frieren", "Sound 2 Chance", 5, new ConfigDescription("Percent chance of triggering Frieren's sound effect 2 when squeezed.", new AcceptableValueRange<int>(0, 100)));

            pomniSFX1 = base.Config.Bind("Pomni", "Sound 1 Chance", 7, new ConfigDescription("Percent chance of triggering Pomni's sound effect 1 when squeezed.", new AcceptableValueRange<int>(0, 100)));
            pomniSFX2 = base.Config.Bind("Pomni", "Sound 2 Chance", 3, new ConfigDescription("Percent chance of triggering Pomni's sound effect 2 when squeezed.", new AcceptableValueRange<int>(0, 100)));

            freddySFX1 = base.Config.Bind("Freddy", "Sound 1 Chance", 7, new ConfigDescription("Percent chance of triggering Freddy's sound effect 1 when squeezed.", new AcceptableValueRange<int>(0, 100)));
            freddySFX2 = base.Config.Bind("Freddy", "Sound 2 Chance", 7, new ConfigDescription("Percent chance of triggering Freddy's sound effect 2 when squeezed (slight variant of sound effect 1).", new AcceptableValueRange<int>(0, 100)));
            freddySFX3 = base.Config.Bind("Freddy", "Sound 3 Chance", 3, new ConfigDescription("Percent chance of triggering Freddy's sound effect 3 when squeezed.", new AcceptableValueRange<int>(0, 100)));

            reimuSFX1 = base.Config.Bind("Reimu", "Sound 1 Chance", 4, new ConfigDescription("Percent chance of triggering Reimu's sound effect 1 when squeezed.", new AcceptableValueRange<int>(0, 100)));
            reimuSFX2 = base.Config.Bind("Reimu", "Sound 2 Chance", 4, new ConfigDescription("Percent chance of triggering Reimu's sound effect 2 when squeezed.", new AcceptableValueRange<int>(0, 100)));

            blahajSFX1 = base.Config.Bind("Blahaj", "Sound 1 Chance", 25, new ConfigDescription("Percent chance of triggering Blahaj's sound effect 1 when squeezed.", new AcceptableValueRange<int>(0, 100)));
            blahajSFX2 = base.Config.Bind("Blahaj", "Sound 2 Chance", 25, new ConfigDescription("Percent chance of triggering Blahaj's sound effect 2 when squeezed (slight variant of sound effect 1).", new AcceptableValueRange<int>(0, 100)));

            pouSFX = base.Config.Bind("Pou", "Sound Chance", 20, new ConfigDescription("Percent chance of triggering Pou's sound effect when squeezed.", new AcceptableValueRange<int>(0, 100)));

            reiSFX = base.Config.Bind("Rei", "Sound Chance", 5, new ConfigDescription("Percent chance of triggering Rei's sound effect when squeezed.", new AcceptableValueRange<int>(0, 100)));

            necoSFX1 = base.Config.Bind("Neco-Arc", "Sound 1 Chance", 25, new ConfigDescription("Percent chance of triggering Neco-Arc's sound effect 1 when squeezed.", new AcceptableValueRange<int>(0, 100)));
            necoSFX2 = base.Config.Bind("Neco-Arc", "Sound 2 Chance", 25, new ConfigDescription("Percent chance of triggering Neco-Arc's sound effect 2 when squeezed.", new AcceptableValueRange<int>(0, 100)));
            necoSFX3 = base.Config.Bind("Neco-Arc", "Sound 3 Chance", 14, new ConfigDescription("Percent chance of triggering Neco-Arc's sound effect 3 when squeezed.", new AcceptableValueRange<int>(0, 100)));
            necoSFX4 = base.Config.Bind("Neco-Arc", "Sound 4 Chance", 9, new ConfigDescription("Percent chance of triggering Neco-Arc's sound effect 4 when squeezed.", new AcceptableValueRange<int>(0, 100)));
            necoSFX5 = base.Config.Bind("Neco-Arc", "Sound 5 Chance", 7, new ConfigDescription("Percent chance of triggering Neco-Arc's sound effect 5 when squeezed.", new AcceptableValueRange<int>(0, 100)));
            necoSFX6 = base.Config.Bind("Neco-Arc", "Sound 6 Chance", 7, new ConfigDescription("Percent chance of triggering Neco-Arc's sound effect 6 when squeezed.", new AcceptableValueRange<int>(0, 100)));
            necoSFX7 = base.Config.Bind("Neco-Arc", "Sound 7 Chance", 7, new ConfigDescription("Percent chance of triggering Neco-Arc's sound effect 7 when squeezed.", new AcceptableValueRange<int>(0, 100)));
            necoSFX8 = base.Config.Bind("Neco-Arc", "Sound 8 Chance", 3, new ConfigDescription("Percent chance of triggering Neco-Arc's sound effect 8 when squeezed.", new AcceptableValueRange<int>(0, 100)));
            necoSFX9 = base.Config.Bind("Neco-Arc", "Sound 9 Chance", 3, new ConfigDescription("Percent chance of triggering Neco-Arc's sound effect 9 when squeezed.", new AcceptableValueRange<int>(0, 100)));

            bocchiSFX = base.Config.Bind("Mendako Bocchi", "Sound Chance", 20, new ConfigDescription("Percent chance of triggering Mendako Bocchi's sound effect when squeezed.", new AcceptableValueRange<int>(0, 100)));

            mikuSFX1 = base.Config.Bind("Miku", "Sound 1 Chance", 7, new ConfigDescription("Percent chance of triggering Miku's sound effect 1 when squeezed.", new AcceptableValueRange<int>(0, 100)));
            mikuSFX2 = base.Config.Bind("Miku", "Sound 2 Chance", 7, new ConfigDescription("Percent chance of triggering Miku's sound effect 2 when squeezed.", new AcceptableValueRange<int>(0, 100)));
            mikuSFX3 = base.Config.Bind("Miku", "Sound 3 Chance", 7, new ConfigDescription("Percent chance of triggering Miku's sound effect 3 when squeezed.", new AcceptableValueRange<int>(0, 100)));

            tetoSFX1 = base.Config.Bind("Teto", "Sound 1 Chance", 15, new ConfigDescription("Percent chance of triggering Teto's sound effect 1 when squeezed.", new AcceptableValueRange<int>(0, 100)));
            tetoSFX2 = base.Config.Bind("Teto", "Sound 2 Chance", 1, new ConfigDescription("Percent chance of triggering Teto's sound effect 2 when squeezed.", new AcceptableValueRange<int>(0, 100)));

            pukekoSFX = base.Config.Bind("Pukeko chick", "Sound Chance", 10, new ConfigDescription("Percent chance of triggering Pukeko chick's sound effect when squeezed.", new AcceptableValueRange<int>(0, 100)));

            nikoSFX1 = base.Config.Bind("Niko", "Sound 1 Chance", 10, new ConfigDescription("Percent chance of triggering Niko's sound effect 1 when squeezed.", new AcceptableValueRange<int>(0, 100)));
            nikoSFX2 = base.Config.Bind("Niko", "Sound 2 Chance", 10, new ConfigDescription("Percent chance of triggering Niko's sound effect 2 when squeezed.", new AcceptableValueRange<int>(0, 100)));
            nikoSFX3 = base.Config.Bind("Niko", "Sound 3 Chance", 10, new ConfigDescription("Percent chance of triggering Niko's sound effect 3 when squeezed.", new AcceptableValueRange<int>(0, 100)));

            nukoTransmitMore = base.Config.Bind("Nuko", "Transmit More Sounds", false, "Transmit sounds over walkie-talkies when Nuko is dropped or grabbed. By default, drop/grab sounds are only played on the ship speaker to avoid checking the map for walkie-talkies every time the scrap is interacted with.");
            nukoAnimFrequency = base.Config.Bind("Nuko", "Animation Frequency", 25f, new ConfigDescription("Maximum time (in seconds) between random idle animations. The interval is rolled randomly, so this just sets the longest possible random interval.", new AcceptableValueRange<float>(10, 200)));
            nukoAnimConsistency = base.Config.Bind("Nuko", "Animation Consistency", 50f, new ConfigDescription("As a percent, how often animations will actually occur on the random interval. The lower this is, animations will be less frequent and more variable.", new AcceptableValueRange<float>(0, 100)));

            sealAttractDogs = base.Config.Bind("Seal", "Heard By Dogs", true, "Eyeless dogs can hear the random noises made by seals. This is similar to mask laughing, and should only have a small effect when the dogs are very close. Grabbing and dropping sounds will always attract dogs regardless of this setting.");
            sealAnimFrequency = base.Config.Bind("Seal", "Animation Frequency", 35f, new ConfigDescription("Maximum time (in seconds) between random idle animations. The interval is rolled randomly, so this just sets the longest possible random interval.", new AcceptableValueRange<float>(10, 200)));
            sealAnimConsistency = base.Config.Bind("Seal", "Animation Consistency", 50f, new ConfigDescription("As a percent, how often animations will actually occur on the random interval. The lower this is, animations will be less frequent and more variable.", new AcceptableValueRange<float>(0, 100)));

            plushDict.TryAdd("Frieren", [frierenSFX1.Value, frierenSFX2.Value]);
            plushDict.TryAdd("Pomni", [pomniSFX1.Value, pomniSFX2.Value]);
            plushDict.TryAdd("Freddy", [freddySFX1.Value, freddySFX2.Value, freddySFX3.Value]);
            plushDict.TryAdd("Reimu", [reimuSFX1.Value, reimuSFX2.Value]);
            plushDict.TryAdd("Blahaj", [blahajSFX1.Value, blahajSFX2.Value]);
            plushDict.TryAdd("Pou", [pouSFX.Value]);
            plushDict.TryAdd("Rei", [reiSFX.Value]);
            plushDict.TryAdd("Neco-Arc", [necoSFX1.Value, necoSFX2.Value, necoSFX3.Value, necoSFX4.Value, necoSFX5.Value, necoSFX6.Value, necoSFX7.Value, necoSFX8.Value, necoSFX9.Value]);
            plushDict.TryAdd("Mendako Bocchi", [bocchiSFX.Value]);
            plushDict.TryAdd("Miku", [mikuSFX1.Value, mikuSFX2.Value, mikuSFX3.Value]);
            plushDict.TryAdd("Teto", [tetoSFX1.Value, tetoSFX2.Value]);
            plushDict.TryAdd("Pukeko chick", [pukekoSFX.Value]);
            plushDict.TryAdd("Niko", [nikoSFX1.Value, nikoSFX2.Value, nikoSFX3.Value]);

            LoadScrap.RegisterScrap();

            Patch();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }

        internal static void Patch()
        {
            Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

            Logger.LogDebug("Patching...");

            bool doLobbyCompat = false;
            bool zigzagPresent = false;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == "BMX.LobbyCompatibility")
                {
                    Logger.LogDebug("Found BMX!");
                    doLobbyCompat = true;
                }
                if (assembly.GetName().Name == "SelfSortingStorage")
                {
                    Logger.LogDebug("Found zigzag!");
                    zigzagPresent = true;
                }
                if (doLobbyCompat && zigzagPresent)
                {
                    break;
                }
            }

            if (doLobbyCompat)
            {
                LobbyCompatibility.RegisterCompatibility();
            }
            if (zigzagPresent)
            {
                SSSBlacklistPatch.DoPatching();
            }

                Harmony.PatchAll();

            Logger.LogDebug("Finished patching!");
        }

        internal static void Unpatch()
        {
            Logger.LogDebug("Unpatching...");

            Harmony?.UnpatchSelf();

            Logger.LogDebug("Finished unpatching!");
        }
    }
}
