using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankManager : MonoBehaviour
{
    [SerializeField]
    public Text UserID;
    [SerializeField]
    public Text SurRankTxt;
    [SerializeField]
    public Text MurRankTxt;

    void Start()
    {
        UserID.text = Login.GetID();
        SurRankTxt.text = Login.GetSurRank();
        MurRankTxt.text = Login.GetMurRank();
    }
}
