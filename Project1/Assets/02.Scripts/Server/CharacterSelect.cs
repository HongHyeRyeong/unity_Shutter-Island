using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour {

    public GameObject survivor;
    public GameObject murderer;

    public GameObject Surbtn1;
    public GameObject Surbtn2;
    public GameObject Surbtn3;
    public GameObject Murbtn1;
    public GameObject Murbtn2;

    static public bool SelSur = false;

	public void OnClickSurvivor()
    {
        survivor.gameObject.SetActive(true);
        murderer.gameObject.SetActive(false);

        Surbtn1.gameObject.SetActive(true);
        Surbtn2.gameObject.SetActive(true);
        Surbtn3.gameObject.SetActive(true);
        Murbtn1.gameObject.SetActive(false);
        Murbtn2.gameObject.SetActive(false);

        SelSur = true;
    }

    public void OnClickMurderer()
    {
        survivor.gameObject.SetActive(false);
        murderer.gameObject.SetActive(true);

        Murbtn1.gameObject.SetActive(true);
        Murbtn2.gameObject.SetActive(true);;
        Surbtn1.gameObject.SetActive(false);
        Surbtn2.gameObject.SetActive(false);
        Surbtn3.gameObject.SetActive(false);

        SelSur = false;
    }
}
