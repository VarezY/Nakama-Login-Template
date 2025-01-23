using System.Collections;
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
            StartCoroutine(DelayHide());
        }

        private IEnumerator DelayHide()
        {
            yield return new WaitForSeconds(2);
            lobbyPanel.SetActive(false);
        }
    }
}