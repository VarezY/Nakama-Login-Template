using Hige.Network;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Hige
{
	public class GameManager : MonoBehaviour
	{
		public GameObject prefabs;
		public Transform target;
		
		#region Private Parameter
		private NetworkManager _networkManager;

		#endregion
	
		private void Awake()
		{
		
		}
	
		void Start()
		{
			_networkManager = NetworkManager.Instance;
			_networkManager.LobbyManager.onMatchFound += LobbyManagerOnMatchFound;
		}
		private void LobbyManagerOnMatchFound()
		{
			Debug.Log($"AAAAAAAAAAAAAAAAA");
			Instantiate(prefabs, target);
		}

		void Update()
		{
        
		}
	
		#region Private Methods
	
		#endregion
	}
}
