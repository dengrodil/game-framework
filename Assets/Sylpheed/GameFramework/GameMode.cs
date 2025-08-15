using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sylpheed.GameFramework
{
    /// <summary>
    /// GameMode handles the rules and lifecycle of a scene
    /// GameMode instantiates a Pawn that is bound to a Player
    /// </summary>
    public abstract class GameMode : MonoBehaviour
    {
        [SerializeField] private GameObject _pawnPrefab;

        /// <summary>
        /// List of all additive scenes that are bundled with the GameMode's main scene
        /// </summary>
        [SerializeField] private string[] _additiveScenes = System.Array.Empty<string>();

        public string Id => gameObject.scene.name;

        /// <summary>
        /// Representation of the player's object
        /// </summary>
        public Pawn Pawn { get; private set; }

        /// <summary>
        /// Storage of data provided by the previous GameMode
        /// </summary>
        public IDictionary<string, object> Data { get; set; }

        #region Overridables

        /// <summary>
        /// GameMode is still loading
        /// Additive scenes are already loaded at this point
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator OnLoad() { yield break; }

        /// <summary>
        /// GameMode is about to unload
        /// You can invoke saving of data as the game will wait for this coroutine to finish.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator OnUnload() { yield break; }

        /// <summary>
        /// Initialize the instantiated pawn
        /// Pawn is still in its Awake state.
        /// </summary>
        /// <param name="pawn"></param>
        protected virtual void OnInitializePawn(Pawn pawn) { }

        /// <summary>
        /// Parse data for loading this GameMode
        /// </summary>
        /// <param name="data"></param>
        protected virtual void OnParseData(IDictionary<string, object> data) { }

        /// <summary>
        /// Default data provided when a previous GameMode didn't provide one.
        /// This is useful for initial data and for testing
        /// </summary>
        /// <returns></returns>
        protected virtual IDictionary<string, object> OnWriteDefaultData() => new Dictionary<string, object>();

        #endregion

        #region Static Methods

        public static GameMode Active => GameInstance.ActiveGameMode;

        public static T GetActive<T>() where T : GameMode 
            => GameInstance.ActiveGameMode as T;

        public static bool IsGameMode<T>() where T : GameMode 
            => GameInstance.ActiveGameMode is T;

        #endregion

        private void Start()
        {
            StartCoroutine(LoadTask());
        }

        private IEnumerator LoadTask()
        {
            SceneManager.SetActiveScene(gameObject.scene);

            // Create default data if nothing was provided
            Data ??= OnWriteDefaultData();

            // Parse data
            OnParseData(Data);

            // Create the pawn
            var pawnObj = _pawnPrefab ? Instantiate(_pawnPrefab) : new GameObject();
            pawnObj.name = _pawnPrefab?.name ?? "DefaultPawn";
            if (!_pawnPrefab) pawnObj.transform.SetParent(transform);
            #if UNITY_EDITOR
            if (!_pawnPrefab) Debug.LogWarning(gameObject.scene.name + " did not create a Pawn. Creating an empty Pawn...");
            #endif

            Pawn = pawnObj.AddComponent<Pawn>();
            Pawn.Player = Player.Local;
            OnInitializePawn(Pawn);

            // Load additive scenes
            foreach (var sceneName in _additiveScenes)
            {
                yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }

            yield return null;

            yield return StartCoroutine(OnLoad());
        }

        public Coroutine Unload()
        {
            return StartCoroutine(UnloadTask());
        }

        private IEnumerator UnloadTask()
        {
            yield return StartCoroutine(OnUnload());

            // Unload additive scenes
            foreach (var sceneName in _additiveScenes)
            {
                yield return SceneManager.UnloadSceneAsync(sceneName);
            }
        }
    }
}