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
    private int SurRank;
    private int MurRank;

    //
    public string SurRankURL;
    public string MurRankURL;

    void Start()
    {
        UserID.text = Login.GetID();
        SurRankTxt.text = Login.GetSurRank();
        MurRankTxt.text = Login.GetMurRank();

        if (ResultCtrl.instance != null)
        {
            int.TryParse(SurRankTxt.text, out SurRank);
            int.TryParse(MurRankTxt.text, out MurRank);

            SurRankURL = "wjddus0424.dothome.co.kr/SurvivorRankUp.php";
                MurRankURL = "wjddus0424.dothome.co.kr/MurdererRankUp.php";

            Check = ResultCtrl.instance.GetCheck();

            if (Check == 1 || Check == 2)
                SurScore = ResultCtrl.instance.GetSurvivorTotalScore();
            else if (Check == 3 || Check == 4)
                MurScore = ResultCtrl.instance.GetMurdererTotalScore();

            if (SurScore >= 10000)
            {
                SurRank++;

                //StartCoroutine(SurRankUp());
            }
            else if (MurScore >= 10000)
            {
                MurRank++;
                
                //StartCoroutine(MurRankUp());
            }
        }
    }

    IEnumerator SurRankUp()
    {
        WWWForm form = new WWWForm();
        form.AddField("SurvivorRank", SurRank);

        WWW webRequest = new WWW(SurRankURL, form);
        yield return webRequest;

        print(webRequest.text);

        yield return null;
    }

    IEnumerator MurRankUp()
    {
        WWWForm form = new WWWForm();
        form.AddField("MurdererRank", MurRank);

        WWW webRequest = new WWW(MurRankURL, form);
        yield return webRequest;

        print(webRequest.text);

        yield return null;
    }
}
