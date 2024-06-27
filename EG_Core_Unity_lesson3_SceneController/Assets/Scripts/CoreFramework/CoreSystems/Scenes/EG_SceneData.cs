using System;

namespace EG
{
    
    namespace Core.Scenes
    {
        /// <summary>
        /// the class that holds the information read in the json to be used
        /// parameters like this
        /// "Number, 1",
        /// "SceneName, PublisherScene",
        /// "UnityScene, PublisherSceneUnity",
        /// "NextFlowAction, FlowPublisherSceneDone",
        /// "Bootstrap, JsonActions/ActionsPublisher",
        /// </summary>
        public class EG_SceneData
        {
            private readonly string[] parametersExtra = null;
            private readonly string[] bootstrapFiles = null;
            private readonly string bootstrapFile = String.Empty;
            private readonly string[] unitySceneNames = null;
            private readonly string nextFlowAction = null;


            public string[] GetParameters => parametersExtra;
            public string[] GetUnitySceneNames => unitySceneNames;
            public string[] GetBootStrapNames => bootstrapFiles;
            public string GetBootStrapName => bootstrapFile;
            public string GetNextFlow => nextFlowAction;

            
            public EG_SceneData(
                string[] aBootStrapFile,
                string[] aUnitySceneNames = null,
                string[] aParameters = null)
            {
                bootstrapFiles = aBootStrapFile;
                unitySceneNames = aUnitySceneNames;
                parametersExtra = aParameters;
            }

            public EG_SceneData(
                string aBootStrapFile,
                string aNextFlow,
                string[] aUnitySceneNames = null,
                string[] aParameters = null)
            {
                nextFlowAction = aNextFlow;
                bootstrapFile = aBootStrapFile;
                unitySceneNames = aUnitySceneNames;
                parametersExtra = aParameters;
            }
            

        }
    }

}