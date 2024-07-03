//example of empty action


namespace EG
{
    namespace Core.BootLoader
    {
        [System.Serializable]
        public class ActionEmpty : EG_BootAction
        {

            public ActionEmpty(bool aValue, int anArrayAmount)  : base (aValue, anArrayAmount){ }

            protected override void DoStart()
            {
                //just to give unity a little bit more of time 
                //to unload unused assets
                EG_Core.Self().StartTimer(0.1f, this, cacheAction => { (cacheAction.Context as ActionEmpty).Wait(); });
            }


            private void Wait()
            {
                DoOnComplete();
            }
        }
    }
}