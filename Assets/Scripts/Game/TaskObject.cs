using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using NaughtyAttributes;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider))]
public class TaskObject : MonoBehaviour
{
    [SerializeField, OnValueChanged("UpdateTaskObject")] private TaskObjectData taskObjectData;

    private SpriteRenderer _spriteRenderer;
    private Collider _collider;

    public TaskObjectData TaskObjectData => taskObjectData;
    public Vector3 TaskPosition => transform.position;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        _spriteRenderer.sprite = taskObjectData.ObjectSprite;
        StartCoroutine(ResetCollider());
    }

    [Button("Update Task Object", EButtonEnableMode.Always)]
    private void UpdateTaskObject() => GetComponent<SpriteRenderer>().sprite = taskObjectData.ObjectSprite;

    IEnumerator ResetCollider()
    {
        Destroy(_collider);
        yield return null;
        _collider = gameObject.AddComponent<BoxCollider>();
    }

    private void OnMouseEnter()
    {
        
    }

    private void OnMouseExit()
    {
        
    }
}
