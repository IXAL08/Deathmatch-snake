using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartidaItem : MonoBehaviour
{
    [SerializeField] Text nombrePartida;
    [SerializeField] Text numeroJugadores;
    EdgegapRelayManager edgeRelayManager;
    
    public void SetUp(ApiResponse apiResponse, EdgegapRelayManager relayManager)
    {
        nombrePartida.text = apiResponse.session_id;
        numeroJugadores.text = apiResponse.session_users.Length.ToString();
        edgeRelayManager = relayManager;
    }

    public async void UnirPartida()
    {
        await edgeRelayManager.JoinMatch(nombrePartida.text);
    }
}
