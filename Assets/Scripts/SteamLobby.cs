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

    private NetworkManagerHousework _networkManager;

    private void Start()
    {
        _networkManager = GetComponent<NetworkManagerHousework>();

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

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, _networkManager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t _callback)
    {
        if (_callback.m_eResult != EResult.k_EResultOK)
        {
            hostButton.interactable = true;
            return;
        }

        // start to host the game and send our host address to the Steam servers, if the lobby was created successfully
        _networkManager.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(_callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());

        Debug.Log("Steam Lobby Created".Color("cyan"));
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

            _networkManager.networkAddress = _hostAddress;
            _networkManager.StartClient();
        }
    }
}
