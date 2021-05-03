using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

///<summary>Class that handles the lobby representation UI of the players.</summary>
public class LobbyPlayerPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text displaNameText;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private Button characterSelectionButton;
    [SerializeField] private GameObject leaderIcon;

    public TMP_Text DisplayNameText => displaNameText;
    public Image CharacterPortrait => characterPortrait;
    public Button CharacterSelectionBtton => characterSelectionButton;
    public GameObject LeaderIcon => leaderIcon;

    public void ToggleCharacterSelection(){
        //TODO character selection
    }
}
