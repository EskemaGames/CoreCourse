//
//Attribution-ShareAlike 
//CC BY-SA
//
//This license lets others remix, tweak, and build upon your work even for commercial purposes, 
//	as long as they credit you and license their new creations under the identical terms. 
//	This license is often compared to “copyleft” free and open source software licenses. 
//	All new works based on yours will carry the same license, so any derivatives will also allow commercial use. 
//	This is the license used by Wikipedia, and is recommended for materials that would benefit from incorporating content from Wikipedia and similarly licensed projects.




namespace EG
{
    namespace Core.BootLoader
    {

        [System.Serializable]
        public class EG_BootAction
        {
            // Dependencies
            public EG_BootAction[] Dependencies { get; protected set; }

            // Do other tasks needs to wait for this one to complete?
            public bool WaitForCompletion { get; protected set; }

            // This task is completed
            public bool Completed { get; private set; }



            #region constructor and destroy
            
            public EG_BootAction(bool aValue, int anArrayAmount)
            {
                Completed = false;
                WaitForCompletion = aValue;
                Dependencies = new EG_BootAction[anArrayAmount];
            }
            
            public void Destroy()
            {
                if (Dependencies == null) return;
                if (Dependencies.Length <= 0) return;
                
                System.Array.Clear(Dependencies, 0, Dependencies.Length);
                Dependencies = null;
            }
            
            #endregion

            
            #region public API
            
            // implement in each action to actually do something
            protected virtual void DoStart() { }

            
            public void DoOnComplete()
            {
                Completed = true;
            }
            
            public void WaitDependencies()
            {
                OnWaitToFinish();
            }
            
            #endregion


            // Wait for dependencies to finish
            private void OnWaitToFinish()
            {
                var foundActive = false;

                if (Dependencies.Length <= 0)
                {
                    DoStart();
                    return;
                }


                //check all our actions looking for the completion of all actions
                for (int i = 0, max = Dependencies.Length; i < max; ++i)
                {
                    EG_BootAction tmpAction = Dependencies[i];

                    if (tmpAction.Completed) continue;

                    foundActive = true;
                    break;
                }

                if (!foundActive)
                {
                    DoStart();
                    return;
                }
                
                //at this point all actions are in progress, wait a bit and call it again
                EG_Core.Self().StartCoreTimer(CoreConstants.CORE_STEP, true, this,
                    cacheAction => { (cacheAction.Context as EG_BootAction).OnWaitToFinish(); });
            }



        }
    }
}