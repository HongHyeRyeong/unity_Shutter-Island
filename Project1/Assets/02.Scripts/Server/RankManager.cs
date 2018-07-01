using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankManager : MonoBehaviour {

    public Text UserID;
    public Text SurRankTxt;
    public Text MurRankTxt;

    // Use this for initialization
    void Start () {
        UserID.text = Login.GetID();
        SurRankTxt.text = Login.GetSurRank();
        MurRankTxt.text = Login.GetMurRank();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
