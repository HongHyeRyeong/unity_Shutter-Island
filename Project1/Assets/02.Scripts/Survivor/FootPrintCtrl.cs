using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPrintCtrl : MonoBehaviour
{
    [SerializeField]
    private PhotonView pv = null;

    private float UseTime = 5.0f;

    private void Start()
    {
        transform.parent = GameCtrl.instance.SurFootPrints.transform;
    }

    public IEnumerator Use()
    {
        float time = UseTime;

        while (true)
        {
            time -= Time.deltaTime;

            if (time < 3)
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y - Time.deltaTime * 0.4f,
                    transform.position.z);

            if (time < 0)
                break;

            yield return null;
        }

        pv.RPC("FootActFalse", PhotonTargets.AllBuffered);
    }

    [PunRPC]
    public void FootActFalse()
    {
        this.gameObject.SetActive(false);
    }
}
