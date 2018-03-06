using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorCamera : MonoBehaviour {
    public Transform target;

    //카메라와의 거리
    float dist = 5f;

    //카메라 회전 속도
    float xSpeed = 150.0f;
    float ySpeed = 100.0f;

    //카메라 초기 위치
    private float x;
    private float y;

    //y값 제한
    float yMinLimit = 10f;
    float yMaxLimit = 40f;

    //앵글의 최소,최대 제한
    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void Update()
    {
        if (target)
        {
            //dist -= 1 * Input.mouseScrollDelta.y;

            //if (dist < 40)
            //{
            //    dist = 40;
            //}
            //else if (dist >= 90)
            //{
            //    dist = 90;
            //}

            x += Input.GetAxis("Mouse X") * xSpeed * 0.015f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.015f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * new Vector3(0, 0.0f, -dist) + target.position + new Vector3(0.0f, 0.0f, 0.0f);

            transform.rotation = rotation;
            transform.position = position;
        }
    }
}
