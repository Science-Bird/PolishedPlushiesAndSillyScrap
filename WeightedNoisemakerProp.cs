using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

namespace PolishedPlushiesAndSillyScrap
{
    public class WeightedNoisemakerProp : GrabbableObject
    {
        public AudioSource noiseAudio;

        public AudioSource noiseAudioFar;

        [Space(3f)]
        public AudioClip fixedNoiseSFX;

        public AudioClip fixedNoiseSFXFar;

        public AudioClip[] noiseSFX;

        public AudioClip[] noiseSFXFar;

        [Space(3f)]
        public float noiseRange;

        public float maxLoudness;

        public float minLoudness;

        public float minPitch;

        public float maxPitch;

        public float loudness = 1f;

        public System.Random noisemakerRandom;

        public Animator triggerAnimator;

        public Mesh[] altMeshVariants = [];

        public Material[] altMaterialVariants = [];

        public MeshFilter targetFilter;

        public MeshRenderer targetRenderer;

        private List<int> noiseWeights;

        private bool isPlaying = false;

        private float lastTime;

        private float sfxLength;

        public override void Start()
        {
            base.Start();
            NetworkObject networkObj = base.gameObject.GetComponent<NetworkObject>();
            if (networkObj != null)
            {
                //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{base.itemProperties.itemName} - Network ID: {(int)networkObj.NetworkObjectId}");
                //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{base.itemProperties.itemName} - Using seed: {StartOfRound.Instance.randomMapSeed + 85 + (int)networkObj.NetworkObjectId}");
                noisemakerRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 85 + (int)networkObj.NetworkObjectId);
            }
            else
            {
                //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{base.itemProperties.itemName} - No network object. Using seed: {StartOfRound.Instance.randomMapSeed + 85}");
                noisemakerRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 85);
            }
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);

            if (!(GameNetworkManager.Instance.localPlayerController == null))
            {
                if (noiseAudio == null || fixedNoiseSFX == null)
                {
                    return;
                }

                if (noisemakerRandom == null)
                {
                    NetworkObject networkObj = base.gameObject.GetComponent<NetworkObject>();
                    if (networkObj != null)
                    {
                        //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{base.itemProperties.itemName} - Network ID: {(int)networkObj.NetworkObjectId}");
                        //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{base.itemProperties.itemName} - Using seed: {StartOfRound.Instance.randomMapSeed + 85 + (int)networkObj.NetworkObjectId}");
                        noisemakerRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 85 + (int)networkObj.NetworkObjectId);
                    }
                    else
                    {
                        //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{base.itemProperties.itemName} - No network object. Using seed: {StartOfRound.Instance.randomMapSeed + 85}");
                        noisemakerRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 85);
                    }
                }

                float randomLoudness = (float)noisemakerRandom.Next((int)(minLoudness * 100f), (int)(maxLoudness * 100f)) / 100f;
                float pitch = (float)noisemakerRandom.Next((int)(minPitch * 100f), (int)(maxPitch * 100f)) / 100f;
                noiseAudio.pitch = pitch;
                noiseAudio.PlayOneShot(fixedNoiseSFX, randomLoudness);
                if (noiseAudioFar != null && fixedNoiseSFXFar != null)
                {
                    noiseAudioFar.pitch = pitch;
                    noiseAudioFar.PlayOneShot(fixedNoiseSFXFar, randomLoudness);
                }
                if (triggerAnimator != null)
                {
                    triggerAnimator.SetTrigger("playAnim");
                }
                WalkieTalkie.TransmitOneShotAudio(noiseAudio, fixedNoiseSFX, randomLoudness);
                RoundManager.Instance.PlayAudibleNoise(base.transform.position, noiseRange, randomLoudness, 0, isInElevator && StartOfRound.Instance.hangarDoorsClosed);
                if (randomLoudness >= 0.6f && playerHeldBy != null)
                {
                    playerHeldBy.timeSinceMakingLoudNoise = 0f;
                }

                if (PolishedPlushiesAndSillyScrap.plushDict.TryGetValue(itemProperties.itemName, out int[] value))
                {
                    noiseWeights = new List<int>();
                    noiseWeights = value.ToList();
                }
                else
                {
                    PolishedPlushiesAndSillyScrap.Logger.LogError("Failed to retrieve SFX config! Reverting to default.");
                }

                if (noiseSFX.Length != noiseWeights.Count)
                {
                    PolishedPlushiesAndSillyScrap.Logger.LogError("Invalid weighted noisemaker! Rejecting item activation.");
                    return;
                }
                if (isPlaying || noiseWeights.Sum() <= 0)
                {
                    return;
                }

                int[] backupArray = noiseWeights.ToArray();
                int num = noisemakerRandom.Next(0, noiseSFX.Length);
                bool doSFX = false;
                if (noiseWeights.Count > 0 && noiseWeights.Sum() <= 100)
                {
                    int roll = noisemakerRandom.Next(0, 100);
                    //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"Roll: {roll})");
                    for (var i = 0; i < noiseWeights.Count; i++)
                    {
                        int chance = noiseWeights[i];
                        if (roll < chance)
                        {
                            //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"Found match at {i} ({backupArray[i]}% chance, rolled {roll})");
                            num = i;
                            doSFX = true;
                            break;
                        }
                        else if (i < noiseWeights.Count - 1)
                        {
                            noiseWeights[i + 1] += chance;
                        }
                    }
                }
                else if (noiseWeights.Count > 0)
                {
                    doSFX = true;
                }
                else
                {
                    PolishedPlushiesAndSillyScrap.Logger.LogError("No weights found! Rejecting item activation.");
                    return;
                }
                if (!doSFX)
                {
                    return;
                }
                noiseAudio.pitch = 1f;
                noiseAudio.PlayOneShot(noiseSFX[num], loudness);
                sfxLength = noiseSFX[num].length;
                lastTime = Time.realtimeSinceStartup;
                isPlaying = true;
                if (noiseAudioFar != null && noiseSFXFar.Length == noiseSFX.Length)
                {
                    noiseAudioFar.pitch = 1f;
                    noiseAudioFar.PlayOneShot(noiseSFXFar[num], loudness);
                }
                WalkieTalkie.TransmitOneShotAudio(noiseAudio, noiseSFX[num], loudness);
                RoundManager.Instance.PlayAudibleNoise(base.transform.position, noiseRange, loudness, 0, isInElevator && StartOfRound.Instance.hangarDoorsClosed);
                if (loudness >= 0.6f && playerHeldBy != null)
                {
                    playerHeldBy.timeSinceMakingLoudNoise = 0f;
                }
                if (triggerAnimator != null && ((itemProperties.name == "Freddy" && num == 2) || itemProperties.name == "Bocchi" || itemProperties.name == "Niko"))
                {
                    triggerAnimator.SetTrigger("playSpecial");
                }
            }
        }

        public override void Update()
        {
            base.Update();
            if (isPlaying && Time.realtimeSinceStartup - lastTime > sfxLength)
            {
                isPlaying = false;
            }
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
            if (radarIcon != null)
            {
                radarIcon.position += new Vector3(0f, 0.1f, 0f);
            }
        }
    }
}
