using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using NaughtyAttributes;

[RequireComponent(typeof(Sprite))]
public class TaskObject : NetworkBehaviour
{
    [SerializeField, Required] private TaskObjectData taskObjectData;

    private SpriteRenderer _spriteRenderer;

    public TaskObjectData TaskObjectData => taskObjectData;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _spriteRenderer.sprite = taskObjectData?.ObjectSprite;
    }

    [Button("Update Task Object", EButtonEnableMode.Always)]
    public void UpdateTaskObject()
    {
        _spriteRenderer.sprite = taskObjectData.ObjectSprite;
    }
}
