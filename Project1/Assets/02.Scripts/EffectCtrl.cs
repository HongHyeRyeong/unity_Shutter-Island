using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCtrl : MonoBehaviour
{
    public GameObject Camera;

    public Material SkyboxDay;
    public Material SkyboxNight;

    public GameObject FootPrint;
    const int FootPrintsNum = 1000;
    GameObject[] FootPrints = new GameObject[FootPrintsNum];

    void Start()
    {
        // SkyBox
        int map = Random.Range(1, 3);

        if (map == 1)
            Camera.AddComponent<Skybox>().material = SkyboxDay;
        else
            Camera.AddComponent<Skybox>().material = SkyboxDay;

        // Survivor FootPrint
        GameObject SurvivorFootPrints = GameObject.Find("SurvivorFootPrints");
        for (int i = 0; i < FootPrintsNum; ++i)
        {
            Quaternion rot = Quaternion.Euler(90, 0, Random.Range(0, 360));
            GameObject temp = Instantiate(FootPrint, new Vector3(0, 0, 0), rot);

            FootPrints[i] = temp;
            temp.transform.parent = SurvivorFootPrints.transform;
        }

    }

    public void UseFootPrint(Vector3 SurPos)
    {
        for (int i = 0; i < FootPrintsNum; ++i)
        {
            if (!FootPrints[i].activeSelf)
            {
                FootPrints[i].SetActive(true);
                FootPrints[i].transform.position = new Vector3(
                    Random.Range(-0.4f, 0.4f) + SurPos.x,
                    SurPos.y + 1.5f,
                    SurPos.z);
                break;
            }
        }
    }
}
