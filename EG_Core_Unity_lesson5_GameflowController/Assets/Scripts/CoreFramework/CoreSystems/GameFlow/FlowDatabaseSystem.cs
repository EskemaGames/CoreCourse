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
using System.Collections.Generic;



namespace EG
{
    namespace Core.FlowStateMachine
    {
        
        [System.Serializable]
        public class FlowDatabaseSystem : IDestroyable
        {
            [SerializeField]private LoaderFlowDatabase loaderFlowDatabase;
            
            private Dictionary<string, List<GameFlowActionsData>> allGameFlowPool = new Dictionary<string, List<GameFlowActionsData>>();

            public string GetBaseFlowName => loaderFlowDatabase.GetBaseFlowName;
            public string GetInitialStateName => loaderFlowDatabase.GetInitialStateName;
            
         

            #region init and destroy

            public void Init()
            {
                allGameFlowPool = loaderFlowDatabase.ParseFlowsData();
            }

            public void IDestroy()
            {
                IDestroyUnity();
            }

            public void IDestroyUnity()
            {
                loaderFlowDatabase?.Destroy();
                allGameFlowPool?.Clear();
            }

            #endregion
            
            
            #region public getters

            public List<GameFlowActionsData> GetFlowData(string id)
            {
                //try casting to get the value
                allGameFlowPool.TryGetValue(id, out var tmp);
  
                //
                // return a new copy, we don't want to modify
                // the actual content of the dictionary
                // also have in mind that the list could be null...
                //
                // now, this is a perfect place to talk about defensive programming
                // I'm NOT defending anything here, cause if I pass an incorrect string
                // the game MUST crash, cause this is a design error (the id passed doesn't exist or contains typos)
                // this way we can easily identify the bug and fix it
                // otherwise we might be hiding this bug until gods know when...
                //
                var final = new List<GameFlowActionsData>(tmp);
                return final;
            }
            
            #endregion
            
            
        }//end class
        
    }//end namespace
}