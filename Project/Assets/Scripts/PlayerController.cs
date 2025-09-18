using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    public float speed = 12f;
    public float jumpPower = 5f;
    public float gravity = -9.81f;

    private CharacterController controller;

    public float rotationSpeed = 10f;

    private CinemachinePOV pov;

   public CinemachineVirtualCamera virtualCamera;

    private Vector3 velocity;

    public bool isGrounded;

    public CinemachineSwitcher cS;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        pov = virtualCamera.GetCinemachineComponent < CinemachinePOV >();
    }

    // Update is called once per frame
    void Update()
    {


        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }


        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 30;
            virtualCamera.m_Lens.FieldOfView = 80f;
        }
        else
        {
            speed = 12;
            virtualCamera.m_Lens.FieldOfView = 60f;
        }


        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 camForward = virtualCamera.transform.forward;
        camForward.y = 0;
        camForward .Normalize();

        Vector3 camRight = virtualCamera.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        
        Vector3 move = (camForward * z + camRight * x).normalized;
        if (!cS.usingFreeLook)
            controller.Move(move * speed * Time.deltaTime);



        float cameraYaw = pov.m_HorizontalAxis.Value;
        Quaternion targetRot = Quaternion.Euler(0f, cameraYaw, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = jumpPower;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }
}
