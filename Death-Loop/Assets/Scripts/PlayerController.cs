using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    Transform cam;
    Animator anim;

    [Header("Movement")]
    public float acc;
    public int maxSpeed;

    [Header("Camera Look")]
    public float sensitivity;
    public int minY, maxY;
    float xRaw = 0, yRaw = 0;
    public Transform head;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        anim = GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
        PlayerMove();
    }

    void Update()
    {
        CameraLook();
    }

    void PlayerMove()
    {
        if (GameManager.instance.isPaused)
        {
            return;
        }

        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(Input.GetAxis("Vertical") * transform.forward * acc * Time.fixedDeltaTime);
            rb.AddForce(Input.GetAxis("Horizontal") * transform.right * (acc/1.5f) * Time.fixedDeltaTime);
            anim.SetFloat("Speed", rb.velocity.magnitude);
        }
    }

    void CameraLook()
    {
        //Follow the heads position
        cam.position = Vector3.Lerp(cam.position, head.position + (head.forward / 3f) + (transform.up / 4f), 10 * Time.deltaTime);

        if (GameManager.instance.isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        xRaw = Input.GetAxisRaw("Mouse X") * sensitivity;
        yRaw += Input.GetAxisRaw("Mouse Y") * sensitivity;

        yRaw = Mathf.Clamp(yRaw, minY, maxY);

        //Rotate player
        transform.Rotate(0, xRaw, 0);

        //Rotate camera
        cam.eulerAngles = new Vector3(-yRaw, transform.eulerAngles.y, 0);
    }
}
