using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField]
    private AudioClip[] BGMClip;
    [SerializeField]
    private AudioClip[] EffectClip;

    private AudioSource BGMSource;
    private AudioSource[] EffectSource = new AudioSource[15];
    private int EffectSourceNum = 0;

    void Awake()
    {
        instance = this;

        BGMSource = gameObject.AddComponent<AudioSource>();
        BGMSource.playOnAwake = false;
        BGMSource.loop = true;

        SetVolume(1, PlayerPrefs.GetFloat("BGM", 1));
        SetVolume(2, PlayerPrefs.GetFloat("Effect", 1));
    }

    public void SetBGM(string name)
    {
        AudioClip ChangeClip = null;

        for (int i = 0; i < BGMClip.Length; ++i)
            if (BGMClip[i].name == name)
            {
                ChangeClip = BGMClip[i];
                break;
            }

        if (ChangeClip == null)
        {
            print(ChangeClip);
            return;
        }

        BGMSource.clip = ChangeClip;
        BGMSource.Play();
    }

    public void SetEffect(bool set, AudioSource source, string name)
    {
        for (int i = 0; i < EffectClip.Length; ++i)
            if (EffectClip[i].name == name)
            {
                if (!set)
                    EffectSource[EffectSourceNum++] = source;
                source.clip = EffectClip[i];
                source.volume = PlayerPrefs.GetFloat("Effect");
                return;
            }

        print("SoundNull: " + name);
    }

    public void SetVolume(int Num, float volume)
    {
        if (Num == 1)
            BGMSource.volume = volume;
        else if (Num == 2)
        {
            for (int i = 0; i < EffectSource.Length; ++i)
                if (EffectSource[i] != null)
                    EffectSource[i].volume = volume;
        }
    }
}