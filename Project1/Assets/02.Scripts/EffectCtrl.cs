using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCtrl : MonoBehaviour
{
    [SerializeField]
    private GameObject Camera;
    [SerializeField]
    private Material SkyboxDay;
    [SerializeField]
    private Material SkyboxNight;

    [SerializeField]
    private GameObject SurvivorFootPrints;
    [SerializeField]
    private GameObject FootPrint;
    private GameObject[] FootPrints = new GameObject[3000];
    private int FootPrintsNum = -1;
    private float delay = 0.05f;
    private float savedelay;

    void Start()
    {
        // SkyBox
        int map = Random.Range(1, 3);

        if (map == 1)
            Camera.AddComponent<Skybox>().material = SkyboxDay;
        else
            Camera.AddComponent<Skybox>().material = SkyboxDay;

        savedelay = delay;
    }

    public void UseFootPrint(Vector3 SurPos)
    {
        if (delay != savedelay)
            return;
        StartCoroutine(DelayTime());

        bool isNew = true;

        float randomX = Random.Range(-0.8f, 0.8f);

        for (int i = 0; i <= FootPrintsNum; ++i)
        {
            if (!FootPrints[i].activeSelf)
            {
                isNew = false;

                StartCoroutine(FootPrints[i].GetComponent<FootPrintCtrl>().Use());
                FootPrints[i].transform.position = new Vector3(
                    randomX + SurPos.x, SurPos.y + 1.5f, SurPos.z);
                break;
            }
        }

        if (isNew)
        {
            Vector3 Pos = new Vector3(
                    randomX + SurPos.x, SurPos.y + 1.5f, SurPos.z);

            FootPrints[++FootPrintsNum] = Instantiate(FootPrint, Pos,
                Quaternion.Euler(90, 0, Random.Range(0, 360)));
            FootPrints[FootPrintsNum].transform.parent = SurvivorFootPrints.transform;
            StartCoroutine(FootPrints[FootPrintsNum].GetComponent<FootPrintCtrl>().Use());
        }
    }

    IEnumerator DelayTime()
    {
        while (true)
        {
            delay -= Time.deltaTime;

            if (delay <= 0)
                break;

            yield return null;
        }
        delay = savedelay;
    }
}
