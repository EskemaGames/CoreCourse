//
//Attribution-ShareAlike 
//CC BY-SA
//
//This license lets others remix, tweak, and build upon your work even for commercial purposes, 
//	as long as they credit you and license their new creations under the identical terms. 
//	This license is often compared to “copyleft” free and open source software licenses. 
//	All new works based on yours will carry the same license, so any derivatives will also allow commercial use. 
//	This is the license used by Wikipedia, and is recommended for materials that would benefit from incorporating content from Wikipedia and similarly licensed projects.


using UnityEngine;
using UnityEngine.SceneManagement;


namespace EG
{
    namespace Core.Scenes
    {
        /// <summary>
        /// idea taken from android context system (pretty much)
        /// loads a scene async or additive async
        /// Provides information about his current state
        /// and provides "callback" methods for OnEnter and 
        /// OnExit
        /// </summary>
        public abstract class EG_Scene
        {
            public bool ExitCompleted { get; private set; }
            public bool EnterCompleted { get; private set; }
            
            private string sceneName = System.String.Empty;
            private bool additive = false;
            private AsyncOperation loadOp = null;

            protected bool GetIsAdditive => additive;
            private System.Action<float> onLoadingProgressUpdate;


            public void Destroy()
            {
                loadOp = null;
            }


            public void SetEnterCompleted()
            {
                EnterCompleted = true;
            }
            
            public virtual void Configure(EG_SceneData aData) { }


            /// Called before Entering on a new Scene
            public virtual void OnEnter(bool anAdditive = false)
            {
                additive = anAdditive;
            }

            /// Called before Exit from a Scene
            public virtual void OnExit()
            {
                ExitCompleted = true;
            }


            #region load new scene asyn and/or additive

            protected void LoadScene(string aSceneName, System.Action<float> onLoadingProgressUpdate,
                bool anAdditive = false)
            {
                sceneName = aSceneName;
                additive = anAdditive;
                this.onLoadingProgressUpdate = onLoadingProgressUpdate;
                loadOp = additive
                    ? SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive)
                    : SceneManager.LoadSceneAsync(sceneName);
                WaitForLoadAsync();
            }

            private void WaitForLoadAsync()
            {
                float progress;

                if (!loadOp.isDone)
                {
                    progress = loadOp.progress;

                    onLoadingProgressUpdate?.Invoke(progress);

                    EG_Core.Self().StartCoreTimer(CoreConstants.CORE_STEP, true, this,
                        cacheAction => { (cacheAction.Context as EG_Scene).WaitForLoadAsync(); });

                    return;
                }
                
                progress = 1f;

                onLoadingProgressUpdate?.Invoke(progress);
                onLoadingProgressUpdate = null;
                
                OnSceneLoaded();
            }

            #endregion


            /// <summary>
            /// EMPTY parent function, implement as needed on each scene
            /// </summary>
            protected virtual void OnSceneLoaded()
            {
                
            }


        }

    }
}