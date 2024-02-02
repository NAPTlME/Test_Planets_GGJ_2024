using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class currentPlanetIndicator : MonoBehaviour
{
    MeshRenderer meshRenderer;
    private PlanetGravity targetPlanet;
    private CameraType mainCameraType = CameraType.Overhead;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        GlobalManager.getInstance().cameraManager.OnMainCameraChanged += CameraManager_OnMainCameraChanged;
        GravityManager.getInstance().OnActivePlanetChanged += GravityManager_OnActivePlanetChanged;
        meshRenderer.enabled = false;
    }

    private void OnDestroy()
    {
        GlobalManager.getInstance().cameraManager.OnMainCameraChanged -= CameraManager_OnMainCameraChanged;
        GravityManager.getInstance().OnActivePlanetChanged -= GravityManager_OnActivePlanetChanged;
    }

    private void CameraManager_OnMainCameraChanged(CameraType obj)
    {
        mainCameraType = obj;
        CheckTargetAndState();
    }

    private void GravityManager_OnActivePlanetChanged(PlanetGravity planet)
    {
        if (!planet.CompareTag("Sun"))
        {
            targetPlanet = planet;
        }
        else
        {
            targetPlanet = null;
        }
        CheckTargetAndState();
    }

    private void CheckTargetAndState()
    {
        if (mainCameraType.Equals(CameraType.Orbital) && targetPlanet != null && !targetPlanet.CompareTag("Sun"))
        {
            meshRenderer.enabled = true;
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (targetPlanet != null)
        {
            transform.position = targetPlanet.transform.position;
        }
    }
}
