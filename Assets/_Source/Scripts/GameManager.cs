using System.Collections.Generic;
using Hige.Network;
using Hige.UI;
using Nakama;
using PimDeWitte.UnityMainThreadDispatcher;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Hige
{
	public class GameManager : MonoBehaviour
	{
		public GameObject prefabs;
		public GameObject localPrefab, remotePrefab;
		
		public Transform target;
		public TMP_Text lobbyText;
		
		#region Private Parameter
		
		private NetworkManager _networkManager;
		private Dictionary<string, GameObject> _players = new Dictionary<string, GameObject>();

		#endregion

		private void Start()
		{
			_networkManager = NetworkManager.Instance;
			SpawnYourself();
			SpawnPlayerThatHasAlreadyInLobby();
			_networkManager.Socket.ReceivedMatchPresence += matchEvent => UnityMainThreadDispatcher.Instance().Enqueue(() => SocketOnReceivedMatchPresence(matchEvent));
		}

		private void SpawnYourself()
		{
			SpawnPlayerPresence(_networkManager.LobbyManager.CurrentMatch.Self);
		}
		
		private void SpawnPlayerThatHasAlreadyInLobby()
		{
			foreach (IUserPresence user in _networkManager.LobbyManager.CurrentMatch.Presences)
			{
				Debug.Log($"{user.UserId}, {user.Username} <color=purple>already in Lobby</color>");
				SpawnPlayerPresence(user);
			}
		}
		
		private void SocketOnReceivedMatchPresence(IMatchPresenceEvent matchPresence)
		{
			foreach (IUserPresence presence in matchPresence.Joins)
			{
				// Spawn a player for this presence and store it in a dictionary by session id.
				Debug.Log($"{presence.Username} <color=purple>joining a Lobby</color>");
				SpawnPlayerPresence(presence);
			}

			// For each player that has left in this event...
			foreach (IUserPresence presence in matchPresence.Leaves)
			{
				// Remove the player from the game if they've been spawned
				if (!_players.TryGetValue(presence.SessionId, out GameObject player))
					continue;
				
				Destroy(player);
				_players.Remove(presence.SessionId);

			}
		}
		
		
		private void SpawnPlayerPresence(IUserPresence user)
		{
			GameObject temp = Instantiate(prefabs, target);
			temp.GetComponent<PlayerLobbyController>().ChangeUsername(user.Username);
			_players.Add(user.SessionId, temp);
			lobbyText.text = $"Number of player {_players.Count}/{_networkManager.LobbyManager.maxPlayerLobby}";
			if (_players.Count == _networkManager.LobbyManager.maxPlayerLobby)
			{
				GameStart();
			}
		}
		
		private void GameStart()
		{
			Debug.Log($"<color=green>GAME START!</color>");
		}
	}
}
