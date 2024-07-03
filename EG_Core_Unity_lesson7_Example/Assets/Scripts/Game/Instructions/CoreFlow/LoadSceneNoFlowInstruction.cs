using System;
using System.Collections.Generic;
using EG.Core.Scenes;


namespace EG
{
    namespace Core.FlowStateMachine
    {
        /// <summary>
        /// This class expects to parse some parameters in the json file
        /// SceneName with the name of the scene to load...
        /// Bootstrap with the name of the json file
        /// NextFlowAction with the name of the next state to go for
        /// </summary>
        [System.Serializable]
        public class LoadSceneNoFlowInstruction : Instruction
        {

            private EG_Scene sceneToLoad = null;

            
            #region constructor

            public LoadSceneNoFlowInstruction() { }

            public LoadSceneNoFlowInstruction(List<string> parameters) : base(parameters)
            {
                var type = typeof(EG_Scene);
                var namespaceName = type.Namespace + ".";
                
                //we KNOW there's only 1, so... arrays are initiated at 1
                var unitySceneNames = new string[1];
        
                unitySceneNames[0] = base.parameters[Constants.ParametersToParse.UnityScene].ToString();
                var bootstrap = base.parameters[Constants.ParametersToParse.Bootstrap].ToString();
     
                var mySceneName = base.parameters[Constants.ParametersToParse.SceneName].ToString();
                var myType = Type.GetType(namespaceName + mySceneName);
                sceneToLoad = (EG_Scene) Activator.CreateInstance(myType);
                
                var data = new EG_SceneData(bootstrap, "", unitySceneNames);
                    
                sceneToLoad.Configure(data);
            }

            #endregion



            #region instruction base code

            public override bool CheckInstruction()
            {
                return true;
            }

            public override bool DoExecuteInstruction()
            {
                Start();

                return true;
            }

            #endregion

            

            private void Start()
            {
                EG_Core.Self().LoadNewScene(sceneToLoad, DoExecuteOnCompletion);
            }
            

        }
    }
}