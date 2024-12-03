using FishNet.Transporting;
using FishNet.Transporting.KCP.Edgegap;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class EdgegapRelayManager : MonoBehaviour
{
    [SerializeField] string relayToken;
    [SerializeField] EdgegapKcpTransport kcpTransport;
    const string kEdgegapBaseUrl = "https://api.edgegap.com/v1";
    HttpClient httpClient = new HttpClient();
    [SerializeField] Transform partidaItemContainer;
    [SerializeField] GameObject partidaItemPrefab;
    bool isLocalHost;
    string sessionActualID;
    uint localUserToken;

    void Start()
    {
        kcpTransport.OnServerConnectionState += OnServerConnectionStateChange;
        kcpTransport.OnClientConnectionState += OnClientConnectionStateChange;
        ResfreshMatches();
        EdgegapAutoConnect.apiResponse = null;
    }


    public async void CreateGame()
    {
        //StartCoroutine(GetIP());
        await CreateGameAsync();
    }
    void OnServerConnectionStateChange(ServerConnectionStateArgs args)
    {
        switch (args.ConnectionState)
        {
            case LocalConnectionState.Stopped:
                print("Servidor detenido");
                break;
            case LocalConnectionState.Starting: 
                break;
            case LocalConnectionState.Started:
                print("Servidor iniciado");
                break;
            case LocalConnectionState.Stopping:
                break;

        }
    }

    void OnClientConnectionStateChange(ClientConnectionStateArgs args)
    {
        switch (args.ConnectionState)
        {
            case LocalConnectionState.Stopped:
                print("Cliente desconectado");
                QuitMatch();
                ResfreshMatches();
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

    void QuitMatch()
    {
        if (!string.IsNullOrEmpty(sessionActualID))
        {
            if (isLocalHost)
            {
                DeleteMatch(sessionActualID);
            }
            else
            {
                LeaveMatch();
            }
            sessionActualID = null;
        }
    }

    private void OnApplicationQuit()
    {
        QuitMatch();
    }

    public async Task CreateGameAsync()
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token",relayToken);
        HttpResponseMessage responseMessage = await httpClient.GetAsync($"{kEdgegapBaseUrl}/ip");
        string response = await responseMessage.Content.ReadAsStringAsync();
        UserIP userIP = JsonUtility.FromJson<UserIP>(response);
        
        Users users = new Users {
            users = new List<User>()
        };
        users.users.Add(new User() {ip = userIP.public_ip });

        string usersJson = JsonUtility.ToJson(users);
        HttpContent content = new StringContent(usersJson, Encoding.UTF8, "application/json");
        responseMessage = await httpClient.PostAsync($"{kEdgegapBaseUrl}/relays/sessions", content);
        response = await responseMessage.Content.ReadAsStringAsync();
        ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(response);
        print("Session: " + apiResponse.session_id);

        while (!apiResponse.ready) { 
            await Task.Delay(2500);
            responseMessage = await httpClient.GetAsync($"{kEdgegapBaseUrl}/relays/sessions/{apiResponse.session_id}");
            response = await responseMessage.Content.ReadAsStringAsync();
            apiResponse = JsonUtility.FromJson<ApiResponse>(response);
        }

        //ConnectToMatch(apiResponse); 
        EdgegapAutoConnect.apiResponse = apiResponse;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Online");
    }

    void ConnectToMatch(ApiResponse apiResponse)
    {
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

    async Task WaitAndHost(string session_id)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", relayToken);
        HttpResponseMessage responseMessage = await httpClient.GetAsync($"{kEdgegapBaseUrl}/relays/sessions/{session_id}");
        string response = await responseMessage.Content.ReadAsStringAsync();
        print(response);
    }

    public async void ResfreshMatches()
    {
        await GetAllMatchesAsync();
    }
    async Task GetAllMatchesAsync()
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", relayToken);
        HttpResponseMessage responseMessage = await httpClient.GetAsync($"{kEdgegapBaseUrl}/relays/sessions");
        string response = await responseMessage.Content.ReadAsStringAsync();

        Sessions sessions = JsonUtility.FromJson<Sessions>(response);
        RefreshMatchesListUI(sessions);
    }

    async Task DeleteMatch(string session_id)
    {
        HttpResponseMessage responseMessage = await httpClient.DeleteAsync($"{kEdgegapBaseUrl}/relays/sessions/{session_id}");
        string response = await responseMessage.Content.ReadAsStringAsync();
    }

    [ContextMenu("Borrar todas las partidas")]
    async void DevDeleteAllMatches()
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", relayToken);
        HttpResponseMessage responseMessage = await httpClient.GetAsync($"{kEdgegapBaseUrl}/relays/sessions");
        string response = await responseMessage.Content.ReadAsStringAsync();
        print(response);

        Sessions sessions = JsonUtility.FromJson<Sessions>(response);
        foreach (ApiResponse partida in sessions.sessions)
        {
            await DeleteMatch(partida.session_id);
        }
        print("Todas las partidas fueron borradas");
    }

    void RefreshMatchesListUI(Sessions sessions)
    {
        foreach (Transform child in partidaItemContainer)
        {
            Destroy(child.gameObject);
        }

        foreach(ApiResponse partidaData in sessions.sessions)
        {
            GameObject newitem = Instantiate(partidaItemPrefab, partidaItemContainer);
            PartidaItem partidaItem = newitem.GetComponent<PartidaItem>();
            partidaItem.SetUp(partidaData,this);
        }
    }

    public async Task JoinMatch(string session_id)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", relayToken);
        HttpResponseMessage responseMessage = await httpClient.GetAsync($"{kEdgegapBaseUrl}/ip");
        string response = await responseMessage.Content.ReadAsStringAsync();
        UserIP userIP = JsonUtility.FromJson<UserIP>(response);

        JoinSession joinSessionData = new JoinSession()
        {
            session_id = session_id,
            user_ip = userIP.public_ip
        };

        string usersJson = JsonUtility.ToJson(joinSessionData);
        HttpContent content = new StringContent(usersJson, Encoding.UTF8, "application/json");
        responseMessage = await httpClient.PostAsync($"{kEdgegapBaseUrl}/relays/sessions:authorize-user", content);
        response = await responseMessage.Content.ReadAsStringAsync();
        ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(response);

        //ConnectToMatch(apiResponse);
        EdgegapAutoConnect.apiResponse = apiResponse;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Online");

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
    /*IEnumerator GetIP()
    {
        using UnityWebRequest unityWebRequest = new UnityWebRequest($"{kEdgegapBaseUrl}/ip", "GET");
        unityWebRequest.SetRequestHeader("Authorization", relayToken);
        unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al obtener IP");
            Debug.LogError(unityWebRequest.error);
            yield break;
        }

        string response = unityWebRequest.downloadHandler.text;
        print(response);
        UserIP userIP = JsonUtility.FromJson<UserIP>(response);
        print(userIP.public_ip);
    }
    IEnumerator CreateGameCoroutines()
    {
        using UnityWebRequest unityWebRequest = new UnityWebRequest($"{kEdgegapBaseUrl}/relays/sessions", "POST");
        unityWebRequest.SetRequestHeader("Authorization", relayToken);

        yield return unityWebRequest.SendWebRequest();

        if(unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al crear partida");
            Debug.LogError(unityWebRequest.error);
            yield break;
        }

        string response = unityWebRequest.downloadHandler.text;
        print(response);

        ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(response);
    }*/

}
