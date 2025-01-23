using Hige.Domino;
using UnityEngine;
namespace Hige.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        public PlayerLobby PlayerLobby { get; private set; }
        public MatchManager MatchManager { get; private set; }
        public CustomDominoManager DominoManager { get; private set; }
        
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }

            PlayerLobby = GetComponentInChildren<PlayerLobby>();
            MatchManager = GetComponentInChildren<MatchManager>();
            DominoManager = GetComponentInChildren<CustomDominoManager>();
        }
    }
}