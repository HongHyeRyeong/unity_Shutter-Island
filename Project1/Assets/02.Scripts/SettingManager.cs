using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public static SettingManager instance;

    public GameObject Setting;
    [SerializeField]
    private Slider[] SoundSlider = new Slider[2];

    public bool MurdererQuit = false;

    void Start()
    {
        instance = this;
        SetSoundSlider();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Setting.SetActive(!Setting.activeSelf);

            if (Setting.activeSelf)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void SetSoundSlider()
    {
        SoundSlider[0].value = PlayerPrefs.GetFloat("BGM", 1);
        SoundSlider[1].value = PlayerPrefs.GetFloat("Effect", 1);
    }

    public void BGMVolume(Slider bgm)
    {
        PlayerPrefs.SetFloat("BGM", bgm.value);
        SoundManager.instance.SetVolume(1, bgm.value);
    }

    public void EffectVolume(Slider effect)
    {
        PlayerPrefs.SetFloat("Effect", effect.value);
        SoundManager.instance.SetVolume(2, effect.value);
    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
