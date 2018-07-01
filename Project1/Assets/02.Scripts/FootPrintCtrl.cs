using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPrintCtrl : MonoBehaviour
{
    float UseTime = 5.0f;

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

        this.gameObject.SetActive(false);
    }
}
