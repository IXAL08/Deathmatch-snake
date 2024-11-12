using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public readonly SyncList<PlayerControllerNetwork> Players = new SyncList<PlayerControllerNetwork>();
    private NetworkManager _networkManager;
    [SerializeField] private PlayerControllerNetwork _playerPrefab;
    public GameObject LoseCanvas, VictoryCanvas;
    
    private void Start()
    {
        InitializeOnce();
    }

    private void Awake()
    {
        Players.OnChange += Players_OnChange;
    }

    private void Players_OnChange(SyncListOperation op, int index, PlayerControllerNetwork oldItem, PlayerControllerNetwork newItem, bool asServer)
    {
        switch (op)
        {
            case SyncListOperation.Add:
                print("Jugador Vivo");
                break;

            case SyncListOperation.RemoveAt:
                break ;
            case SyncListOperation.Complete:
                break;

        }
    }

    void Update()
    {

        print(Players[0].isAlive.Value);
    }

    private void InitializeOnce()
    {
        _networkManager = InstanceFinder.NetworkManager;
        if (_networkManager == null)
        {
            NetworkManagerExtensions.LogWarning($"PlayerSpawner on {gameObject.name} cannot work as NetworkManager wasn't found on this object or within parent objects.");
            return;
        }

        _networkManager.SceneManager.OnClientLoadedStartScenes += SceneManager_OnClientLoadedStartScenes;
    }

    private void SceneManager_OnClientLoadedStartScenes(NetworkConnection conn, bool asServer)
    {
        if (!asServer)
            return;
        if (_playerPrefab == null)
        {
            NetworkManagerExtensions.LogWarning($"Player prefab is empty and cannot be spawned for connection {conn.ClientId}.");
            return;
        }

        Players.Add(_playerPrefab);
    }



    [ServerRpc(RequireOwnership = false)]
    void LoseServerRPC()
    {
        print("Server");
        LoseClientRPC();
    }

    [ObserversRpc]
    void LoseClientRPC()
    {
        LoseCanvas.SetActive(true);
    }
}
