using Unity.Netcode;
using UnityEngine;

namespace PolishedPlushiesAndSillyScrap
{
    public class IdleAnimatingProp : GrabbableObject
    {
        public AudioClip[] randomClips = [];

        public AudioClip[] randomClipsFar = [];

        public Animator triggerAnimator;

        public string[] animatorTriggers = [];

        public bool holdingAnimation = false;

        public AudioSource animAudio;

        public AudioSource animAudioFar;

        public float loudness = 1f;

        public float noiseRange;

        public float minInterval;

        public float maxInterval;

        public float animChancePercent;

        public bool fixedAssignment = false;

        public bool isNuko = false;

        public AudioClip specialNukoClip;

        public bool playedFallAudio = false;

        public float currentInterval;

        public float lastIntervalTime;

        public System.Random animationRandom;

        public float correctVertical = 0.5f;

        public override void Start()
        {
            base.Start();
            NetworkObject networkObj = base.gameObject.GetComponent<NetworkObject>();
            if (networkObj != null)
            {
                //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{base.itemProperties.itemName} - Network ID: {(int)networkObj.NetworkObjectId}");
                //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{base.itemProperties.itemName} - Using seed: {StartOfRound.Instance.randomMapSeed + 85 + (int)networkObj.NetworkObjectId}");
                animationRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 85 + (int)networkObj.NetworkObjectId);
            }
            else
            {
                //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{base.itemProperties.itemName} - No network object. Using seed: {StartOfRound.Instance.randomMapSeed + 85}");
                animationRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 85);
            }
            if (isNuko)
            {
                maxInterval = PolishedPlushiesAndSillyScrap.nukoAnimFrequency.Value;
                animChancePercent = PolishedPlushiesAndSillyScrap.nukoAnimConsistency.Value;
                correctVertical = 0.26f;
                itemProperties.verticalOffset = 0.26f;
            }
            else if (itemProperties.itemName == "Seal")
            {
                maxInterval = PolishedPlushiesAndSillyScrap.sealAnimFrequency.Value;
                animChancePercent = PolishedPlushiesAndSillyScrap.sealAnimConsistency.Value;
                correctVertical = 0.5f;
                itemProperties.verticalOffset = 0.5f;
            }


        }



        public override void Update()
        {
            base.Update();
            if (Time.realtimeSinceStartup - lastIntervalTime > currentInterval)
            {
                lastIntervalTime = Time.realtimeSinceStartup;
                if (animationRandom == null)
                {
                    NetworkObject networkObj = base.gameObject.GetComponent<NetworkObject>();
                    if (networkObj != null)
                    {
                        //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{base.itemProperties.itemName} - Network ID: {(int)networkObj.NetworkObjectId}");
                        //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{base.itemProperties.itemName} - Using seed: {StartOfRound.Instance.randomMapSeed + 85 + (int)networkObj.NetworkObjectId}");
                        animationRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 85 + (int)networkObj.NetworkObjectId);
                    }
                    else
                    {
                        //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{base.itemProperties.itemName} - No network object. Using seed: {StartOfRound.Instance.randomMapSeed + 85}");
                        animationRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 85);
                    }
                }
                currentInterval = (float)animationRandom.NextDouble() * (maxInterval - minInterval) + minInterval;
                if ((float)animationRandom.NextDouble() * 100f < animChancePercent && !isHeld)
                {
                    int num1 = animationRandom.Next(0, animatorTriggers.Length);
                    if (animatorTriggers.Length > 0)
                    {
                        triggerAnimator.SetTrigger(animatorTriggers[num1]);
                    }
                    bool playWalkieSFX = SomeWalkiesAreActive();
                    if (isNuko && !playWalkieSFX && !isInShipRoom && !isInElevator)
                    {
                        return;
                    }
                    if (fixedAssignment)
                    {
                        if (animAudio != null && num1 < randomClips.Length)
                        {
                            if (isNuko)
                            {
                                if (isInElevator || isInShipRoom)
                                {
                                    StartOfRound.Instance.speakerAudioSource.PlayOneShot(randomClips[num1]);
                                }
                                if (playWalkieSFX)
                                {
                                    PlayOnActiveWalkies(randomClips[num1]);
                                }
                            }
                            else
                            {
                                animAudio.PlayOneShot(randomClips[num1], loudness);
                                WalkieTalkie.TransmitOneShotAudio(animAudio, randomClips[num1], loudness);
                                if (PolishedPlushiesAndSillyScrap.sealAttractDogs.Value)
                                {
                                    RoundManager.Instance.PlayAudibleNoise(transform.position, noiseRange, loudness, 0, isInElevator && StartOfRound.Instance.hangarDoorsClosed);
                                }
                                if (animAudioFar != null && num1 < randomClipsFar.Length)
                                {
                                    animAudioFar.PlayOneShot(randomClipsFar[num1], loudness);
                                }
                            }
                        }
                    }
                    else
                    {
                        int num2 = animationRandom.Next(0, randomClips.Length);
                        if (animAudio != null && randomClips.Length > 0)
                        {
                            animAudio.PlayOneShot(randomClips[num2], loudness);
                            WalkieTalkie.TransmitOneShotAudio(animAudio, randomClips[num2], loudness);
                            if (PolishedPlushiesAndSillyScrap.sealAttractDogs.Value)
                            {
                                RoundManager.Instance.PlayAudibleNoise(transform.position, noiseRange, loudness, 0, isInElevator && StartOfRound.Instance.hangarDoorsClosed);
                            }
                        }
                        if (animAudioFar != null && num2 < randomClipsFar.Length)
                        {
                            animAudioFar.PlayOneShot(randomClipsFar[num2], loudness);
                        }
                    }


                }
            }
        }

        public override void GrabItem()
        {
            base.GrabItem();
            if (animationRandom == null)
            {
                NetworkObject networkObj = base.gameObject.GetComponent<NetworkObject>();
                if (networkObj != null)
                {
                    //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{base.itemProperties.itemName} - Network ID: {(int)networkObj.NetworkObjectId}");
                    //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{base.itemProperties.itemName} - Using seed: {StartOfRound.Instance.randomMapSeed + 85 + (int)networkObj.NetworkObjectId}");
                    animationRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 85 + (int)networkObj.NetworkObjectId);
                }
                else
                {
                    //PolishedPlushiesAndSillyScrap.Logger.LogDebug($"{base.itemProperties.itemName} - No network object. Using seed: {StartOfRound.Instance.randomMapSeed + 85}");
                    animationRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 85);
                }
            }
            if (holdingAnimation && triggerAnimator != null)
            {
                triggerAnimator.SetTrigger("playHold");
            }
            if (animAudio != null && randomClips.Length > 0)
            {
                int num = animationRandom.Next(0, randomClips.Length);
                if (isNuko)
                {
                    if (isInElevator || isInShipRoom)
                    {
                        StartOfRound.Instance.speakerAudioSource.PlayOneShot(randomClips[num]);
                    }
                    if (PolishedPlushiesAndSillyScrap.nukoTransmitMore.Value && SomeWalkiesAreActive())
                    {
                        PlayOnActiveWalkies(randomClips[num]);
                    }
                }
                else
                {
                    animAudio.PlayOneShot(randomClips[num], loudness);
                    WalkieTalkie.TransmitOneShotAudio(animAudio, randomClips[num], loudness);
                    RoundManager.Instance.PlayAudibleNoise(transform.position, noiseRange, loudness, 0, isInElevator && StartOfRound.Instance.hangarDoorsClosed);
                    if (animAudioFar != null && num < randomClipsFar.Length)
                    {
                        animAudioFar.PlayOneShot(randomClipsFar[num], loudness);
                    }
                }
            }
        }

        public override void DiscardItem()
        {
            base.DiscardItem();
            if (itemProperties.verticalOffset != correctVertical)
            {
                itemProperties.verticalOffset = correctVertical;
            }
            if (holdingAnimation && triggerAnimator != null)
            {
                triggerAnimator.SetTrigger("stopHold");
            }
            if (animAudio != null && randomClips.Length > 0)
            {
                int num = animationRandom.Next(0, randomClips.Length);
                if (isNuko)
                {
                    if (isInElevator || isInShipRoom)
                    {
                        StartOfRound.Instance.speakerAudioSource.PlayOneShot(randomClips[num]);
                    }
                    if (PolishedPlushiesAndSillyScrap.nukoTransmitMore.Value && SomeWalkiesAreActive())
                    {
                        PlayOnActiveWalkies(randomClips[num]);
                    }
                }
                else
                {
                    animAudio.PlayOneShot(randomClips[num], loudness);
                    WalkieTalkie.TransmitOneShotAudio(animAudio, randomClips[num], loudness);
                    RoundManager.Instance.PlayAudibleNoise(transform.position, noiseRange, loudness, 0, isInElevator && StartOfRound.Instance.hangarDoorsClosed);
                    if (animAudioFar != null && num < randomClipsFar.Length)
                    {
                        animAudioFar.PlayOneShot(randomClipsFar[num], loudness);
                    }
                }
            }  
        }

        public bool SomeWalkiesAreActive()
        {
            foreach (WalkieTalkie walkie in WalkieTalkie.allWalkieTalkies)
            {
                if ((walkie.walkieTalkieLight.enabled || walkie.isBeingUsed) && Vector3.Distance(walkie.gameObject.transform.position, gameObject.transform.position) < 25f)
                {
                    return true;
                }
            }
            return false;
        }

        public void PlayOnActiveWalkies(AudioClip clip)
        {
            foreach (WalkieTalkie walkie in WalkieTalkie.allWalkieTalkies)
            {
                if (walkie.isBeingUsed)
                {
                    walkie.target.PlayOneShot(clip, 1f);
                }
                else if (walkie.walkieTalkieLight.enabled)
                {
                    if (walkie.PlayerIsHoldingAnotherWalkieTalkie(walkie))
                    {
                        break;
                    }
                    walkie.thisAudio.PlayOneShot(clip);
                }
            }
        }

        public override void OnHitGround()
        {
            base.OnHitGround();
            if (isNuko)
            {
                playedFallAudio = false;
            }
        }
    }
}
