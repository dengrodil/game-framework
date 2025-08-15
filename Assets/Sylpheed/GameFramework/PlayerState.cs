using UnityEngine;
using System.Collections;

namespace Sylpheed.GameFramework
{
    /// <summary>
    /// Persistent data bound to a Player
    /// PlayerState does not get destroyed between scene loads
    /// </summary>
    public abstract class PlayerState : MonoBehaviour
    {
        [Header("Serialization")]
        [SerializeField] private string _serializationKey;

        public string SerializationKey => _serializationKey;
        // Should only be modified by the owning Player
        public Player Owner { get; set; }

        public T GetSiblingState<T>()
            where T : PlayerState
        {
            return Owner.GetState<T>();
        }
    }
}