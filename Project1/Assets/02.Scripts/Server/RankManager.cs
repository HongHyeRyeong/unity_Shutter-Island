using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankManager : MonoBehaviour
{
    //
    [SerializeField]
    public Text UserID;
    [SerializeField]
    public Text SurRankTxt;
    [SerializeField]
    public Text MurRankTxt;

    //
    private int Check;
    private int SurScore;
    private int MurScore;

    //
    public string RankURL;

    void Start()
    {
        UserID.text = Login.GetID();
        SurRankTxt.text = Login.GetSurRank();
        MurRankTxt.text = Login.GetMurRank();

        if (ResultCtrl.instance != null)
        {
            //RankURL = "wjddus0424.dothome.co.kr/RankUp.php";

            Check = ResultCtrl.instance.GetCheck();

            if (Check == 1 || Check == 2)
                SurScore = ResultCtrl.instance.GetSurvivorTotalScore();
            else if (Check == 3 || Check == 4)
                MurScore = ResultCtrl.instance.GetMurdererTotalScore();

            //if (SurScore >= 10000)
            //    StartCoroutine(SurRankUp());
            //else if (MurScore >= 10000)
            //    StartCoroutine(MurRankUp());
        }
    }

    IEnumerator SurRankUp()
    {
        WWWForm form = new WWWForm();

        WWW webRequest = new WWW(RankURL, form);
        yield return webRequest;
    }

    IEnumerator MurRankUp()
    {
        WWWForm form = new WWWForm();

        WWW webRequest = new WWW(RankURL, form);
        yield return webRequest;
    }
}
