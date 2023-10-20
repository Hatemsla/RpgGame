using System;
using UnityEngine;

namespace World.Ability
{
    public class SpellView : MonoBehaviour
    {
        public float spellTime;
        public float spellSpeed;
        public Vector3 spellDirection;

        private void Update()
        {
            spellTime -= Time.deltaTime;
            if (spellTime > 0)
            {
                transform.Translate(spellDirection  * spellSpeed * Time.deltaTime);
                /*RaycastHit[] hits = Physics.RaycastAll(new Ray(spellDirection, (transform.position - spellDirection).normalized), 
                    (transform.position - spellDirection).magnitude);*/
            }
        }

        private void CreateSpell()
        {
            
        }
    }
}