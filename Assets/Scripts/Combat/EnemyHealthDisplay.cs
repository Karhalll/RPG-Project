using System;
using UnityEngine;
using UnityEngine.UI;

using RPG.Attributes;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter = null;
        Text text;

        private void Awake() 
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            text = GetComponent<Text>();
        }

        private void Update() 
        {
            if (fighter.GetTarget() == null)
            {
                text.text = "N/A";
            }
            else if (fighter.GetTarget() != null)
            {
                Health health = fighter.GetTarget();
                text.text = String.Format("{0:0}/{1:0}", health.GethealthPoints(), health.GetMaxHealthPoints());
            }    
        }
    }
}
