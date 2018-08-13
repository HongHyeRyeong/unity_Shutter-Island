using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCtrl : MonoBehaviour
{
    public static LobbyCtrl instance;

    [SerializeField]
    private GameObject[] survivor;
    [SerializeField]
    private GameObject[] murderer;
    [SerializeField]
    private RectTransform[] Join;
    [SerializeField]
    private Material[] Msurvivor = new Material[3];
    [SerializeField]
    private SkinnedMeshRenderer MRsurvivor;

    [HideInInspector]
    public bool SelSur;
    [HideInInspector]
    public int SurStat;
    [HideInInspector]
    public int Map;
    [HideInInspector]
    public bool SurRoomSelect;

    [SerializeField]
    private GameObject Fade;

    void Start()
    {
        Screen.SetResolution(1920, 1080, false);

        instance = this;

        SelSur = false;
        SurStat = 0;
        Map = 0;
        SurRoomSelect = false;

        SoundManager.instance.SetBGM("Lobby");
        SoundManager.instance.CreateEffect();
        StartCoroutine(StartFade(true));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            Screen.fullScreen = !Screen.fullScreen;
    }

    public void OnClickCharacter(int cha)
    {
        if (cha == 1)        // 생존자
        {
            if (Map != 0)
            {
                StartCoroutine(SelectMap(1, false));
            }

            SelSur = true;
            Map = 0;
        }
        else if (cha == 2)   // 살인마
        {
            if (SurRoomSelect)
            {
                SurRoomSelect = false;
                StartCoroutine(SelectMap(0, false));
            }

            SelSur = false;
            SurStat = 0;
            MRsurvivor.material = Msurvivor[0];
        }

        for (int i = 0; i < survivor.Length; ++i)
            survivor[i].SetActive(SelSur);

        for (int i = 0; i < murderer.Length; ++i)
            murderer[i].SetActive(!SelSur);

        SoundManager.instance.PlayEffect("Click");
    }

    public void OnClickSurvivor(int i)
    {
        SurStat = i;
        MRsurvivor.material = Msurvivor[i - 1];

        SoundManager.instance.PlayEffect("Click");
    }

    public void OnClickMap(int i)
    {
        Map = i;
        StartCoroutine(SelectMap(1, true));

        SoundManager.instance.PlayEffect("Click");
    }

    public IEnumerator SelectMap(int cha, bool use)
    {
        Vector3 startPos;
        Vector3 endPos;

        if (use)
        {
            startPos = new Vector3(0, -665, 0);
            endPos = new Vector3(0, -415, 0);
        }
        else
        {
            startPos = new Vector3(0, -415, 0);
            endPos = new Vector3(0, -665, 0);
        }

        float time = 0;

        while (true)
        {
            time += Time.deltaTime * 3;
            Join[cha].localPosition = Vector3.Lerp(startPos, endPos, time);

            if (time >= 1)
                break;

            yield return null;
        }
    }

    public IEnumerator StartFade(bool start)
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
        }
    }
}
