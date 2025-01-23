using System;
using UnityEngine;
namespace Hige.Game
{
    public class MatchManager : MonoBehaviour
    {
        public event Action OnStartGame;
        public void StartGame()
        {
            Debug.Log($"<color=green>GAME START!</color>");
            OnStartGame?.Invoke();
        }
    }
}