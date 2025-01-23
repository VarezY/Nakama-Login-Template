using System.Collections.Generic;
using Hige.Network;
using Hige.UI;
using Nakama;
using PimDeWitte.UnityMainThreadDispatcher;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
namespace Hige.Game
{
	public class PlayerLobby : MonoBehaviour
	{
		[Title("Lobby Configuration")]
		public GameObject lobbyPrefabs;
		public Transform lobbyParentTarget;
		public TMP_Text lobbyText;
		
		#region Private Parameter
		
		private NetworkManager _networkManager;
		private Dictionary<string, GameObject> _players = new Dictionary<string, GameObject>();
		private int _playerIndex;

		#endregion

		private void Start()
		{
			_networkManager = NetworkManager.Instance;
			SpawnPlayerThatHasAlreadyInLobby();
			SpawnYourself();
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
				Debug.Log($"{user.Username} <color=purple>already in Lobby</color>");
				SpawnPlayerPresence(user);
			}
		}
		
		private async void SocketOnReceivedMatchPresence(IMatchPresenceEvent matchPresence)
		{
			foreach (IUserPresence presence in matchPresence.Joins)
			{
				if (_players.ContainsKey(presence.UserId))
					return;
				Debug.Log($"{presence.Username} <color=purple>joining a Lobby</color>");
				SpawnPlayerPresence(presence);
			}

			foreach (IUserPresence presence in matchPresence.Leaves)
			{
				if (!_players.TryGetValue(presence.SessionId, out GameObject player))
					continue;
				Destroy(player);
				_players.Remove(presence.SessionId);
			}
		}
		
		private void SpawnPlayerPresence(IUserPresence user)
		{
			_playerIndex++;
			GameObject temp = Instantiate(lobbyPrefabs, lobbyParentTarget);
			temp.GetComponent<PlayerLobbyController>().ChangeUsername(_playerIndex % 2 == 0 ? $"{user.Username} - {_playerIndex} - <color=Blue>T1</color>" : $"{user.Username} - {_playerIndex} - <color=Red>T2</color>");
			_players.Add(user.SessionId, temp);
			int lobbyManagerMaxPlayerLobby = _networkManager.LobbyManager.maxPlayerLobby;
			lobbyText.text = $"Number of player {_players.Count}/{lobbyManagerMaxPlayerLobby}";

			if (_playerIndex == lobbyManagerMaxPlayerLobby)
			{
				GameStart();
			}
		}
		
		private void GameStart()
		{
			GameManager.Instance.MatchManager.StartGame();
		}
	}
}
