using System;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Object = UnityEngine.Object;

namespace World.Network
{
    public class NetworkRunnerService
    {
        public NetworkRunnerService(NetworkRunner networkRunnerPrefab, NetworkRunner networkRunner)
        {
            if (networkRunner == null)
            {
                networkRunner = Object.Instantiate(networkRunnerPrefab);
                networkRunner.name = "Network runner";
                
                networkRunner.GetComponent<NetworkSpawnerService>().SetNetworkRunnerToken(this);

                var gameMode = GameMode.Client;

#if UNITY_EDITOR
                gameMode = GameMode.AutoHostOrClient;
#elif UNITY_SERVER
                gameMode = GameMode.Server;
#endif
                connectionToken = ConnectionTokenUtils.NewToken();

                Utils.Utils.DebugLog($"Player connection token {ConnectionTokenUtils.HashToken(connectionToken)}");

                InitializeNetworkRunner(networkRunner, gameMode, connectionToken, "TestSession", NetAddress.Any(),
                    SceneManager.GetActiveScene().buildIndex, null);

                Utils.Utils.DebugLog("Server NetworkRunner started");
            }
        }
        
        public bool IsPlayerJoined { get; set; }
        
        public byte[] connectionToken { get; set; }

        private void InitializeNetworkRunner(NetworkRunner runner, GameMode mode, byte[] connectionToken,
            string sessionName,
            NetAddress address, SceneRef sceneRef, Action<NetworkRunner> initialized)
        {
            var sceneManager = GetSceneManager(runner);

            runner.ProvideInput = true;

            Utils.Utils.DebugLog("InitializeNetworkRunner done");

            runner.StartGame(new StartGameArgs
            {
                GameMode = mode,
                Address = address,
                Scene = sceneRef,
                SessionName = sessionName,
                SceneManager = sceneManager,
                Initialized = initialized,
                ConnectionToken = connectionToken
            });
        }

        private INetworkSceneManager GetSceneManager(NetworkRunner runner)
        {
            var sceneManager =
                runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault() ??
                runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

            return sceneManager;
        }
    }
}