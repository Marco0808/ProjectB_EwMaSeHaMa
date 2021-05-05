using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GameData", menuName = "Custom/Game Data")]
public class GameData : ScriptableObject
{
    [SerializeField] CharacterData[] availableCharacters;

    public CharacterData[] AvailableCharacters => availableCharacters;

    public CharacterData GetCharacterById(int characterId)
    {
        return availableCharacters[characterId];
    }

    public int GetCharacterId(CharacterData character)
    {
        for (int i = 0; i < availableCharacters.Length; i++)
            if (availableCharacters[i] == character)
                return i;
        return 0;
    }
}
