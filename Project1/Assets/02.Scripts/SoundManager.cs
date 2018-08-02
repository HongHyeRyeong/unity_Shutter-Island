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

    // 로비, 결과 효과음
    public void CreateEffect()
    {
        for (int i = 0; i < 3; ++i)
        {
            EffectSource[i] = gameObject.AddComponent<AudioSource>();
            EffectSource[i].playOnAwake = false;
        }
    }

    // 로비, 결과 효과음 플레이
    public void PlayEffect(string name)
    {
        for (int i = 0; i < EffectClip.Length; ++i)
        {
            if (EffectSource[i] != null)
                if (EffectClip[i].name == name)
                {
                    AudioSource source = GetEmptyEffectSource();
                    source.clip = EffectClip[i];
                    source.Play();
                    return;
                }
        }

        print("SoundNull: " + name);
    }

    // 빈오디오소스 받아오기
    AudioSource GetEmptyEffectSource()
    {
        int MaxIndex = 0;
        float MaxProgress = 0;

        for (int i = 0; i < EffectSource.Length; ++i)
        {
            if (!EffectSource[i].isPlaying)
                return EffectSource[i];

            float progress = EffectSource[i].time / EffectSource[i].clip.length;

            if (progress > MaxProgress && !EffectSource[i].loop)    // 다 사용중이라면 제일 많이 플레이된 오디오소스
            {
                MaxIndex = i;
                MaxProgress = progress;
            }
        }
        return EffectSource[MaxIndex];
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