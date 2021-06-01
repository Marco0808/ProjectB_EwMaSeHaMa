using UnityEngine;

public class HideInGame : MonoBehaviour
{
    [SerializeField] private bool hideInGame = true;

    void Start()
    {
        gameObject.SetActive(!hideInGame);
    }
}
