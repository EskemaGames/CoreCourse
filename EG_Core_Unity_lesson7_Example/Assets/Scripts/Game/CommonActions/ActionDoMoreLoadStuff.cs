//example of empty action


namespace EG
{
    namespace Core.BootLoader
    {
        [System.Serializable]
        public class ActionDoMoreLoadStuff : EG_BootAction
        {

            public ActionDoMoreLoadStuff(bool aValue, int anArrayAmount)  : base (aValue, anArrayAmount){ }

            protected override void DoStart()
            {
                UnityEngine.Debug.Log("do start action do more load stuff");
                //use a timer to have a "pause" instead of a corutine or an Invoke method
                EG_Core.Self().StartTimer(3f, this, cacheAction =>
                {
                    (cacheAction.Context as ActionDoMoreLoadStuff).Wait();
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