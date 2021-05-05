using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[ExecuteAlways]
public class FaceCamera : MonoBehaviour
{
    [Button("Reset Facing to Camera", EButtonEnableMode.Always)]
    private void Start()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
