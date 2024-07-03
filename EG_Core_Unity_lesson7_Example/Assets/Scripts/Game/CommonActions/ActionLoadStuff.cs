//example of empty action


using UnityEngine;

namespace EG
{
    namespace Core.BootLoader
    {
        [System.Serializable]
        public class ActionLoadStuff : EG_BootAction
        {

            public ActionLoadStuff(bool aValue, int anArrayAmount)  : base (aValue, anArrayAmount){ }

            protected override void DoStart()
            {
                Debug.Log("do start action load stuff");
                //use a timer to have a "pause" instead of a corutine or an Invoke method
                EG_Core.Self().StartTimer(1f, this, cacheAction =>
                {
                    (cacheAction.Context as ActionLoadStuff).Wait();
                });
            }


            private void Wait()
            {
                //after the wait we flag the action as completed and the bootactions controller will move on with the next one
                DoOnComplete();
            }
        }
    }
}