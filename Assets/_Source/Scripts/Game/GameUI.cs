using UnityEngine;
namespace Hige.Game
{
    public class GameUI : MonoBehaviour
    {
        public GameObject lobbyPanel;
        
        private void Start()
        {
            GameManager.Instance.MatchManager.OnStartGame += MatchManagerOnStartGame;
        }

        private void OnDisable()
        {
            GameManager.Instance.MatchManager.OnStartGame -= MatchManagerOnStartGame;
        }

        private void MatchManagerOnStartGame()
        {
            Debug.Log("Hide Lobby");
            lobbyPanel.SetActive(false);
        }
    }
}