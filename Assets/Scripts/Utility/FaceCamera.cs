using UnityEngine;
using NaughtyAttributes;

[ExecuteInEditMode]
public class FaceCamera : MonoBehaviour
{
    [Tooltip("Camera facing is automatically updated in the editor and on game start.\nYou can't rotate the object if this is enabled.")]
    [SerializeField] private bool autoUpdateFacing = true;

    private void Update()
    {
        Quaternion cameraRotation = Camera.main.transform.rotation;
        if (!Application.isPlaying && autoUpdateFacing && transform.rotation != cameraRotation)
            transform.rotation = cameraRotation;
    }

    private void Start()
    {
        if (autoUpdateFacing) UpdateFaceCamera();
    }

    [Button("Reset Facing to Camera", EButtonEnableMode.Always)]
    private void UpdateFaceCamera()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
