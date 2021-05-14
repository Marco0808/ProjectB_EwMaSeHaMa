using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New CharacterData", menuName = "Housework/Character Data")]
public class CharacterData : ScriptableObject
{
    [SerializeField] private string characterName = "John Doe";
    [SerializeField, ResizableTextArea] private string backgroundStory = "Lorem ipsum dolor sit amet consectetur adipisicing elit.";
    [SerializeField] private Color color;
    [SerializeField, ShowAssetPreview] private Sprite portrait;
    [SerializeField, ShowAssetPreview] private Sprite questInterfaceHand;
    [SerializeField] private RuntimeAnimatorController playerAnimatorController; 

    public string CharacterName => characterName;
    public string BackgroundStory => backgroundStory;
    public Color Color => color;
    public Sprite Portrait => portrait;
    public Sprite QuestInterfaceHand => questInterfaceHand;
    public RuntimeAnimatorController PlayerAnimatorController => playerAnimatorController; 
}
