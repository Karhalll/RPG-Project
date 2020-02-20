using UnityEngine;

using GameDevTV.Inventories;

using RPG.Stats;
using System.Collections.Generic;

namespace RPG.Inventories
{
    public class StatsEquipment : Equipment, IModifierProvider
    {
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            foreach (var slot in GetAllPopulatedSlots())
            {
                IModifierProvider item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null) continue;

                foreach (float modifier in item.GetAdditiveModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }

        public IEnumerable<float> GetPercentageMofifier(Stat stat)
        {
            foreach (var slot in GetAllPopulatedSlots())
            {
                IModifierProvider item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null) continue;

                foreach (float modifier in item.GetPercentageMofifier(stat))
                {
                    yield return modifier;
                }
            }
        }
    }
}