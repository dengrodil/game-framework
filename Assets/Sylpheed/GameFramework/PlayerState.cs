using UnityEngine;
using System.Collections;

/// <summary>
/// Persistent data bound to a Player
/// PlayerState does not get destroyed between scene loads
/// </summary>
public abstract class PlayerState : MonoBehaviour
{
    [Header("Serialization")]
    public string SerializationKey;

    // Should only be modified by the owning Player
    public Player Owner { get; set; }

    public T GetSiblingState<T>()
        where T : PlayerState
    {
        return Owner.GetState<T>();
    }
}
