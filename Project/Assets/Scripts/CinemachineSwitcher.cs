using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera VirtualCamera;

    public CinemachineFreeLook freeLookCam;

    public bool usingFreeLook = false;



    // Start is called before the first frame update
    void Start()
    {
        if (Input.GetMouseButtonDown(1))
        {
            usingFreeLook = !usingFreeLook;
            if (usingFreeLook)
            {
                freeLookCam.Priority = 20;
                VirtualCamera.Priority = 0;
            }
            else
            {
                VirtualCamera.Priority = 20;
                freeLookCam.Priority= 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
