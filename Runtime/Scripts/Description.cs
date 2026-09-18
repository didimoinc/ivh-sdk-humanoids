using UnityEngine;

namespace Didimo.IVH.Humanoids
{
    /// <summary>
    /// Holds human-readable metadata about the humanoid this component is attached to
    /// (name, version, use case and character type).
    /// </summary>
    public class Description : MonoBehaviour
    {
        [TextArea(3, 10)]
        [SerializeField] private string description;

        /// <summary>
        /// Returns the description text authored on this humanoid's prefab.
        /// </summary>
        public string GetDescription()
        {
            return description;
        }
    }
}
