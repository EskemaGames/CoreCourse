using UnityEngine;


namespace EG
{
    namespace Core.Scenes
    {

        public class MenuScene : EG_Scene
        {
            private string sceneToLoad = System.String.Empty;
            private string actionToLoad = System.String.Empty;

            public override void Configure(EG_SceneData aData)
            {
                sceneToLoad = aData.GetUnitySceneNames[0];
                actionToLoad = aData.GetBootStrapName;
            }
            
            public override void OnEnter(bool isadditive = false)
            {
                LoadScene(sceneToLoad, OnUpdateProgressCallback);
            }
            
            private void OnUpdateProgressCallback(float progress)
            {
                //just in case we want to update some loading screen text with the percentage loaded....
                //EG_Core.Self().UpdateProgress(progress);
            }

            protected override void OnSceneLoaded()
            {
                SetEnterCompleted();

                //do something like for example open the menu screen, as this is the menu scene...

                //var menuScreen = EG_Core.Self().GetScreen<MenuScreen>();
                //menuScreen.Open();
            }

            public override void OnExit()
            {
                base.OnExit();
            }
            

        }
        
    }
}