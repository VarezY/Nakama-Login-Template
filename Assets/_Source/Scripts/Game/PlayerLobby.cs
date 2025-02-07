using System.Collections.Generic;
using System.Linq;
using Hige.Network;
using Hige.UI;
using Nakama;
using Nakama.TinyJson;
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
		private Dictionary<string, string> _playersIndex = new Dictionary<string, string>();
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
			GivePlayerIndex();
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
			_playersIndex.Add(user.UserId, user.Username);
			
			int lobbyManagerMaxPlayerLobby = _networkManager.LobbyManager.maxPlayerLobby;
			lobbyText.text = $"Number of player {_players.Count}/{lobbyManagerMaxPlayerLobby}";

			if (_playerIndex == lobbyManagerMaxPlayerLobby)
			{
				GameStart();
			}
		}

		private async void GivePlayerIndex()
		{
			PlayerIndex playerIndex = new PlayerIndex
			{
				IndexPlayer = _playerIndex,
			};

			WriteStorageObject writeObject = new WriteStorageObject
			{
				Collection = "PlayerIndex",
				Key = "MatchIndex",
				Value = JsonWriter.ToJson(playerIndex),
				PermissionRead = 2, // Only the server and owner can read
				PermissionWrite = 1, // The server and owner can write
			};

			await _networkManager.Client.WriteStorageObjectsAsync(_networkManager.Session, new[] { writeObject });
		}
		
		private void GameStart()
		{
			ReadPlayerIndex();
			// GameManager.Instance.MatchManager.StartGame();
		}
		
		private async void ReadPlayerIndex()
		{
			/*foreach (StorageObjectId readObjectId in _playersId.Select(id => new StorageObjectId
				{
					Collection = "PlayerIndex",
					Key = "MatchIndex",
					UserId = id,
				}))
			{
				IApiStorageObjects result = await _networkManager.Client.ReadStorageObjectsAsync(_networkManager.Session, new [] { readObjectId });

				if (!result.Objects.Any())
					return;
			
				var storageObject = result.Objects.First();
				var playerIndex = JsonParser.FromJson<PlayerIndex>(storageObject.Value);
				Debug.LogFormat($"<color=orange>Player</color> ");
			}*/
			foreach (KeyValuePair<string,string> keyValue in _playersIndex)
			{
				var readObjectId = new StorageObjectId
				{
					Collection = "PlayerIndex",
					Key = "MatchIndex",
					UserId = keyValue.Key,
				};
			
				IApiStorageObjects result = await _networkManager.Client.ReadStorageObjectsAsync(_networkManager.Session, new [] { readObjectId });

				if (!result.Objects.Any())
					return;
			
				var storageObject = result.Objects.First();
				var playerIndex = JsonParser.FromJson<PlayerIndex>(storageObject.Value);
				Debug.LogFormat(playerIndex.IndexPlayer % 2 == 0 ? $"<color=Orange>{keyValue.Value}</color> Team: <color=red>Red</color>" : $"<color=Orange>{keyValue.Value}</color> Team: <color=cyan>Blue</color>");
				// Debug.LogFormat($"<color=Orange>{keyValue.Value}</color> Index: {playerIndex.IndexPlayer}");
			}
		}
	}
	
	public class PlayerIndex
	{
		public int IndexPlayer;
	}
}
