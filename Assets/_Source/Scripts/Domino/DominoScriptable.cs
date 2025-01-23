using UnityEngine;

namespace Hige.Domino
{
    [CreateAssetMenu(fileName = "DominoFactory", menuName = "Scriptables/Dominoes 3D Factory", order = 0)]
    public class DominoScriptable : ScriptableObject
    {
        public Material[] dominoMaterials;

        public Material GetDominoMaterial(int index)
        {
            return index > dominoMaterials.Length ? null : Instantiate(dominoMaterials[index]);
        }
    }
}