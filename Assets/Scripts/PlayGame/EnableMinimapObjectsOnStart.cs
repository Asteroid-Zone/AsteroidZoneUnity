using System;
using System.Linq;
using UnityEngine;

namespace PlayGame
{
    public class EnableMinimapObjectsOnStart : MonoBehaviour
    {
        private void Start()
        {
            (FindObjectsOfType(typeof(GameObject))
                    as GameObject[] ?? Array.Empty<GameObject>())
                .Where(o => o.layer == LayerMask.NameToLayer("Minimap"))
                .ToList()
                .ForEach(o => o.GetComponent<MeshRenderer>().enabled = true);
        }
    }
}
