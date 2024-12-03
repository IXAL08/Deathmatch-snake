using FishNet.Transporting.KCP.Edgegap;
using FishNet.Transporting.KCP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Transporting;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text;

public class EdgegapAutoConnect : MonoBehaviour
{
    public static ApiResponse apiResponse;
    [SerializeField] EdgegapKcpTransport kcpTransport;
    [SerializeField] string relayToken;
    const string kEdgegapBaseUrl = "https://api.edgegap.com/v1";
    HttpClient httpClient = new HttpClient();
    uint localUserToken;
    bool isLocalHost;
    string sessionActualID;

    private void Start()
    {
        if(apiResponse == null) return;

        kcpTransport.OnClientConnectionState += OnClientConnectionStateChange;
        uint userToken = 0;
        if (apiResponse.session_users != null)
        {
            userToken = apiResponse.session_users[0].authorization_token;
            isLocalHost = true;
        }
        else
        {
            userToken = apiResponse.session_user.authorization_token;
            isLocalHost = false;
        }
        localUserToken = userToken;

        EdgegapRelayData relayData = new EdgegapRelayData(
        apiResponse.relay.ip,
        apiResponse.relay.ports.server.port,
        apiResponse.relay.ports.client.port,
        userToken,
        apiResponse.authorization_token
        );
        sessionActualID = apiResponse.session_id;

        kcpTransport.SetEdgegapRelayData(relayData);
        if (isLocalHost) kcpTransport.StartConnection(true);
        kcpTransport.StartConnection(false);
    }

    private void OnDestroy()
    {
        if(kcpTransport)
            kcpTransport.OnClientConnectionState -= OnClientConnectionStateChange;
    }

    void OnClientConnectionStateChange(ClientConnectionStateArgs args)
    {
        switch (args.ConnectionState)
        {
            case LocalConnectionState.Stopped:
                print("Cliente desconectado");
                QuitMatch();
                
                break;
            case LocalConnectionState.Starting:
                break;
            case LocalConnectionState.Started:
                print("Cliente conectado");
                break;
            case LocalConnectionState.Stopping:
                break;
        }
    }

    async Task QuitMatch()
    {
        if (!string.IsNullOrEmpty(sessionActualID))
        {
            if (isLocalHost)
            {
                await DeleteMatch(sessionActualID);
            }
            else
            {
                await LeaveMatch();
            }
            sessionActualID = null;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Online");
        }
    }

    async Task DeleteMatch(string session_id)
    {
        HttpResponseMessage responseMessage = await httpClient.DeleteAsync($"{kEdgegapBaseUrl}/relays/sessions/{session_id}");
        string response = await responseMessage.Content.ReadAsStringAsync();
    }

    public async Task LeaveMatch()
    {
        LeaveSession leaveSessionData = new LeaveSession()
        {
            session_id = sessionActualID,
            authorization_token = localUserToken
        };

        string leaveSessionJson = JsonUtility.ToJson(leaveSessionData);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", relayToken);
        HttpContent content = new StringContent(leaveSessionJson, Encoding.UTF8, "application/json");
        await httpClient.PostAsync($"{kEdgegapBaseUrl}/relays/sessions:revoke-user", content);
    }
}
