using UnityEngine;
using System.Collections;
 
[RequireComponent (typeof(MatrixBlender))]
public class PerspectiveSwitcher : MonoBehaviour
{
    private Matrix4x4   ortho,
                        perspective;
    public float        fov     = 60f,
                        near    = .3f,
                        far     = 1000f,
                        orthographicSize = 50f;
    private float       aspect;
    private MatrixBlender blender;
    private bool        orthoOn;
 	public Camera camera;

    void Start()
    {
        aspect = (float) Screen.width / (float) Screen.height;
        ortho = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, near, far);
        perspective = Matrix4x4.Perspective(fov, aspect, near, far);
        camera.projectionMatrix = perspective;
        orthoOn = false;
        blender = (MatrixBlender) GetComponent(typeof(MatrixBlender));
    }
 
    /* 
     *	Switch from perspective to ortographic and back
     */
    public void SwitchPerspective(float seconds)
    {
		orthoOn = !orthoOn;
        if (orthoOn)
            blender.BlendToMatrix(camera, ortho, seconds);
        else
			blender.BlendToMatrix(camera, perspective, seconds);
    }
}