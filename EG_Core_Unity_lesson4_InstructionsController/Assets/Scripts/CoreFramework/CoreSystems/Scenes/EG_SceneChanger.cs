using System.Collections.Generic;


namespace EG
{
    namespace Core.Scenes
    {
        public class EG_SceneChanger : IDestroyable
        {
            public EG_Scene CurrentScene => currentScene;

            private EG_Scene currentScene = null;
            private EG_Scene tmpLoadingScene = null;
            private bool isChangingScene = false;
            private uint timerChangeScene = 99999999;

            private System.Action onSceneLoaded = null;
            private Queue<EG_Scene> scenesStack = new Queue<EG_Scene>(10);
      

            #region public API
            
            public void IDestroy()
            {
                IDestroyUnity();
            }

            public void IDestroyUnity()
            {
                onSceneLoaded = null;
                
                if (EG_Core.Self() != null)
                {
                    EG_Core.Self().StopCoreTimerWithID(timerChangeScene);
                }

                scenesStack.Clear();
            }

            //change to a new scene unless we are loading one...
            public void LoadNewScene(EG_Scene aScene, System.Action aOnSceneLoaded)
            {
                if (isChangingScene) return;

                onSceneLoaded = aOnSceneLoaded;
                
                isChangingScene = true;

                ChangeToScene(aScene);
            }

            //change to a new scene unless we are loading one, but additively
            public void LoadNewSceneAdditive(EG_Scene aScene, System.Action aOnSceneLoaded)
            {
                if (isChangingScene) return;
                
                onSceneLoaded = aOnSceneLoaded;
                
                isChangingScene = true;
                
                scenesStack.Enqueue(aScene);

                currentScene = aScene;
                currentScene.OnEnter(true);

                WaitToLoadNewSceneComplete();
            }

            #endregion


 
            private void ChangeToScene(EG_Scene aScene)
            {
                //previous shit??, clean it up!!
                if (scenesStack.Count > 0)
                {
                    //store the scene for later use
                    tmpLoadingScene = aScene;

                    //exit from here, to NOT return ever because of this condition
                    CleanPreviousScenes();

                    return;
                }

                //if no other previous scene was in our system we will be directed here
                //otherwise the clean function will take care of continuing
                scenesStack.Enqueue(aScene);

                currentScene = aScene;
                currentScene.OnEnter();
                WaitToLoadNewSceneComplete();
            }

            private void CleanPreviousScenes()
            {
                //if we have previous scenes, we keep calling this function recursively
                //until no more scenes are present in our stack
                if (scenesStack.Count > 0)
                {
                    currentScene = scenesStack.Dequeue();
                    currentScene.OnExit();

                    //this function will trigger a timer that will come back here
                    WaitToExitSceneCompleteInList();

                    return;
                }
                
                //
                //now ALL previous scenes has been unloaded, so time to load the new one
                //
                
                //at this point we have exited from the scene
                //clean up the garbage before we enter in the new one
                currentScene.Destroy();

                //add the tmp stored scene
                scenesStack.Enqueue(tmpLoadingScene);
                
                //set our new fresh scene
                currentScene = tmpLoadingScene;
                currentScene.OnEnter();

                WaitToLoadNewSceneComplete();
            }


            #region timer callbacks

            private void WaitToExitSceneCompleteInList()
            {
                // wait until the scene is marked as completed
                // so keep calling this function recursively
                // without using coroutines or any async thing
                if (!currentScene.ExitCompleted)
                {
                    EG_Core.Self().StartCoreTimer(CoreConstants.CORE_STEP, true, this,
                        cacheAction => { (cacheAction.Context as EG_SceneChanger).WaitToExitSceneCompleteInList(); });

                    return;
                }

                CleanPreviousScenes();
            }

            private void WaitToLoadNewSceneComplete()
            {
                //just keep pooling until the scene and the bootstrap was fully loaded
                //we check every few frames this timer...
                //this is just to be able to load a new scene.
                //The code does NOTHING else...

                if (!currentScene.EnterCompleted)
                {
                    EG_Core.Self().StartCoreTimer(CoreConstants.CORE_STEP, true, this,
                        cacheAction => { (cacheAction.Context as EG_SceneChanger).WaitToLoadNewSceneComplete(); });

                    return;
                }

                isChangingScene = false;
                onSceneLoaded?.Invoke();
            }

            #endregion



        }

    }
}
