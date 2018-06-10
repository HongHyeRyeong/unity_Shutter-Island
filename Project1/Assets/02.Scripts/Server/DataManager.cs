using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour {

    private string SaveDataUrl = "wjddus0424.dothome.co.kr/SaveData.php";
    private string username;

    private void Start()
    {
        username = Login.GetID();
        print(username);
    }

    IEnumerator SaveScore(int SurScore)
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", username);
        form.AddField("SurvivorScore", SurScore);

        WWW webRequest = new WWW(SaveDataUrl, form);
        yield return webRequest;
    }
}
