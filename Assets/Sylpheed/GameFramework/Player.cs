using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the persistent data for each player through PlayerStates
/// Player can be referenced from the GameMode or from the created Pawn
/// </summary>
public sealed class Player : MonoBehaviour
{
    public static Player Local => GameInstance.LocalPlayer;
    
    private Dictionary<System.Type, PlayerState> statesDict = new Dictionary<System.Type, PlayerState>();
    public IEnumerable<PlayerState> States { get { return statesDict.Values; } }

    public bool IsLocal => this == Local;

    private void Awake()
    {
        var states = GetComponentsInChildren<PlayerState>();
        foreach (PlayerState state in states)
        {
            statesDict[state.GetType()] = state;
            state.Owner = this;
        }
    }

    /// <summary>
    /// Gets a PlayerState. Object must be registered.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
	public T GetState<T>() where T : PlayerState
    {
        PlayerState obj;
        if (statesDict.TryGetValue(typeof(T), out obj))
        {
            // Clear registered state if it was destroyed
            if (!obj) statesDict.Remove(typeof(T));
        }

        return obj as T;
    }
}

public static class PlayerExtension
{
    #region Extensions
    public static Player GetPlayer(this MonoBehaviour obj)
    {
        return obj.GetComponent<Pawn>().Player;
    }

    public static Player GetPlayer(this GameObject obj)
    {
        return obj.GetComponent<Pawn>().Player;
    }
    #endregion
}
