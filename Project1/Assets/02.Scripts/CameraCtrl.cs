using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    public static CameraCtrl instance;

    [HideInInspector]
    public Camera MainCam;
    private CameraFilter_Attack FilterAttack;

    [SerializeField]
    private Material SkyboxDay;
    [SerializeField]
    private Material SkyboxNight;

    // Survivor
    [HideInInspector]
    public Transform targetSurvivorComPivot;

    private float dist = 5.0f;
    private float height = 3f;

    //Murderer
    [HideInInspector]
    public Transform targetMurderer;
    [HideInInspector]
    public Transform targetMurdererCamPivot;

    private int saveState = -1;
    private float MouseY;

    private void Awake()
    {
        instance = this;
        MainCam = GetComponent<Camera>();
        FilterAttack = GetComponent<CameraFilter_Attack>();

        // SkyBox
        int sky = Random.Range(1, 3);

        if (sky == 1)
            gameObject.AddComponent<Skybox>().material = SkyboxDay;
        else
            gameObject.AddComponent<Skybox>().material = SkyboxDay;
    }

    void LateUpdate()
    {
        if (GameCtrl.instance.Character == 1)
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
        else if (GameCtrl.instance.Character == 2)
        {
            if (targetMurdererCamPivot)
            {
                int state = targetMurderer.GetComponent<MurdererCtrl>().GetState();

                if (state == 0)
                {
                    if (saveState != state)
                    {
                        saveState = state;
                        MainCam.cullingMask = -1;
                    }

                    transform.position = Vector3.Lerp(transform.position, new Vector3(
                        targetMurdererCamPivot.transform.position.x,
                        targetMurderer.transform.position.y + 2.4f,
                        targetMurdererCamPivot.transform.position.z), Time.deltaTime * 15);

                    MouseY -= Input.GetAxis("Mouse Y") * Time.deltaTime * 100;
                    MouseY = ClampAngle(MouseY, -30, 60);
                    transform.rotation = Quaternion.Euler(MouseY, targetMurderer.eulerAngles.y, 0);
                }
                else if (state == 1)
                {
                    if (saveState != state)
                    {
                        saveState = state;
                        MainCam.cullingMask = ~(1 << 10);
                    }

                    transform.position = Vector3.Lerp(transform.position, new Vector3(
                        targetMurdererCamPivot.transform.position.x,
                        targetMurderer.transform.position.y + 2.4f,
                        targetMurdererCamPivot.transform.position.z), Time.deltaTime * 15);

                    MouseY -= Input.GetAxis("Mouse Y") * Time.deltaTime * 100;
                    MouseY = ClampAngle(MouseY, -30, 60);
                    transform.rotation = Quaternion.Euler(MouseY, targetMurderer.eulerAngles.y, 0);
                }
                else
                {
                    if (saveState != state)
                    {
                        saveState = state;
                        MainCam.cullingMask = -1;
                    }

                    transform.position = targetMurdererCamPivot.transform.position;

                    float angleX = 20;

                    if (state == 3)
                        angleX = 60;

                    transform.rotation = Quaternion.Euler(
                        targetMurdererCamPivot.transform.eulerAngles.x + angleX,
                        targetMurderer.transform.eulerAngles.y, 0);

                    MouseY = transform.localRotation.x + angleX;
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

    public IEnumerator Attack(float delay)
    {
        FilterAttack.Fade = 0;

        while (FilterAttack.Fade <= 1)
        {
            FilterAttack.Fade += Time.deltaTime * 0.5f;
            yield return null;
        }

        yield return new WaitForSeconds(delay);

        FilterAttack.Fade = 1;

        while (FilterAttack.Fade >= 0)
        {
            FilterAttack.Fade -= Time.deltaTime * 0.3f;
            yield return null;
        }
    }
}
