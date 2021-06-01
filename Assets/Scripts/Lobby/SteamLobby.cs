using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using Mirror;

public class SteamLobby : MonoBehaviour
{
    [SerializeField] Button hostButton;

    private const string HostAddressKey = "HostAdress";

    private Callback<LobbyCreated_t> _lobbyCreated;
    private Callback<GameLobbyJoinRequested_t> _lobbyJoinRequested;
    private Callback<LobbyEnter_t> _lobbyEntered;

    public static CSteamID LobbyId { get; private set; }

    private void Start()
    {
        if (SteamManager.Initialized)
        {
            _lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            _lobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
            _lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }
    }

    public void HostLobby()
    {
        hostButton.interactable = false;

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, NetworkManagerHW.Singleton.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t _callback)
    {
        if (_callback.m_eResult != EResult.k_EResultOK)
        {
            hostButton.interactable = true;
            return;
        }

        LobbyId = new CSteamID(_callback.m_ulSteamIDLobby);

        // start to host the game and send our host address to the Steam servers, if the lobby was created successfully
        NetworkManagerHW.Singleton.StartHost();
        SteamMatchmaking.SetLobbyData(LobbyId, HostAddressKey, SteamUser.GetSteamID().ToString());

        Debug.Log($"Steam Lobby {LobbyId} Created".Color("cyan"));
    }

    private void OnLobbyJoinRequested(GameLobbyJoinRequested_t _callback)
    {
        SteamMatchmaking.JoinLobby(_callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t _callback)
    {
        // Get the host address from the Steam server and start out client, if we are not the host
        if (!NetworkServer.active)
        {
            string _hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(_callback.m_ulSteamIDLobby), HostAddressKey);

            LobbyId = new CSteamID(_callback.m_ulSteamIDLobby);
            NetworkManagerHW.Singleton.networkAddress = _hostAddress;
            NetworkManagerHW.Singleton.StartClient();
        }
    }
}
