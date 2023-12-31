using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using Utils;

namespace World.Network
{
    public class NetworkSpawnerService : MonoBehaviour, INetworkRunnerCallbacks
    {
        private NetworkRunnerService _nrs;

        public void SetNetworkRunnerToken(NetworkRunnerService nrs)
        {
            _nrs = nrs;
        }
        
        private int GetPlayerToken(NetworkRunner runner, PlayerRef playerRef)
        {
            if (runner.LocalPlayer == playerRef)
            {
                return ConnectionTokenUtils.HashToken(_nrs.connectionToken);
            }

            var token = runner.GetPlayerConnectionToken(playerRef);

            if (token != null)
                return ConnectionTokenUtils.HashToken(token);
                
            Utils.Utils.DebugLogError("GetPlayerToken returned invalid token");

            return 0;
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                var playerToken = GetPlayerToken(runner, player);
                
                Utils.Utils.DebugLog($"OnPlayerJoined we are server. Connection token {playerToken}");
                
                Utils.Utils.DebugLog($"Spawning new player for connection token {playerToken}");

                _nrs.IsPlayerJoined = true;
            }
            else
            {
                Utils.Utils.DebugLog("OnPlayerJoined");
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Utils.Utils.DebugLog("OnPlayerLeft");
            
            _nrs.IsPlayerJoined = false;
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Utils.Utils.DebugLog("OnShutdown");
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            Utils.Utils.DebugLog("OnConnectedToServer");
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Utils.Utils.DebugLog("OnDisconnectedFromServer");
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            Utils.Utils.DebugLog("OnConnectRequest");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Utils.Utils.DebugLog("OnConnectFailed");
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
            
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            
        }
    }
}