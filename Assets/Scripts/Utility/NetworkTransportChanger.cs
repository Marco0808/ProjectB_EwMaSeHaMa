using UnityEngine;
using Mirror;
using TMPro;

[RequireComponent(typeof(NetworkManagerHW), typeof(SteamLobby))]
public class NetworkTransportChanger : MonoBehaviour
{
    [SerializeField] private Transport steamworksTransport;
    [SerializeField] private Transport localTransport;
    [SerializeField] private TMP_InputField networkAdressField;

    private NetworkManagerHW _networkManager;

    private void Awake()
    {
        _networkManager = GetComponent<NetworkManagerHW>();
    }

    public void SteamworksHost()
    {
        Transport.activeTransport = steamworksTransport;
        GetComponent<SteamLobby>().HostLobby();
    }

    public void LocalHost()
    {
        Transport.activeTransport = localTransport;
        _networkManager.networkAddress = networkAdressField.text;
        _networkManager.StartHost();
    }

    public void LocalJoin()
    {
        Transport.activeTransport = localTransport;
        _networkManager.networkAddress = networkAdressField.text;
        _networkManager.StartClient();
    }
}
