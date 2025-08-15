using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

namespace Sylpheed.GameFramework
{
    public sealed class GameInstance : MonoBehaviour
    {
        private static GameInstance _instance;
        
        [SerializeField] private string _firstScene;
        [SerializeField] private GameObject _playerPrefab;

        public static Player LocalPlayer => _instance.Player;
        public static GameMode ActiveGameMode => _instance._activeGameMode;

        public string FirstScene => _firstScene;
        public GameObject PlayerPrefab => _playerPrefab;
        public Player Player { get; private set; }

        private GameMode _activeGameMode;

        private void Awake()
        {
            Assert.IsNull(_instance, "There can only be one GameInstance");
            _instance = this;

            // Instantiate player
            Assert.IsNotNull(PlayerPrefab, "PlayerPrefab must be set in the inspector");
            Player = Instantiate(PlayerPrefab.GetComponent<Player>());
            Player.name = PlayerPrefab.name;
        }

        private void Start()
        {
            StartCoroutine(StartTask());
        }

        private IEnumerator StartTask()
        {
            yield return null;
            if (!string.IsNullOrEmpty(FirstScene)) yield return LoadGameMode(FirstScene);
        }

        public static T GetActiveGameMode<T>() where T : GameMode
            => _instance._activeGameMode as T;

        /// <summary>
        /// Loads the scene for the specified GameMode
        /// This unloads active GameMode
        /// </summary>
        /// <param name="gameMode">Must be included in BuildSettings</param>
        /// <param name="data">Optional data to be provided to the GameMode</param>
        /// <returns></returns>
        public static Coroutine LoadGameMode(string gameMode, IDictionary<string, object> data = null)
        {
            return _instance.StartCoroutine(LoadGameModeTask(gameMode, data));
        }

        private static IEnumerator LoadGameModeTask(string gameMode, IDictionary<string, object> data)
        {
            Assert.IsFalse(string.IsNullOrEmpty(gameMode), "GameMode id is null");

            // Unload active game mode
            if (_instance._activeGameMode)
            {
                yield return _instance._activeGameMode.Unload();
                yield return SceneManager.UnloadSceneAsync(_instance._activeGameMode.gameObject.scene);
                _instance._activeGameMode = null;
            }

            Resources.UnloadUnusedAssets();
            yield return null;
            GC.Collect();
            yield return null;

            // Load next game mode
            yield return SceneManager.LoadSceneAsync(gameMode, LoadSceneMode.Additive);

            // Configure active game mode
            _instance._activeGameMode = FindAnyObjectByType<GameMode>();
            _instance._activeGameMode.Data = data;
        }

        public static Coroutine LoadFirstMode()
        {
            return LoadGameMode(_instance.FirstScene);
        }
    }
}