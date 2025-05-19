using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using LethalLib.Modules;
using System.IO;
using static LethalLib.Modules.ContentLoader;
using System;

namespace PolishedPlushiesAndSillyScrap
{
    public class LoadScrap
    {
        public static AssetBundle PlushAssets;
        public static Dictionary<string, GameObject> Prefabs = new Dictionary<string, GameObject>();
        public static string[] contentNames = ["Frieren", "Pomni", "Freddy", "Reimu", "Blahaj", "Pou", "Rei", "NecoArc", "Bocchi", "Miku", "Teto", "Pukeko", "Niko", "Nuko", "Seal"];

        public static void RegisterScrap()
        {
            PlushAssets = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ppassitems"));

            List<CustomContent> content = new List<CustomContent>();
            content.Add(new ScrapItem("Frieren", "Assets/LethalCompany/Mods/PolishedPlushiesAndSillyScrap/Frieren/Frieren.asset", PolishedPlushiesAndSillyScrap.plushiesSpawn[0].Value, PolishedPlushiesAndSillyScrap.enablePlushies[0].Value ? Levels.LevelTypes.All : Levels.LevelTypes.None));
            content.Add(new ScrapItem("Pomni", "Assets/LethalCompany/Mods/PolishedPlushiesAndSillyScrap/Pomni/Pomni.asset", PolishedPlushiesAndSillyScrap.plushiesSpawn[1].Value, PolishedPlushiesAndSillyScrap.enablePlushies[1].Value ? Levels.LevelTypes.All : Levels.LevelTypes.None));
            content.Add(new ScrapItem("Freddy", "Assets/LethalCompany/Mods/PolishedPlushiesAndSillyScrap/Freddy/Freddy.asset", PolishedPlushiesAndSillyScrap.plushiesSpawn[2].Value, PolishedPlushiesAndSillyScrap.enablePlushies[2].Value ? Levels.LevelTypes.All : Levels.LevelTypes.None));
            content.Add(new ScrapItem("Reimu", "Assets/LethalCompany/Mods/PolishedPlushiesAndSillyScrap/Reimu/Reimu.asset", PolishedPlushiesAndSillyScrap.plushiesSpawn[3].Value, PolishedPlushiesAndSillyScrap.enablePlushies[3].Value ? Levels.LevelTypes.All : Levels.LevelTypes.None));
            content.Add(new ScrapItem("Blahaj", "Assets/LethalCompany/Mods/PolishedPlushiesAndSillyScrap/Blahaj/Blahaj.asset", PolishedPlushiesAndSillyScrap.plushiesSpawn[4].Value, PolishedPlushiesAndSillyScrap.enablePlushies[4].Value ? Levels.LevelTypes.All : Levels.LevelTypes.None));
            content.Add(new ScrapItem("Pou", "Assets/LethalCompany/Mods/PolishedPlushiesAndSillyScrap/Pou/Pou.asset", PolishedPlushiesAndSillyScrap.plushiesSpawn[5].Value, PolishedPlushiesAndSillyScrap.enablePlushies[5].Value ? Levels.LevelTypes.All : Levels.LevelTypes.None));
            content.Add(new ScrapItem("Rei", "Assets/LethalCompany/Mods/PolishedPlushiesAndSillyScrap/Rei/Rei.asset", PolishedPlushiesAndSillyScrap.plushiesSpawn[6].Value, PolishedPlushiesAndSillyScrap.enablePlushies[6].Value ? Levels.LevelTypes.All : Levels.LevelTypes.None));
            content.Add(new ScrapItem("NecoArc", "Assets/LethalCompany/Mods/PolishedPlushiesAndSillyScrap/Neco-Arc/NecoArc.asset", PolishedPlushiesAndSillyScrap.plushiesSpawn[7].Value, PolishedPlushiesAndSillyScrap.enablePlushies[7].Value ? Levels.LevelTypes.All : Levels.LevelTypes.None));
            content.Add(new ScrapItem("Bocchi", "Assets/LethalCompany/Mods/PolishedPlushiesAndSillyScrap/Bocchi/Bocchi.asset", PolishedPlushiesAndSillyScrap.plushiesSpawn[8].Value, PolishedPlushiesAndSillyScrap.enablePlushies[8].Value ? Levels.LevelTypes.All : Levels.LevelTypes.None));
            content.Add(new ScrapItem("Miku", "Assets/LethalCompany/Mods/PolishedPlushiesAndSillyScrap/MikuTeto/Miku.asset", PolishedPlushiesAndSillyScrap.plushiesSpawn[9].Value, PolishedPlushiesAndSillyScrap.enablePlushies[9].Value ? Levels.LevelTypes.All : Levels.LevelTypes.None));
            content.Add(new ScrapItem("Teto", "Assets/LethalCompany/Mods/PolishedPlushiesAndSillyScrap/MikuTeto/Teto.asset", PolishedPlushiesAndSillyScrap.plushiesSpawn[10].Value, PolishedPlushiesAndSillyScrap.enablePlushies[10].Value ? Levels.LevelTypes.All : Levels.LevelTypes.None));
            content.Add(new ScrapItem("Pukeko", "Assets/LethalCompany/Mods/PolishedPlushiesAndSillyScrap/Pukeko/Pukeko.asset", PolishedPlushiesAndSillyScrap.plushiesSpawn[11].Value, PolishedPlushiesAndSillyScrap.enablePlushies[11].Value ? Levels.LevelTypes.All : Levels.LevelTypes.None));
            content.Add(new ScrapItem("Niko", "Assets/LethalCompany/Mods/PolishedPlushiesAndSillyScrap/Niko/Niko.asset", PolishedPlushiesAndSillyScrap.plushiesSpawn[12].Value, PolishedPlushiesAndSillyScrap.enablePlushies[12].Value ? Levels.LevelTypes.All : Levels.LevelTypes.None));
            content.Add(new ScrapItem("Nuko", "Assets/LethalCompany/Mods/PolishedPlushiesAndSillyScrap/Nuko/Nuko.asset", PolishedPlushiesAndSillyScrap.plushiesSpawn[13].Value, PolishedPlushiesAndSillyScrap.enablePlushies[13].Value ? Levels.LevelTypes.All : Levels.LevelTypes.None));
            content.Add(new ScrapItem("Seal", "Assets/LethalCompany/Mods/PolishedPlushiesAndSillyScrap/Seal/Seal.asset", PolishedPlushiesAndSillyScrap.plushiesSpawn[14].Value, PolishedPlushiesAndSillyScrap.enablePlushies[14].Value ? Levels.LevelTypes.All : Levels.LevelTypes.None));

            if (PolishedPlushiesAndSillyScrap.onlyLoadEnabled.Value)
            {
                for (int i = 0; i < contentNames.Length; i++)
                {
                    if (!PolishedPlushiesAndSillyScrap.enablePlushies[i].Value)
                    {
                        content.RemoveAll(x => x.ID == contentNames[i]);
                    }
                }
            }
            foreach (ScrapItem scrap in content)
            {
                Item item = PlushAssets.LoadAsset<Item>(scrap.contentPath);
                NetworkPrefabs.RegisterNetworkPrefab(item.spawnPrefab);
                Items.RegisterScrap(item, scrap.levelRarities, scrap.customLevelRarities);
            }
        }

    }
}
