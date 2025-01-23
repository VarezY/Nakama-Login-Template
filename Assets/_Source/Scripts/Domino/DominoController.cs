using System;
using UnityEngine;
namespace Hige.Domino
{
    public class DominoController : MonoBehaviour
    {
        private MeshRenderer _dominoMesh;

        private void Awake()
        {
            _dominoMesh = GetComponent<MeshRenderer>();
        }

        public void ChangeDominoTexture(Material newTexture)
        {
            _dominoMesh.material = newTexture;
        }
    }
}