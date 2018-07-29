using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultCtrl : MonoBehaviour {

    public Text ResultText;

    int result = 0;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;

        result = PlayerPrefs.GetInt("Result", 0);

        if(result == 1)
        {
            ResultText.text = "Survivor Win";
        }
        else if(result == 2)
        {
            ResultText.text = "Survivor Dead";
        }
        else if (result == 3)
        {
            ResultText.text = "Murderer Win";
        }
        else if (result == 4)
        {
            ResultText.text = "Murderer Dead";
        }
        else
        {
            ResultText.text = "-";
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void OnClickExitBtn()
    {
        SceneManager.LoadScene("1. Lobby");
    }
}
