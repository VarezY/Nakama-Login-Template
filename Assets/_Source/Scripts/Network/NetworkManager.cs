using System;
using System.Text;
using System.Threading.Tasks;
using Nakama;
using PimDeWitte.UnityMainThreadDispatcher;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.UI;
using UnityEngine.SceneManagement;
namespace Hige.Network
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance;
        
        [Title("Server Configuration")]
        [SerializeField] private string scheme = "http";
        [SerializeField] private string host;
        [SerializeField] private string serverKey = "defaultKey";
        [SerializeField] private int port = 80;

        [Title("Events")]
        public UnityEvent onConnecting;
        public UnityEvent onConnected;
        public UnityEvent onDisconnected;
        
        [Title("Network UI")]
        [SerializeField] private GameObject loadingScreen;

        public ISocket Socket { get; private set; }
        public ISession Session { get; private set; }
        public IClient Client { get; private set; }
        public IApiAccount CurrentAccount { get; private set; }
        public IMatch CurrentMatch { get; private set; } 
        public IMatchmakerMatched CurrentMatchmaker { get; private set; }
 
        public NetworkUser NetworkUser { get; private set; }
        public LobbyManager LobbyManager { get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(this);

            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }

            NetworkUser = GetComponent<NetworkUser>();
            LobbyManager = GetComponent<LobbyManager>();
        }

        private void Start()
        {
            Connect();
        }

        public async void AuthenticateWithDevice(string deviceId, string newUsername = null)
        {
            try
            {
                Session = await Client.AuthenticateDeviceAsync(deviceId);
            }
            catch (ApiResponseException ex)
            {
                Debug.Log($"<color=red>Error authenticating device: </color> {ex.StatusCode}:{ex.Message}");
                return;
            }
            
            Debug.Log($"<color=green>Authenticated with Device</color>");
            CreateSocket();

            if (string.IsNullOrEmpty(newUsername))
                return;
            
            NetworkUser.UpdateUser(newUsername);
        }

        public async void AuthenticateWithCustomId(string newUsername)
        {
            int byteCount = Encoding.ASCII.GetByteCount(newUsername);
            string paddedUsername = newUsername;
            
            if (byteCount < 6)
                paddedUsername = newUsername.PadRight(6, '*'); //Padded for Nakama custom ID (min id Bytes is 6) 

            try
            {
                Session = await Client.AuthenticateCustomAsync(paddedUsername);
            }
            catch (ApiResponseException ex)
            {
                Debug.Log($"<color=red>Error authenticating device: </color> {ex.StatusCode}:{ex.Message}");
                return;
            }
            
            Debug.Log($"<color=green>Authenticated with CustomID</color>");
            CreateSocket();
            
            NetworkUser.UpdateUser(newUsername);
        }
        
        public async void AuthenticateWithGoogle()
        {
            const string playerIdToken = "...";

            try
            {
                Session = await Client.AuthenticateGoogleAsync(playerIdToken);
            }
            catch (ApiResponseException ex)
            {
                Debug.Log($"<color=red>Error authenticating device: </color> {ex.StatusCode}:{ex.Message}");
                return;
            }
            
            Debug.Log($"<color=green>Authenticated with Google</color>");
            CreateSocket();
        }
        
        private void Connect()
        {
            Client = new Client(scheme, host, port, serverKey);
            Client.GlobalRetryConfiguration = new RetryConfiguration(1, 5, OnRetry);;

            Debug.Log($"<color=green>Connected to Nakama</color>");
        }

        private async void CreateSocket()
        {
            Socket = Nakama.Socket.From(Client);
            try
            {
                await Socket.ConnectAsync(Session, true, 30).ContinueWith(completeTask =>
                {
                    Debug.Log($"<color=green>Connect Completed</color>");
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch (ApiResponseException ex)
            {
                Debug.Log($"<color=red>Error Connecting to Session: </color> {ex.StatusCode}:{ex.Message}");
                return;
            }
            GetAccount();
        }

        private async void GetAccount()
        {
            CurrentAccount = await Client.GetAccountAsync(Session, new RetryConfiguration(1, 5, (retry, retry1) =>
            {
                Debug.Log($"<color=red>Error</color>: User is not found!");
            }));
            onConnected?.Invoke();
        }
        
        private static void OnRetry(int numretry, Retry retry)
        {
            Debug.LogWarning($"<color=red>Something is wrong, about to try again. Number tries: {numretry}</color>");
        }
    }
}