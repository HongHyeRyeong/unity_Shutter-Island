using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class ResultCtrl : MonoBehaviour
{
    // 1: 생존자 승리, 2: 생존자 패배, 3: 살인마 승리, 4: 살인마 패배
    private int Result;
    private bool isVideo = true;

    [SerializeField]
    private GameObject UI;
    [SerializeField]
    private GameObject Fade;

    [SerializeField]
    private VideoPlayer videoPlayer;
    [SerializeField]
    private VideoClip[] videoClip = new VideoClip[2];   // 0: 생존자 승리, 1: 살인마 승리

    [SerializeField]
    private GameObject[] Survivor;
    [SerializeField]
    private GameObject[] Murderer;
    [SerializeField]
    private Material[] Msurvivor = new Material[3];
    [SerializeField]
    private SkinnedMeshRenderer[] MRsurvivor = new SkinnedMeshRenderer[2];

    [SerializeField]
    private Text[] SurvivorScore = new Text[5];
    [SerializeField]
    private Text[] MurdererScore = new Text[5];

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;

        Result = PlayerPrefs.GetInt("Result", 0);

            if (Result == 1 || Result == 4)
            videoPlayer.clip = videoClip[0];
        else if (Result == 2 || Result == 3)
            videoPlayer.clip = videoClip[1];

        videoPlayer.Play();
        StartCoroutine(VidioPlaying());

        StartCoroutine(StartFade(true));
    }

    IEnumerator VidioPlaying()
    {
        yield return new WaitForSeconds(1);

        while (true)
        {
            if (!videoPlayer.isPlaying)
            {
                StartCoroutine(StartFade(false));
                break;
            }

            yield return null;
        }
    }

    void VideioEnd()
    {
        isVideo = false;
        UI.SetActive(true);

        if (Result == 1 || Result == 2)
        {
            if (Result == 1)
            {
                Survivor[0].SetActive(true);

                if (LobbyCtrl.instance.SurStat != 0)
                    MRsurvivor[0].material = Msurvivor[LobbyCtrl.instance.SurStat - 1];
            }
            else
            {
                Survivor[1].SetActive(true);

                if (LobbyCtrl.instance.SurStat != 0)
                    MRsurvivor[1].material = Msurvivor[LobbyCtrl.instance.SurStat - 1];
            }

            for (int i = 2; i < Survivor.Length; ++i)
                Survivor[i].SetActive(true);
        }
        else if (Result == 3 || Result == 4)
        {
            for (int i = 0; i < Murderer.Length; ++i)
                Murderer[i].SetActive(true);

            if (Result == 3)
            {
                Murderer[0].transform.eulerAngles = Murderer[0].transform.eulerAngles + Vector3.down * 30;
                Murderer[0].GetComponent<Animator>().SetBool("isWin", true);
            }
            else
            {
                Murderer[0].transform.position = Murderer[0].transform.position + Vector3.right * 0.3f;
                Murderer[0].GetComponent<Animator>().SetBool("isWin", false);
            }
        }

        StartCoroutine(StartFade(true));
    }

    IEnumerator StartFade(bool start)
    {
        Fade.SetActive(true);
        Image imgFade = Fade.GetComponent<Image>();

        Color color = imgFade.color;
        float time = 0;

        if (start)
        {
            color.a = 1;

            while (color.a > 0)
            {
                time += Time.deltaTime * 0.8f;
                color.a = Mathf.Lerp(1, 0, time);
                imgFade.color = color;

                yield return null;
            }

            Fade.SetActive(false);
        }
        else
        {
            color.a = 0;

            while (color.a < 1)
            {
                time += Time.deltaTime;
                color.a = Mathf.Lerp(0, 1, time);
                imgFade.color = color;

                yield return null;
            }

            if(isVideo)
                VideioEnd();
            else
                SceneManager.LoadScene("1. Lobby");
        }
    }

    public void OnClickExitBtn()
    {
        StartCoroutine(StartFade(false));
    }
}
