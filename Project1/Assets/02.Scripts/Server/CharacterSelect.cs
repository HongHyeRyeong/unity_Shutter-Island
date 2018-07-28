using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public static CharacterSelect instance;

    [SerializeField]
    public GameObject[] survivor;
    [SerializeField]
    public GameObject[] murderer;
    [SerializeField]
    public RectTransform[] Join;

    public bool SelSur = false;
    public int SurStat = -1;
    public int Map = -1;

    public bool SurRoomSelect = false;

    [SerializeField]
    public GameObject Fade;

    void Start()
    {
        instance = this;

        SoundManager.instance.SetBGM("Lobby2-Last Stand");
        StartCoroutine(StartFade(true));
    }

    public void OnClickCharacter(int cha)
    {
        if (cha == 1)        // 생존자
        {
            if (Map != -1)
                StartCoroutine(SelectMap(1, false));

            SelSur = true;
            Map = -1;
        }
        else if (cha == 2)   // 살인마
        {
            if (SurRoomSelect)
            {
                SurRoomSelect = false;
                StartCoroutine(SelectMap(0, false));
            }

            SelSur = false;
            SurStat = -1;
        }

        for (int i = 0; i < survivor.Length; ++i)
            survivor[i].SetActive(SelSur);

        for (int i = 0; i < murderer.Length; ++i)
            murderer[i].SetActive(!SelSur);
    }

    public void OnClickSurvivor(int i)
    {
        SurStat = i;
    }

    public void OnClickMap(int i)
    {
        Map = i;
        StartCoroutine(SelectMap(1, true));
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
