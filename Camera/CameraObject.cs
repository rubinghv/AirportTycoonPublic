using UnityEngine;
using System.Collections;

public abstract class CameraObject {

    protected CameraParameters cameraParams;

    protected CameraObject(CameraParameters cameraParameters)
    {
        cameraParams = cameraParameters;
    }
    public abstract void UpdateManual(float deltaTime);
}
