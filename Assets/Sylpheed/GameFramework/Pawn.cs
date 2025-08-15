using UnityEngine;

namespace Sylpheed.GameFramework
{
    /// <summary>
    /// Serves as a connection to the Player
    /// Pawn is automatically attached by the GameMode
    /// </summary>
    public sealed class Pawn : MonoBehaviour
    {
        /// <summary>
        /// Bound player for the pawn. This is useful if we're handling multiple pawns.
        /// </summary>
        public Player Player { get; set; }

        public static Pawn Local { get { return GameInstance.ActiveGameMode.Pawn; } }
    }

    public static class PawnExtension
    {
        public static bool IsPawn(this MonoBehaviour mono)
        {
            return mono.GetComponent<Pawn>();
        }

        public static bool IsPawn(this GameObject obj)
        {
            return obj.GetComponent<Pawn>();
        }
    }
}
