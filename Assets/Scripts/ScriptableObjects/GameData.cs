using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GameData", menuName = "Custom/Game Data")]
public class GameData : ScriptableObject
{
    [SerializeField]  CharacterData[] availableCharacters;

    public CharacterData[] AvailableCharacters => availableCharacters;
}
