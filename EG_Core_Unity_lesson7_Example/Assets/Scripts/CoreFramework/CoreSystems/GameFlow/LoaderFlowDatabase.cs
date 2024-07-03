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
using UnityEngine;


namespace EG
{
    namespace Core.FlowStateMachine
    {
        //
        // This is how it looks like the json file for this configuration
        //
        // "LevelFlow": {
        //     "Id": "MainGameFlow",
        //     "Flow": [
        //     {
        //         "Id": "StartFramework",
        //         "Loopable": false,
        //         "Actions": [
        //         {
        //             "TriggerConditions": [],
        //             "ResultToExecute": [
        //             {
        //                 "ClassName": "LoadCoreSceneInstruction",
        //                 "ParametersToParse": [
        //                 "Number, 1",
        //                 "SceneName, CoreInitScene",
        //                 "Bootstrap, JsonActions/ActionsBoot",
        //                 "NextFlowAction, FlowBootScene",
        //                 "FlowToExecute, None"
        //                     ]
        //             }
        //             ]
        //         }
        //         ]
        //     },
        
        #region data from json
        
        [System.Serializable]
        public class PARSERootGameFlowData
        {
            public FlowJsonData LevelFlow = new FlowJsonData();
        }

        [System.Serializable]
        public class FlowJsonData
        {
            public string Id;
            public List<GameFlowActionsData> Flow = new List<GameFlowActionsData>();
        }

        [System.Serializable]
        public class GameFlowActionsData
        {
            public string Id = System.String.Empty;
            public bool Loopable = false;
            public List<GameFlowJsonDataForActions> Actions = new List<GameFlowJsonDataForActions>();
        }

        [System.Serializable]
        public class GameFlowJsonDataForActions
        {
            public List<TriggerConditionsData> TriggerConditions = new List<TriggerConditionsData>();
            public List<ResultOptionsData> ResultToExecute = new List<ResultOptionsData>();
        }
        
        #endregion
        
        
        #region events for flow

        [System.Serializable]
        public class TriggerConditionsData
        {
            public string ClassName = System.String.Empty;
            public List<string> ParametersToParse = new List<string>();
        }

        [System.Serializable]
        public class ResultOptionsData
        {
            public string ClassName = System.String.Empty;
            public List<string> ParametersToParse = new List<string>();
        }

        #endregion

        
        /// <summary>
        /// class to load the json file and return the dictionary with the results
        /// in this case using the Unity json library
        /// this class is separated, so in case of changing unity by newtonjson or other library
        /// will be restricted to this data class
        /// </summary>
        
        [System.Serializable]
        public class LoaderFlowDatabase
        {
            [SerializeField] private string baseFlowName = "MainGameFlow";
            [SerializeField] private string initialStateName = "StartFramework";
            
            [Header("Flow data")] 
            [SerializeField] private TextAsset[] gameFlowsData;
            
            public string GetBaseFlowName => baseFlowName;
            public string GetInitialStateName => initialStateName;

            public void Destroy()
            {
                gameFlowsData = null;
            }

            public Dictionary<string, List<GameFlowActionsData>> ParseFlowsData()
            {
                Dictionary<string, List<GameFlowActionsData>> tmpDictionary = new Dictionary<string, List<GameFlowActionsData>>();

                for (var i = 0; i < gameFlowsData.Length; ++i)
                {
                    var flowData = gameFlowsData[i];
                    PARSERootGameFlowData allData = null;
                    try
                    {
                        allData = JsonUtility.FromJson<PARSERootGameFlowData>(flowData.text);
                    }
                    catch (Exception)
                    {
                        Debug.LogError("error parsing gameFlowData json " + flowData.name);
                        return null;
                    }
                    
                    for (var loop1 = 0; loop1 < allData.LevelFlow.Flow.Count; ++loop1)
                    {
                        if (!tmpDictionary.ContainsKey(allData.LevelFlow.Id))
                        {
                            tmpDictionary.Add(allData.LevelFlow.Id, new List<GameFlowActionsData>());
                        }

                        tmpDictionary[allData.LevelFlow.Id].Add(allData.LevelFlow.Flow[loop1]);
                    }
                }

                return tmpDictionary;
            }

        }
    }
}
