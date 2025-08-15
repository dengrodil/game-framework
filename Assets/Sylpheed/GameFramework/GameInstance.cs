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
        private static GameInstance instance;

        public static Player LocalPlayer
        {
            get { return instance.Player; }
        }

        public static GameMode ActiveGameMode
        {
            get { return instance.activeGameMode; }
        }

        public string FirstScene;
        public GameObject PlayerPrefab;

        public Player Player { get; private set; }

        private GameMode activeGameMode;

        private void Awake()
        {
            Assert.IsNull(instance, "There can only be one GameInstance");
            instance = this;

            // Instantiate player
            Assert.IsNotNull(PlayerPrefab, "PlayerPrefab must be set in the inspector");
            Player = Instantiate(PlayerPrefab.GetComponent<Player>());
            Player.name = PlayerPrefab.name;
        }

        private void Start()
        {
            StartCoroutine(StartTask());
        }

        IEnumerator StartTask()
        {
            yield return null;
            if (!string.IsNullOrEmpty(FirstScene)) yield return LoadGameMode(FirstScene);
        }

        public static T GetActiveGameMode<T>()
            where T : GameMode
        {
            return instance.activeGameMode as T;
        }

        /// <summary>
        /// Loads the scene for the specified GameMode
        /// This unloads active GameMode
        /// </summary>
        /// <param name="gameMode">Must be included in BuildSettings</param>
        /// <param name="data">Optional data to be provided to the GameMode</param>
        /// <returns></returns>
        public static Coroutine LoadGameMode(string gameMode, IDictionary<string, object> data = null)
        {
            return instance.StartCoroutine(LoadGameModeTask(gameMode, data));
        }

        static IEnumerator LoadGameModeTask(string gameMode, IDictionary<string, object> data)
        {
            Assert.IsFalse(string.IsNullOrEmpty(gameMode), "GameMode id is null");

            // Unload active game mode
            if (instance.activeGameMode)
            {
                yield return instance.activeGameMode.Unload();
                yield return SceneManager.UnloadSceneAsync(instance.activeGameMode.gameObject.scene);
                instance.activeGameMode = null;
            }

            Resources.UnloadUnusedAssets();
            yield return null;
            GC.Collect();
            yield return null;

            // Load next game mode
            yield return SceneManager.LoadSceneAsync(gameMode, LoadSceneMode.Additive);

            // Configure active game mode
            instance.activeGameMode = FindAnyObjectByType<GameMode>();
            instance.activeGameMode.Data = data;
        }

        public static Coroutine LoadFirstMode()
        {
            return LoadGameMode(instance.FirstScene);
        }
    }
}