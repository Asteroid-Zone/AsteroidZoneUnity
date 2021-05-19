using System;
using System.Linq;
using Statics;
using UnityEngine;

namespace PlayGame.UI {
    
    /// <summary>
    /// Finds all GameObjects on the Minimap Layer.
    /// For each GameObject it enables the MeshRenderer and disables the Collider.
    /// </summary>
    public class EnableMinimapObjectsOnStart : MonoBehaviour {
        
        private void Start() {
            (FindObjectsOfType(typeof(GameObject))
                    as GameObject[] ?? Array.Empty<GameObject>())
                .Where(o => o.layer == LayerMask.NameToLayer(Layers.MinimapLayer))
                .ToList()
                .ForEach(o =>
                {
                    o.GetComponent<MeshRenderer>().enabled = true;
                    Collider minimapObjCollider = o.GetComponent<BoxCollider>();
                    if (minimapObjCollider != null)
                    {
                        minimapObjCollider.enabled = false;
                    }
                });
        }
    }
}
