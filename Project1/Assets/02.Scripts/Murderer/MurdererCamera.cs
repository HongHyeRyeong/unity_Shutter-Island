using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MurdererCamera : MonoBehaviour
{
    private Transform tr;
    Transform targetMurderer;
    Transform targetCam;
    
    private float MouseY;

    void Start()
    {
        tr = GetComponent<Transform>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (targetMurderer)
        {
            if (targetMurderer.GetComponent<MurdererCtrl>().GetState() == 2)
            {
                tr.transform.position = targetCam.transform.position;
                tr.rotation = Quaternion.Euler(
                    targetCam.transform.eulerAngles.x, targetMurderer.transform.eulerAngles.y, 0);
            }
            else
            {
                tr.transform.position = new Vector3(
                    targetCam.transform.position.x, targetMurderer.transform.position.y + 2, targetCam.transform.position.z);

                MouseY -= Input.GetAxis("Mouse Y") * Time.deltaTime * 100;
                MouseY = ClampAngle(MouseY, -30, 40);

                tr.rotation = Quaternion.Euler(MouseY, targetMurderer.eulerAngles.y, 0);
            }
        }
        else
        {
            try
            {
                targetMurderer = GameObject.Find("Murderer").transform;
                targetCam = GameObject.Find("MurdererCamPivot").transform;
            }
            catch (NullReferenceException ex)
            {
                print(ex);
            }
        }
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
