using System.Collections.Generic;
using UnityEngine;
namespace Hige.Domino
{
    public class CustomDominoManager : MonoBehaviour
    {
        public GameObject dominoPrefab;
        public List<GameObject> dominoes = new List<GameObject>(28);

        private void Start()
        {
            dominoes.Shuffle();
        }
    }
    
    public static class DominoTileExtension
    {
        private static void Swap<T>(this List<T> list, int index1, int index2)
        {
            (list[index1], list[index2]) = (list[index2], list[index1]);
        }

        public static void Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            System.Random rnd = new System.Random();
            for (int i = 0; i < n; i++)
            {
                int j = (rnd.Next(0, n) % n);
                list.Swap(i, j);
            }
        }
    }
}
