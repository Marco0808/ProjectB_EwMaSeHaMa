using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using NaughtyAttributes;

[RequireComponent(typeof(SpriteRenderer))]
public class TaskObject : NetworkBehaviour
{
    [SerializeField, OnValueChanged("UpdateTaskObject")] private TaskData taskData;
    [SerializeField] private Transform taskMenuRoot;

    [SyncVar] private bool sync_isTrapActive;

    private SpriteRenderer _spriteRenderer;
    private Collider _collider;

    public TaskData TaskData => taskData;
    public Transform TaskMenuRoot => taskMenuRoot;
    public Vector3 TaskPosition => transform.position;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        _spriteRenderer.sprite = taskData.ObjectSprite;
        StartCoroutine(ResetCollider());
    }

    public void SetHighlighted(bool isHighlighted)
    {
        SetSpriteBrightness(isHighlighted ? 1 : 0.5f);
    }

    /// <param name="brightness">Value between 0 and 1. Default brightness is 0.5</param>
    private void SetSpriteBrightness(float brightness)
    {
        Color color = _spriteRenderer.color;
        color.a = Mathf.Clamp01(brightness);
        _spriteRenderer.color = color;
    }

    [Button("Update Task Object", EButtonEnableMode.Always)]
    private void UpdateTaskObject() => GetComponent<SpriteRenderer>().sprite = taskData.ObjectSprite;

    IEnumerator ResetCollider()
    {
        if (_collider) Destroy(_collider);
        yield return null;
        _collider = gameObject.AddComponent<BoxCollider>();
        _collider.isTrigger = true;
    }

    private void OnMouseEnter()
    {
        
    }

    private void OnMouseExit()
    {
        
    }
}
