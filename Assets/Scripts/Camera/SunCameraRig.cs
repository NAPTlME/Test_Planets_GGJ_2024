using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunCameraRig : MonoBehaviour
{
    public bool isFocused;

    public float minFov = 5;
    public float maxFov = 10;
    public float zoomSpeed = 1;

    public Cinemachine.CinemachineVirtualCamera mainCamera;
    public Cinemachine.CinemachineVirtualCamera miniCamera;

    private float currentFov;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
        currentFov = maxFov;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFocused) {
            currentFov -= Input.mouseScrollDelta.y * zoomSpeed;

            currentFov = Mathf.Clamp(currentFov, minFov, maxFov);

            mainCamera.m_Lens.FieldOfView = currentFov;
        }
    }
}
