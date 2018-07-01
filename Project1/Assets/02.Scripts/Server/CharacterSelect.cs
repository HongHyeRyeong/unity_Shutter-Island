using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour {

    public GameObject survivor;
    public GameObject murderer;

    public GameObject Surbtn;
    public GameObject Murbtn;

    static public bool SelSur = false;

	public void OnClickSurvivor()
    {
        SelSur = true;

        survivor.gameObject.SetActive(SelSur);
        Surbtn.gameObject.SetActive(SelSur);

        murderer.gameObject.SetActive(!SelSur);
        Murbtn.gameObject.SetActive(!SelSur);
    }

    public void OnClickMurderer()
    {
        SelSur = false;

        survivor.gameObject.SetActive(SelSur);
        Surbtn.gameObject.SetActive(SelSur);

        murderer.gameObject.SetActive(!SelSur);
        Murbtn.gameObject.SetActive(!SelSur);
    }
}
