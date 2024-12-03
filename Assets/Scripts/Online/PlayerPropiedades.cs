using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPropiedades : NetworkBehaviour
{
    public static PlayerPropiedades Instance { get; private set; }
    public readonly SyncDictionary<NetworkConnection, PlayerProperty> Properties = new();

    public static PlayerProperty LocalProperty;

    void Awake()
    {
        Instance = this;
        InstanceFinder.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        InstanceFinder.ServerManager.OnRemoteConnectionState += ServerManager_OnRemoteConnectionState;
        InstanceFinder.ServerManager.RegisterBroadcast<PlayerProperty>(OnServerPlayerProperty);
    }

    private void ServerManager_OnRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs arg)
    {
        if (arg.ConnectionState != RemoteConnectionState.Stopped)
            return;

        if (Properties.ContainsKey(conn))
        {
            Properties.Remove(conn);
        }
    }

    private void OnDestroy()
    {
        if (InstanceFinder.ClientManager) {

            ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState;
            InstanceFinder.ServerManager.UnregisterBroadcast<PlayerProperty>(OnServerPlayerProperty);
        }

    }

    void OnServerPlayerProperty(NetworkConnection conn ,PlayerProperty playerProperty, Channel channel = Channel.Reliable)
    {
        print($"Se acaba de conectar {conn.ClientId}, su indexColor es: {playerProperty.indexColor}");
        Properties.Add(conn, playerProperty);
    }

    void ClientManager_OnClientConnectionState(ClientConnectionStateArgs arg)
    {
        if (arg.ConnectionState != LocalConnectionState.Started) return;

        int index = PlayerPrefs.GetInt("Color");
        //int index = LocalProperty.indexColor;

        PlayerProperty properties = new PlayerProperty() { 
            indexColor = index,
        };


        InstanceFinder.ClientManager.Broadcast(properties);
    }

    public struct PlayerProperty: IBroadcast
    {
        public int indexColor;
    }
}
