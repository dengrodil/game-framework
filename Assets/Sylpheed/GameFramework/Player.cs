using UnityEngine;
using System.Collections.Generic;

namespace Sylpheed.GameFramework
{
    /// <summary>
    /// Manages the persistent data for each player through PlayerStates
    /// Player can be referenced from the GameMode or from the created Pawn
    /// </summary>
    public sealed class Player : MonoBehaviour
    {
        public static Player Local => GameInstance.LocalPlayer;
    
        private readonly Dictionary<System.Type, PlayerState> _statesDict = new();
        public IEnumerable<PlayerState> States => _statesDict.Values;

        public bool IsLocal => this == Local;

        private void Awake()
        {
            var states = GetComponentsInChildren<PlayerState>();
            foreach (var state in states)
            {
                _statesDict[state.GetType()] = state;
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
            if (_statesDict.TryGetValue(typeof(T), out var obj))
            {
                // Clear registered state if it was destroyed
                if (!obj) _statesDict.Remove(typeof(T));
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
}
