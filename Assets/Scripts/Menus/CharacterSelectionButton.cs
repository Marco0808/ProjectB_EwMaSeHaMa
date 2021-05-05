using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionButton : MonoBehaviour
{
    [SerializeField] private Image characterPortrait;

    public static event Action<CharacterData> OnCharacterSelected;

    private CharacterData _character;

    public void SetCharacter(CharacterData character)
    {
        _character = character;
        characterPortrait.sprite = character.Portrait;
    }

    public void CharacterSelected()
    {
        OnCharacterSelected?.Invoke(_character);
    }
}
