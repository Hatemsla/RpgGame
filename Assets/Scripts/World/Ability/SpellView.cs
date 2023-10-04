using System;
using UnityEngine;

namespace World.Ability
{
    public class SpellView : MonoBehaviour
    {
        public float spellTime;
        public float spellSpeed;

        private void Update()
        {
            spellTime -= Time.deltaTime;
            if (spellTime > 0)
            {
                transform.Translate(transform.forward * spellSpeed);
            }
        }
    }
}