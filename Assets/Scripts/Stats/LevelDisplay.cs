using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        BaseStats baseStats;
        Text text;

        private void Awake() 
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            text = GetComponent<Text>();
        }

        private void Update() 
        {
            float currentLevel = baseStats.GetLevel();
            text.text = currentLevel + "lvl";
        }
    }
}
