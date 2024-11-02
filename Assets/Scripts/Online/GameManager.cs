using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public readonly SyncVar<GameObject> food = new SyncVar<GameObject>();
    public readonly SyncVar<GameObject> SpeedUp = new SyncVar<GameObject>();
    public BoxCollider2D GameField;
    public PlayerControllerNetwork Player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
