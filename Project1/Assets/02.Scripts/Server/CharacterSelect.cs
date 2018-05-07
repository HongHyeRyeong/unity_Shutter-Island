using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour {

    public GameObject survivor;
    public GameObject murderer;

    static public bool SelSur = false;
    static public bool SelMur = false;

	public void OnClickSurvivor()
    {
        survivor.gameObject.SetActive(true);
        murderer.gameObject.SetActive(false);

        SelSur = true;
    }

    public void OnClickMurderer()
    {
        survivor.gameObject.SetActive(false);
        murderer.gameObject.SetActive(true);

        SelMur = true;
    }
}
