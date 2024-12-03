using FishNet;
using FishNet.Connection;
using FishNet.Demo.AdditiveScenes;
using FishNet.Managing;
using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public List<NetworkObject> Players = new List<NetworkObject>();
    public GameObject Inicio, SetUp;
    public bool Start, Game, Final;

    private void Awake()
    {
        Instance = this;
        Start = true;
    }

    private void Update()
    {
        if (Start && Players.Count < 2)
        {
            Inicio.SetActive(true);
        }
        else if (Start && Players.Count >= 2) { 
            Inicio.SetActive(false);
            SetUp.SetActive(true);
            float timer = 0;
            timer += timer * Time.deltaTime;
            if (timer >= 30) {
                SetUp.SetActive(false);
                Start = false;
                Game = true;
            }
        }
    }

}
