using System;
using EG.Core.BootLoader;
using UnityEngine;


namespace EG
{
    namespace Core.Scenes
    {

        public class MenuScene : EG_Scene
        {
            private string sceneToLoad = System.String.Empty;
            private string actionToLoad = System.String.Empty;
            private string nextFlow = String.Empty;
            private LoadSceneBootActions sceneLoadingBootActions = null;

            public override void Configure(EG_SceneData aData)
            {
                sceneToLoad = aData.GetUnitySceneNames[0];
                actionToLoad = aData.GetBootStrapName;
                nextFlow = aData.GetNextFlow;
            }



            public override void OnEnter(bool aIsAdditive = false)
            {
                LoadScene(sceneToLoad, OnUpdateProgressCallback);
            }


            private void OnUpdateProgressCallback(float progress)
            {
                //just in case we want to update some loading screen in our UI with the percentage loaded....
            }

            protected override void OnSceneLoaded()
            {
                sceneLoadingBootActions = new LoadSceneBootActions();
                sceneLoadingBootActions.DoInit(actionToLoad, OnActionsDone);
            }


            public override void OnExit()
            {
                base.OnExit();
            }

            private void OnActionsDone()
            {
                sceneLoadingBootActions.Destroy();
                sceneLoadingBootActions = null;

                Debug.Log("OnActionsDone MenuScene");
                
                // get rid of the black screen used for loading
                // again, this will be implemented later
                // as you can see in this commented code, everything belongs to the core
                //
                //EG_Core.Self().FadeOutLoadingScreen(FadeoutComplete);
                
                FadeoutComplete();
            }


            private void FadeoutComplete()
            {
                Debug.Log("fade out completed, we are here in the menu scene within our core framework");
                Debug.Log("\n");
                //open the menu screen for example
                // or other thing you might want to do up to this point
                //
                // I'm showcasing here the code that will be implemented later
                // to demonstrate how decoupled the code is
                //
                // again at this point we have loaded our menu scene (data)
                // and a unity scene with the main menu and whatever things you might have there
                // at this point the core is awaiting for your decision
                //
                // in my case I just call the core to open the menu screen 
                // and leave the execution to the game in the hands of the menu screen
                // as soon as I exit from this function the core is just waiting for us
                // to ask for a service or an action, it will be doing NOTHING until then
                
                // MenuScreen ms = EG_Core.Self().GetScreen<MenuScreen>();
                // EG_Core.Self().PushScreen(ms);
                
                
                // from the menu screen, when the user press the play button
                // I will call the core to load the game
                // like this commented code...
                // I will create some constant string to refer to the flow name action that I want to load
                // and the state machine will load automatically as instructed within the corresponding instruction
                // in "GameFlowCore.json"
                //
                EG_Core.Self().ChangeToGameCoreFlow(nextFlow);
            }

        }
        
    }
}