using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionButton : MonoBehaviour
{
    [SerializeField] Image portrait;

    public static event Action<CharacterData> OnCharacterSelected;

    private CharacterData _character;

    public void CharacterSelected()
    {
        OnCharacterSelected?.Invoke(_character);
    }

    private void Initialize(CharacterData character)
    {
        _character = character;
        portrait.sprite = character.Portrait;
    }

}
