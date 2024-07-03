using EG.Core.BootLoader;


namespace EG
{
    namespace Core.Scenes
    {
        public class GameplayScene : EG_Scene
        {
            
            private string sceneToLoad = System.String.Empty;
            private string actionToLoad = System.String.Empty;
            private float percentageLoaded = 0f;
            private float percentageToIncrease = 0f;
            private System.Action onProgressCompleted = null;
            private LoadSceneBootActions sceneLoadingBootActions = null;
            
            // let's understand the use of the scenes. As I told earlier
            // scenes are a way to have data and accesors
            // in my case I use the gameplayscene to store my gameplay controller
            // that way I can call it from anywhere in my game without having to declare
            // any singleton at all
            
            //
            //public GameplayController GameplayController { get; set; } //to be set in runtime by an action for example

            
            #region scene overrides

            public override void Configure(EG_SceneData aData)
            {
                sceneToLoad = aData.GetUnitySceneNames[0];
                actionToLoad = aData.GetBootStrapName;
            }

            public override void OnEnter(bool aIsAdditive = false)
            {
                base.OnEnter(aIsAdditive);

                LoadScene(sceneToLoad, null, aIsAdditive);

                percentageToIncrease = 0f;
                percentageLoaded = 0f;

                //EG_Core.Self().UpdateProgress(0f);
            }


            private void OnUpdateProgressCallback(float progress)
            {
                //just in case we want to update some loading screen text with the percentage loaded....
                //EG_Core.Self().UpdateProgress(progress);
            }

            protected override void OnSceneLoaded()
            {
                sceneLoadingBootActions = new LoadSceneBootActions();

                percentageToIncrease = 20f;
                UpdateProgress();

                LoadActions();

                base.OnSceneLoaded();
            }

            
            public override void OnExit()
            {
                onProgressCompleted = null;

                sceneLoadingBootActions?.Destroy();

                EG_Core.Self().SetCoreUnPaused();

                EG_Core.Self().StartCoreTimer(0.3f, true, this,
                    cacheAction => { (cacheAction.Context as GameplayScene).FinishedExit(); });
            }
            
            #endregion
            

            private void LoadActions()
            {
                sceneLoadingBootActions.DoInit(actionToLoad, OnActionsDone);
            }

            private void OnActionsDone()
            {
                sceneLoadingBootActions.Destroy();

                onProgressCompleted = StartGame; //set the callback for when the progress reaches 100%
                percentageToIncrease = 100f;
                UpdateProgress();

            }

            private void FinishedExit()
            {
                base.OnExit();
            }



            #region scene intro

            private void StartGame()
            {
                // for example I like to have a monobehaviour with the audio clip I'm gonna use 
                // during the game within the Unity scene, so I use the gameplayscene to init this object.
                // Of course this could be done in an action outside this scope
                // but I want to show you how you can use this scene to do more work before the game starts
                //
                // MenuMusic menu = GameObject.FindObjectOfType<MenuMusic>();
                // if (menu != null)
                // {
                //     EG_Core.Self().StopMusic(menu.GetMenuMusic, 1.0f, true);
                //     GameObject.Destroy(menu.gameObject);
                // }

                EG_Core.Self().StartCoreTimer(0.15f, this,
                    cacheAction => { (cacheAction.Context as GameplayScene).StartFlow(); });
            }

            private void StartFlow()
            {
                // now we have the unity scene loaded, the actions loaded
                // and we just move to the state machine in order to start the flow of the game itself
                // "GameflowLevel1.json"
                //
                // and the core once again will be "disabled" cause the state machine is now in control
                // of our code, and through the state machine we decide how the code works
                
                //GameplayController.GetGameplayFlowController.StartFlow();
            }

            #endregion

            
            #region progress bar update loading

            private void UpdateProgress()
            {
                if (percentageLoaded < 100f)
                {
                    if (percentageLoaded < percentageToIncrease)
                    {
                        EG_Core.Self().StartCoreTimer(0.01f, true, this,
                            cacheAction => { (cacheAction.Context as GameplayScene).UpdateProgress(); });

                        percentageLoaded += 1f;
                        var result = Helpers.Map01(percentageLoaded, 0f, 100f);
                        //EG_Core.Self().UpdateProgress(result);
                    }
                }
                else if (percentageLoaded >= 100f)
                {
                    onProgressCompleted?.Invoke();
                }
            }

            #endregion
            
        }
        
    }
}