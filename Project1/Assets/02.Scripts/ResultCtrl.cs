using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultCtrl : MonoBehaviour {

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void OnClickExitBtn()
    {
        SceneManager.LoadScene("1. Lobby");
    }
}
