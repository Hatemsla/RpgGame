using Fusion;
using UnityEngine;

namespace Utils.Locations
{
    public class Health : NetworkBehaviour
    {
        [Networked]
        public float networkedHealth { get; set; } = 100;

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void DealDamageRpc(float damage)
        {
            Debug.Log("Received DealDamageRpc on StateAuthority, modifying Networked variable");
            networkedHealth -= damage;
        }
    }
}