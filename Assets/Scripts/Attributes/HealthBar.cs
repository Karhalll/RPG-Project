using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health health = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas canvas = null;

        private void Update() 
        {
            foreground.localScale = new Vector3(health.GetFraction(), 1, 1);

            if (health.GetFraction() > 0 && health.GetFraction() < 1)
            {
                canvas.enabled = true;
            }
            else
            {
                canvas.enabled = false;
            }         
        }
    }
}