using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Nakama;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
namespace Hige.Network
{
    public class LobbyManager : MonoBehaviour
    {
        public event Action onMatchFound;
        private NetworkManager _networkManager;
        
        public IApiMatchList MatchList{ get; private set; }
        public IMatch CurrentMatch { get; private set; }
        public IMatchmakerMatched CurrentMatchmaker { get; private set; }

        private void Start()
        {
            _networkManager = NetworkManager.Instance;
            _networkManager.onConnected.AddListener(() =>
            {
                _networkManager.Socket.ReceivedMatchmakerMatched += OnReceivedMatchmakerMatched;
            });
        }

        public async Task StartQuickMatch()
        {
            try
            {
                var ticket = await _networkManager.Socket.AddMatchmakerAsync(
                    query: "*",
                    minCount: 2,
                    maxCount: 2);
                
                Debug.Log($"<color=orange>Added to Matchmaking:</color> {ticket.Ticket}");
            }
            catch (ApiResponseException ex)
            {
                Debug.LogError($"<color=red>Error starting matchmaking</color>: {ex.Message}");
            }
        }
        
        public async void SearchLobby()
        {
            const int minPlayers = 4;
            const int maxPlayers = 4;
            const int limit = 10;
            const bool authoritative = false;
            const string label = "";
            const string query = "";
            IApiMatchList result = await _networkManager.Client.ListMatchesAsync(_networkManager.Session, minPlayers, maxPlayers, limit, authoritative, label, query);

            if (result.Matches.Count() > 0)
            {
                JoinLobby();
            }
            else
            {
                CreateLobby();
            }
            
            /*foreach (IApiMatch match in result.Matches)
            {
                Debug.LogFormat("{0}: {1}/10 players", match.MatchId, match.Size);
            }*/        
        }

        public void JoinLobby()
        {
            
        }

        public void CreateLobby()
        {
            
        }
        
        private async void OnReceivedMatchmakerMatched(IMatchmakerMatched matchmaker)
        {
            Debug.Log($"<color=cyan>Match Found!</color>");
            CurrentMatchmaker = matchmaker;
            try
            {
                CurrentMatch = await _networkManager.Socket.JoinMatchAsync(matchmaker);
                Debug.Log($"<color=cyan>Successfully joined match with ID</color>: {CurrentMatch.Id} {matchmaker.Users}");
                await LoadSceneAsync();
                onMatchFound?.Invoke();
            }
            catch (ApiResponseException ex)
            {
                Debug.LogError($"<color=red>Error joining matchmaking</color>: {ex.Message}");
            }
        }
        
        private async Task LoadSceneAsync()
        {
            var operation = SceneManager.LoadSceneAsync("_Source/Scenes/GamePage");
        
            // Optional loading progress
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                Debug.Log($"Loading progress: {progress:P0}");
                await Task.Yield();
            } 
        }
    }
}