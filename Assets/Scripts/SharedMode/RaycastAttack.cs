using System;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Utils.Locations
{
    public class RaycastAttack : NetworkBehaviour
    {
        public float damage = 10;

        public PlayerMovement playerMovement;

        private void Update()
        {
            if(HasStateAuthority == false)
                return;

            var ray = playerMovement.playerCamera.ScreenPointToRay(Mouse.current.position.value);
            ray.origin += playerMovement.playerCamera.transform.forward;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Debug.DrawRay(ray.origin, ray.direction, Color.red, 1f);
                if (Runner.GetPhysicsScene().Raycast(ray.origin,ray.direction, out var hit))
                {
                    Debug.Log(hit.transform.name);
                    if (hit.transform.TryGetComponent<Health>(out var health))
                    {
                        health.DealDamageRpc(damage);
                    }
                }
            }
        }
    }
}