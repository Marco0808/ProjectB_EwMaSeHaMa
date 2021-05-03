using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterData", menuName = "Custom/Character Data")]
public class CharacterData : ScriptableObject
{
    [SerializeField]  Color color;
    [SerializeField]  Sprite portrait;
    [SerializeField]  Sprite playerCharacter;

    public Color Color => color;
    public Sprite Portrait => portrait;
    public Sprite PlayerCharacter => playerCharacter;
}
