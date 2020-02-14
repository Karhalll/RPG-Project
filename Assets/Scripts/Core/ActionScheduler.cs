using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour 
    {
        IAction curretAction;

        public void StartAction(IAction action)
        {
            if (curretAction == action) return;
            if (curretAction != null)
            {
                curretAction.Cancel();
            }
        
            curretAction = action;
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}