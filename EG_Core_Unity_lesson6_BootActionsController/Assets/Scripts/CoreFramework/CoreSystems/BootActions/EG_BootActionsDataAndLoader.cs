//
//Attribution-ShareAlike 
//CC BY-SA
//
//This license lets others remix, tweak, and build upon your work even for commercial purposes, 
//	as long as they credit you and license their new creations under the identical terms. 
//	This license is often compared to “copyleft” free and open source software licenses. 
//	All new works based on yours will carry the same license, so any derivatives will also allow commercial use. 
//	This is the license used by Wikipedia, and is recommended for materials that would benefit from incorporating content from Wikipedia and similarly licensed projects.


using System.Collections.Generic;
using UnityEngine;

namespace EG
{
    namespace Core.BootLoader
    {

        /// <summary>
        /// Our class to handle the data coming from the jsons
        /// </summary>
        [System.Serializable]
        public class EG_BootActionsDataAndLoader
        {
            
            #region json data for actions

            [System.Serializable]
            public class Dependencies
            {
                public string Name = System.String.Empty;
            }

            [System.Serializable]
            public class BootActionDefinitionJson
            {
                public bool WaitForCompletion = false;
                public string Name = System.String.Empty;
                public List<Dependencies> Dependencies = new List<Dependencies>();
            }
        
            [System.Serializable]
            public class BootActionsJson
            {
                public List<BootActionDefinitionJson> ActionsList = new List<BootActionDefinitionJson>();
            }

            #endregion
            
            
            #region load all actions
            
            public List<EG_BootAction> LoadActions(string aFileName)
            {
             
                List<EG_BootAction> actions = new List<EG_BootAction>(20); 
                var actionsAsset = UnityEngine.Resources.Load(aFileName) as TextAsset;

                var actionsJson = JsonUtility.FromJson<BootActionsJson>(actionsAsset.text);

                var myType = typeof(EG_BootAction);
                var namespaceWithAppendFull = myType.Namespace + ".";

                //loop the json and create the actions
                for (var i = 0; i < actionsJson.ActionsList.Count; ++i)
                {
                    var argTypeInstruction = new object[] {actionsJson.ActionsList[i].WaitForCompletion, actionsJson.ActionsList[i].Dependencies.Count};
                    var action = Helpers.CreateInstance<EG_BootAction>(namespaceWithAppendFull, actionsJson.ActionsList[i].Name, argTypeInstruction);

                    //add it to the list
                    actions.Add(action);

                } //end taskjson list
                
                //now loop the tasks and assign the dependencies if any available
                for (int i = 0, max = actions.Count; i < max; ++i)
                {
                    if (actions[i].Dependencies.Length <= 0) continue;

                    //loop the json list of dependencies, cause we have the root of actions
                    //now we need to check the dependencies looping through the list
                    //checking the matches and adding them to the array
                    for (var cnt = 0; cnt < actionsJson.ActionsList[i].Dependencies.Count; ++cnt)
                    {
                        //basically we loop the list again because we don't know that task is the dependency
                        //it could be a position we are not right now, for example task 0 doesn't have dependency
                        //BUT task 1 usually have task 0 as dependency, and so on
                        for (var cnt1 = 0; cnt1 < actions.Count; ++cnt1)
                        {
                            if (!actions[cnt1].GetType().Name.Equals(actionsJson.ActionsList[i].Dependencies[cnt].Name)) continue;
                            
                            actions[i].Dependencies[cnt] = actions[cnt1];
                            break;
                        }
                    }
                } //end for loop
                
                return actions;
            }
            
            #endregion


        }
    }
}