using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        Experience experience;
        Text text;

        private void Awake() 
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
            text = GetComponent<Text>();
        }

        private void Update() 
        {
            float experienceValue = experience.GetExperience();
            text.text = Mathf.RoundToInt(experienceValue) + "XP";
        }
    }
}
