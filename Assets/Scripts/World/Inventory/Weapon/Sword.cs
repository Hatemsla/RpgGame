using System;
using UnityEngine;

namespace World.Inventory.Weapon
{
    public sealed class Sword : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
        }
    }
}