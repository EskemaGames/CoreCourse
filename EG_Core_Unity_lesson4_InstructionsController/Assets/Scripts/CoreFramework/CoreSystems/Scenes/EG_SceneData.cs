namespace EG
{
    
    namespace Core.Scenes
    {
        public class EG_SceneData
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
            private readonly string bootstrapFile = null;
            private readonly string[] unitySceneNames = null;
            private readonly string nextFlowAction = null;

            public string[] GetUnitySceneNames => unitySceneNames;
            public string GetBootStrapName => bootstrapFile;
            public string GetNextFlow => nextFlowAction;


            public EG_SceneData(
                string aBootStrapFile,
                string aNextFlow,
                string[] aUnitySceneNames = null)
            {
                bootstrapFile = aBootStrapFile;
                nextFlowAction = aNextFlow;
                unitySceneNames = aUnitySceneNames;
            }

        }
    }

}