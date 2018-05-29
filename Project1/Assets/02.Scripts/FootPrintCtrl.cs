using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPrintCtrl : MonoBehaviour
{
    float UseTime = 5.0f;

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        if (this.gameObject.activeSelf)
        {
            UseTime -= Time.deltaTime;

            if (UseTime < 3)
            {
                float tempY = this.gameObject.transform.position.y - 0.01f;
                this.gameObject.transform.position = new Vector3(
                    this.gameObject.transform.position.x, tempY, this.gameObject.transform.position.z);
            }

            if(UseTime < 0)
            {
                UseTime = 5.0f;
                this.gameObject.SetActive(false);
            }

        }
    }
}
