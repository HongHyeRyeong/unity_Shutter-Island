using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    int Character;

    // Survivor
    public Transform targetSurvivorComPivot;

    float dist = 5.0f;
    float height = 3f;

    //Murderer
    public Transform targetMurderer;
    public Transform targetMurdererCamPivot;

    private float MouseY;

    void LateUpdate()
    {
        if (Character == 1)
        {
            if (targetSurvivorComPivot)
            {
                dist -= 0.5f * Input.mouseScrollDelta.y;

                if (dist < 4) dist = 4;
                else if (dist >= 7) dist = 7;

                Vector3 pos = targetSurvivorComPivot.position;
                pos.y += 1.5f;

                height -= Input.GetAxis("Mouse Y") * Time.deltaTime * 10;
                height = ClampAngle(height, 0, 4);

                transform.position = Vector3.Lerp(transform.position,
                    pos - (targetSurvivorComPivot.forward * dist) + (Vector3.up * height),
                    Time.deltaTime * 20.0f);

                transform.LookAt(pos);
            }
        }
        else if (Character == 2)
        {
            if (targetMurdererCamPivot)
            {
                if (targetMurderer.GetComponent<MurdererCtrl>().GetState() == 0 || targetMurderer.GetComponent<MurdererCtrl>().GetState() == 1)
                {
                    transform.position = new Vector3(
                        targetMurdererCamPivot.transform.position.x, 
                        targetMurderer.transform.position.y + 2.2f, 
                        targetMurdererCamPivot.transform.position.z);

                    MouseY -= Input.GetAxis("Mouse Y") * Time.deltaTime * 100;
                    MouseY = ClampAngle(MouseY, -30, 40);

                    transform.rotation = Quaternion.Euler(MouseY, targetMurderer.eulerAngles.y, 0);
                }
                else
                {
                    transform.position = targetMurdererCamPivot.transform.position;
                    transform.rotation = Quaternion.Euler(
                        targetMurdererCamPivot.transform.eulerAngles.x, targetMurderer.transform.eulerAngles.y, 0);

                    MouseY = targetMurdererCamPivot.transform.eulerAngles.x;
                }
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

    public void SetCharacter(int character)
    {
        Character = character;
    }
}
