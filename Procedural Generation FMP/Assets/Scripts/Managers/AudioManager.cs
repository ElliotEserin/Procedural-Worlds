using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static AudioManager instance;
    static System.Random rand;

    private void Awake()
    {
        instance = this;
        rand = new System.Random();

        source = GetComponent<AudioSource>();
    }

    public static AudioSource source;
    public static float masterVol, musicVol, sfxVol;
    public AudioClip[] footstepSounds;

    public float footstepLength = 0.5f;
    float footTimer;

    public static void PlayFootstep()
    {
        if (instance.footTimer <= 0)
        {
            int i = rand.Next(0, instance.footstepSounds.Length);

            source.PlayOneShot(instance.footstepSounds[i]);

            instance.footTimer = instance.footstepLength;
        }
    }

    public static void PlayAmbientNoise(AudioClip clip)
    {
        source.Stop();
        source.clip = clip;
        source.Play();
    }

    public static void PlayRandomSound(AudioClip[] clips)
    {
        int i = rand.Next(0, clips.Length);
        source.PlayOneShot(clips[i]);
    }

    private void Update()
    {
        footTimer -= Time.deltaTime;
    }
}
