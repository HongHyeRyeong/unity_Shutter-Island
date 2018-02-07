using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private float PlayerMoveSpeed = 0;
    float rotationSpeed = 50f;

    void Update()
    {
        if (PlayerMoveSpeed == 0)
        {
            PlayerMoveSpeed = GameObject.Find("Player").GetComponent<PlayerState>().MoveSpeed;
        }

        float horizontal = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, horizontal, 0);

        float vertical = Input.GetAxis("Vertical") * PlayerMoveSpeed * Time.deltaTime;
        transform.Translate(0, 0, vertical); ;
    }

    //private float h = 0.0f;
    //private float v = 0.0f;

    //private Transform tr;

    //float Movespeed = 5.0f;
    //float Rotspeed = 150.0f;

    //void Start()
    //{
    //    tr = GetComponent<Transform>();
    //}

    //void Update()
    //{
    //    h = Input.GetAxis("Horizontal");
    //    v = Input.GetAxis("Vertical");

    //    Vector3 move = (Vector3.forward * v) + (Vector3.right * h);

    //    tr.Translate(move * Time.deltaTime * Movespeed, Space.Self);
    //    tr.Rotate(Vector3.up * Time.deltaTime * Rotspeed * Input.GetAxis("Mouse X"));
    //}
}