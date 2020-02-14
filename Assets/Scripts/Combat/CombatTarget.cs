﻿using UnityEngine;

using RPG.Control;
using RPG.Attributes;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]

    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HendleRaycast(PlayerController callingController)
        {
            if (!callingController.GetComponent<Fighter>().CanAttack(gameObject)) 
            { 
                return false; 
            }

            if (Input.GetMouseButton(0))
            {
                callingController.GetComponent<Fighter>().Attack(gameObject);
            }
            return true;
        }
    }
}
