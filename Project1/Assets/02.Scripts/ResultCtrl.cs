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
    private int[] SurScore = new int[5];
    private int[] MurScore = new int[5];

    private bool isVideo = true;

    [SerializeField]
    private GameObject UI;
    [SerializeField]
    private GameObject Fade;

    [SerializeField]
    private VideoPlayer videoPlayer;
    [SerializeField]
    private VideoClip[] videoClip = new VideoClip[2];

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
            videoPlayer.clip = videoClip[0];    // 생존자 승리
        else if (Result == 2 || Result == 3)
            videoPlayer.clip = videoClip[1];    // 살인마 승리

        if (Result == 1 || Result == 2)
        {
            for (int i = 0; i < 5; ++i)
                SurScore[i] = PlayerPrefs.GetInt("SurScore" + i, 0);
            CheckSurvivorScore();
        }
        else if (Result == 3 || Result == 4)
        {
            for (int i = 0; i < 5; ++i)
                MurScore[i] = PlayerPrefs.GetInt("MurScore" + i, 0);
            CheckMurdererScore();
        }

        videoPlayer.Play();
        StartCoroutine(VidioPlaying());

        StartCoroutine(StartFade(true));
    }

    void CheckSurvivorScore()
    {
        for (int i = 0; i < 5; ++i)
            SurvivorScore[i].text = SurScore[i].ToString();
    }
    
    void CheckMurdererScore()
    {
        for (int i = 0; i < 5; ++i)
            MurdererScore[i].text = MurScore[i].ToString();
    }

    IEnumerator VidioPlaying()
    {
        // 비디오가 시작했는 지 검사
        while (!videoPlayer.isPlaying)
            yield return null;

        yield return new WaitForEndOfFrame();

        // 비디오가 시작한 후 끝났는 지 검사
        while (videoPlayer.isPlaying)
            yield return null;

        StartCoroutine(StartFade(false));
    }

    void VideioEnd()
    {
        isVideo = false;
        UI.SetActive(true);

        SoundManager.instance.SetBGM("Ending2-Battle Lines");
        SoundManager.instance.CreateEffect();

        if (Result == 1 || Result == 2)
        {
            if (Result == 1)
            {
                Survivor[0].SetActive(true);
                MRsurvivor[0].material = Msurvivor[LobbyCtrl.instance.SurStat - 1];
            }
            else
            {
                Survivor[1].SetActive(true);
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
        PlayerPrefs.SetInt("Result", 0);
        SoundManager.instance.PlayEffect("Click");
        StartCoroutine(StartFade(false));
    }
}
