using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public static SettingManager instance;

    public GameObject Setting;

    public bool MurdererQuit = false;

    void Start()
    {
        instance = this;
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

    public void BGMVolume(Slider bgm)
    {
        SoundManager.instance.SetVolume(1, bgm.value);
    }

    public void EffectVolume(Slider effect)
    {
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
