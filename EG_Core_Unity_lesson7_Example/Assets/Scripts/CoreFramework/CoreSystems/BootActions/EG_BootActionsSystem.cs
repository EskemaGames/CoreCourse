//
//Attribution-ShareAlike 
//CC BY-SA
//
//This license lets others remix, tweak, and build upon your work even for commercial purposes, 
//	as long as they credit you and license their new creations under the identical terms. 
//	This license is often compared to “copyleft” free and open source software licenses. 
//	All new works based on yours will carry the same license, so any derivatives will also allow commercial use. 
//	This is the license used by Wikipedia, and is recommended for materials that would benefit from incorporating content from Wikipedia and similarly licensed projects.


using System;
using System.Collections.Generic;


namespace EG
{
    namespace Core.BootLoader
    {
        
        public abstract class EG_BootActionsSystem
        {
            private EG_BootActionsDataAndLoader bootActionsDataAndLoader = null;
            private List<EG_BootAction> actions = null;
            private bool bootCompleted = false;
            private uint timerID = 0;
            private string fileName = String.Empty;
            private System.Action onAllActionsDone;

            
            #region init

            public virtual void DoInit(string aFileName, System.Action onActionsDone = null)
            {
                bootActionsDataAndLoader = new EG_BootActionsDataAndLoader();
                onAllActionsDone = onActionsDone;
                fileName = aFileName;
                actions = bootActionsDataAndLoader.LoadActions(fileName);
                InitActions();
            }
            
            #endregion
            
            
            #region destroy

            public void Destroy()
            {
                if (!bootCompleted)
                {
                    if (EG_Core.Self() != null)
                    {
                        EG_Core.Self().StopCoreTimerWithID(timerID);
                    }
                }

                if (actions != null)
                {
                    for (var i = actions.Count - 1; i > -1; --i)
                    {
                        actions[i].Destroy();
                    }
                    
                    actions.Clear();
                }

                onAllActionsDone = null;
            }

            #endregion

            
            private void InitActions()
            {
                for (int i = 0, max = actions.Count; i < max; ++i)
                {
                    var action = actions[i];

                    action?.WaitDependencies();
                }

                WaitForBootCompleted();
            }
            
            private void WaitForBootCompleted()
            {
                var actionsRunning = false;

                //check if any action is still running
                for (int i = 0, max = actions.Count; i < max; ++i)
                {
                    var action = actions[i];
                    if (action.WaitForCompletion && !action.Completed)
                    {
                        actionsRunning = true;
                        break;
                    }
                }

                //no tasks, we are done 
                if (!actionsRunning)
                {
                    bootCompleted = true;
                    onAllActionsDone?.Invoke();
                    onAllActionsDone = null;

                    return;
                }

                //check later as a recursion
                timerID = EG_Core.Self().StartCoreTimerId(CoreConstants.CORE_STEP, true, this,
                    cacheAction => { (cacheAction.Context as EG_BootActionsSystem).WaitForBootCompleted(); });
            }

            
        }
        
    }
}