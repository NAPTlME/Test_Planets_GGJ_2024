using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineFreeLook))]
public class PlanetCam : MonoBehaviour
{
    CinemachineFreeLook vCamera;

    private float yAxis_MaxSpeed = 2;
    private float xAxis_MaxSpeed = 300;
    // Start is called before the first frame update
    void Start()
    {
        vCamera = GetComponent<CinemachineFreeLook>();
        GlobalManager.getInstance().cameraManager.OnMainCameraChanged += CameraManager_OnMainCameraChanged;
        CameraManager_OnMainCameraChanged(GlobalManager.getInstance().cameraManager.MainCameraType);
    }
    private void OnDestroy()
    {
        GlobalManager.getInstance().cameraManager.OnMainCameraChanged -= CameraManager_OnMainCameraChanged;
    }

    private void CameraManager_OnMainCameraChanged(CameraType obj)
    {
        if (obj.Equals(CameraType.Orbital))
        {
            vCamera.m_XAxis.m_MaxSpeed = xAxis_MaxSpeed;
            vCamera.m_YAxis.m_MaxSpeed = yAxis_MaxSpeed;
        } else
        {
            // take current max speeds and save
            xAxis_MaxSpeed = vCamera.m_XAxis.m_MaxSpeed;
            yAxis_MaxSpeed = vCamera.m_YAxis.m_MaxSpeed;
            vCamera.m_XAxis.m_MaxSpeed = 0f;
            vCamera.m_YAxis.m_MaxSpeed = 0f;
        }
    }
}
