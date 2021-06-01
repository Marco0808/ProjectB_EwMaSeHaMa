using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;

public class InvitableSteamFriend : MonoBehaviour
{
    [SerializeField] RawImage profileImage;
    [SerializeField] TMP_Text usernameText;
    [SerializeField] GameObject alreadyInvitedIcon;

    private Callback<AvatarImageLoaded_t> avatarImageLoaded;

    private CSteamID _steamFriendId;

    private void Start()
    {
        avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);

        alreadyInvitedIcon.SetActive(false);
    }

    public void Initalize(CSteamID steamFriendId)
    {
        _steamFriendId = steamFriendId;

        usernameText.text = SteamFriends.GetFriendPersonaName(steamFriendId);

        int imageId = SteamFriends.GetLargeFriendAvatar(steamFriendId);
        if (imageId == -1) return;
        profileImage.texture = GetSteamImageAsTexture(imageId);
    }

    public void InviteFriend()
    {
        if (SteamLobby.LobbyId.m_SteamID != 0 && SteamMatchmaking.InviteUserToLobby(SteamLobby.LobbyId, _steamFriendId))
        {
            alreadyInvitedIcon.SetActive(true);
        }
    }

    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID == _steamFriendId)
        {
            profileImage.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
    }

    private Texture2D GetSteamImageAsTexture(int imageId)
    {
        Texture2D texture = null;

        if (SteamUtils.GetImageSize(imageId, out uint width, out uint height))
        {
            byte[] image = new byte[width * height * 4];

            if (SteamUtils.GetImageRGBA(imageId, image, (int)(width * height * 4)))
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, false);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }

        return texture;
    }
}
