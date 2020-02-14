using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        bool wasPlayed = false;

        private void OnTriggerEnter(Collider other) 
        {
            if (other.tag == "Player" && wasPlayed == false)
            {
                GetComponent<PlayableDirector>().Play();
                wasPlayed = true;
            }
        }
    }
}
