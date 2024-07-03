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


namespace EG
{
    namespace Core.FlowStateMachine
    {

        /// <summary>
        /// Our state machine to control the flow of the game via Instructions
        /// </summary>
        public class GameFlowSystem : IRestart, IReset, IDestroyable
        {
            //
            // this will control the flow of the game
            // switching from tutorials to normal state or whatever you want
            // and doing game actions like spawning new people after a certain amount of time, etc
            // basically any flow state in your game will be handled by this state machine
            //
            private class GameFlowData
            {
                public bool Loopable = false;
                public List<Instruction> TriggerConditionInstructions = new List<Instruction>(10);
                public List<Instruction> ResultsToExecuteInstructions = new List<Instruction>(10);
            }


            private Dictionary<string, List<GameFlowData>> allGameFlowPool = new Dictionary<string, List<GameFlowData>>(30);
            private List<Instruction> instructionsToExecute = new List<Instruction>(30);
            private List<Instruction> instructionsExecuted = new List<Instruction>(30);
            private List<Instruction> currentLoopFlowActions = new List<Instruction>(30);
            private List<GameFlowData> currentFlowData = new List<GameFlowData>(10);
            private InstructionsSystem instructionsSystem = null;
            private int totalInstructionsToProcess = 0;


            #region init and destroy

            public void Init(List<GameFlowActionsData> aList, string aFlowToParse)
            {
                var myType = typeof(GameFlowSystem);
                var namespaceFlowNameWithAppendFull = myType.Namespace + ".";
                
                instructionsSystem?.IDestroy();
                instructionsSystem = new InstructionsSystem();
                instructionsSystem.Init();

                //clean any previous references or stuff that might be pending
                foreach (var flow in allGameFlowPool)
                {
                    for (var i = 0; i < flow.Value.Count; ++i)
                    {
                        var alltriggers = flow.Value[i].TriggerConditionInstructions;
                        for (var loop1 = 0; loop1 < alltriggers.Count; ++loop1)
                        {
                            var ins = alltriggers[loop1];
                            ins.Destroy();
                        }

                        var allresults = flow.Value[i].ResultsToExecuteInstructions;
                        for (var loop1 = 0; loop1 < allresults.Count; ++loop1)
                        {
                            var ins = allresults[loop1];
                            ins.Destroy();
                        }
                    }
                }

                allGameFlowPool.Clear();

                
                //after all the clean up, start reading data and storing it in our dictionary
                for (var loop = 0; loop < aList.Count; ++loop)
                {
                    var datavaluesFoundToAdd = new List<GameFlowData>(15);

                    for (var i = 0; i < aList[loop].Actions.Count; ++i)
                    {
                        GameFlowJsonDataForActions data = aList[loop].Actions[i];

                        //prepare our tmp lists to parse the information
                        var resultsToExecuteInstructions = new List<Instruction>(10);
                        var triggerConditionInstructions = new List<Instruction>(10);

                        //convert all the json data into real classes
                        for (var loop1 = 0; loop1 < data.TriggerConditions.Count; ++loop1)
                        {
                            var argTypeInstruction = new object[]
                                {data.TriggerConditions[loop1].ParametersToParse};

                            var instruction = Helpers.CreateInstance<Instruction>(
                                namespaceFlowNameWithAppendFull,
                                data.TriggerConditions[loop1].ClassName,
                                argTypeInstruction);

                            if (instruction == null) continue;

                            triggerConditionInstructions.Add(instruction);
                        }

                        //convert all the json data into real classes
                        for (var loop1 = 0; loop1 < data.ResultToExecute.Count; ++loop1)
                        {
                            var argTypeInstruction = new object[]
                                {data.ResultToExecute[loop1].ParametersToParse};

                            var instruction = Helpers.CreateInstance<Instruction>(
                                namespaceFlowNameWithAppendFull,
                                data.ResultToExecute[loop1].ClassName,
                                argTypeInstruction);

                            if (instruction == null) continue;

                            resultsToExecuteInstructions.Add(instruction);
                        }

                        var flowData = new GameFlowData
                        {
                            Loopable = aList[loop].Loopable,
                            ResultsToExecuteInstructions = resultsToExecuteInstructions,
                            TriggerConditionInstructions = triggerConditionInstructions
                        };

                        datavaluesFoundToAdd.Add(flowData);
                    }

                    allGameFlowPool.Add(aList[loop].Id, datavaluesFoundToAdd);
                }

                currentFlowData = GetFlowData(aFlowToParse);
            }

            public void IDestroy()
            {
                Destroy();

                currentLoopFlowActions.Clear();
                instructionsToExecute.Clear();
                
                instructionsSystem?.IDestroy();
                instructionsSystem = null;

                DestroyFlow();
            }

            public void IDestroyUnity()
            {
                Destroy();

                currentLoopFlowActions.Clear();
                instructionsToExecute.Clear();
                
                instructionsSystem?.IDestroyUnity();
                instructionsSystem = null;
                
                DestroyFlow();
            }
            
            private void Destroy()
            {
                if (instructionsExecuted != null)
                {
                    for (var i = instructionsExecuted.Count - 1; i > -1; --i)
                    {
                        instructionsExecuted[i].Destroy();
                    }
                }

                if (instructionsToExecute != null)
                {
                    for (var i = instructionsToExecute.Count - 1; i > -1; --i)
                    {
                        instructionsToExecute[i].Destroy();
                    }
                }

                if (currentFlowData != null)
                {
                    for (var i = currentFlowData.Count - 1; i > -1; --i)
                    {
                        var alltriggers = currentFlowData[i].TriggerConditionInstructions;
                        for (var loop1 = alltriggers.Count - 1; loop1 > -1; --loop1)
                        {
                            var ins = alltriggers[loop1];
                            ins.Destroy();
                        }

                        var allresults = currentFlowData[i].ResultsToExecuteInstructions;
                        for (var loop1 = allresults.Count - 1; loop1 > -1; --loop1)
                        {
                            var ins = allresults[loop1];
                            ins.Destroy();
                        }
                    }
                }
            }

            private void DestroyFlow()
            {
                foreach (var flow in allGameFlowPool)
                {
                    for (var i = flow.Value.Count-1; i > -1; --i)
                    {
                        var alltriggers = flow.Value[i].TriggerConditionInstructions;
                        for (var loop1 = alltriggers.Count-1; loop1 > -1; --loop1)
                        {
                            var ins = alltriggers[loop1];
                            ins.Destroy();
                        }

                        var allresults = flow.Value[i].ResultsToExecuteInstructions;
                        for (var loop1 = allresults.Count-1; loop1 > -1; --loop1)
                        {
                            var ins = allresults[loop1];
                            ins.Destroy();
                        }
                    }
                }

                allGameFlowPool.Clear();
            }

            #endregion

            
            #region public API

            public void StartFlow()
            {
                ProcessNonLoopableFlow();
            }

            public void IOnUpdate()
            {
                if (currentLoopFlowActions.Count <= 0) return;
                
                ExecuteActionsEnqueuedLoopable();
            }

            public void IOnRestart()
            {
                IOnReset();
            }

            public void IOnReset()
            {
                currentLoopFlowActions.Clear();

                //not restartable, init from start
                totalInstructionsToProcess = 0;

                instructionsExecuted.Clear();
                instructionsToExecute.Clear();

                instructionsSystem.IOnRestart();

                foreach (var dictData in allGameFlowPool)
                {
                    if (dictData.Value == null) continue;

                    List<GameFlowData> valueList = dictData.Value;

                    for (var i = 0; i < valueList.Count; ++i)
                    {
                        for (var loop1 = 0; loop1 < valueList[i].TriggerConditionInstructions.Count; ++loop1)
                        {
                            valueList[i].TriggerConditionInstructions[loop1].OnRestart();
                        }

                        for (var loop1 = 0; loop1 < valueList[i].ResultsToExecuteInstructions.Count; ++loop1)
                        {
                            valueList[i].ResultsToExecuteInstructions[loop1].OnRestart();
                        }
                    }
                }
                
            }

            public void ChangeFlow(string aNewFlow)
            {
                currentLoopFlowActions.Clear();

                totalInstructionsToProcess = 0;

                instructionsToExecute.Clear();

                currentFlowData.Clear();

                //get a copy to work locally
                currentFlowData = GetFlowData(aNewFlow);

                ProcessNonLoopableFlow();
            }

            #endregion


            #region main core process logic

            private void ProcessNonLoopableFlow()
            {
                ProcessFlowActions();
            }
            
            #endregion


            #region loopable flow
            
            private void EnqueueLoopableFlowAction(int anId)
            {
                if (!CheckTriggerConditions(currentFlowData[anId].TriggerConditionInstructions)) return;
                
                var allresults = currentFlowData[anId].ResultsToExecuteInstructions;

                for (var loop1 = 0; loop1 < allresults.Count; ++loop1)
                {
                    var ins = allresults[loop1];

                    if (ins.WasExecuted) continue;
                    if (currentLoopFlowActions.Contains(ins)) continue;

                    currentLoopFlowActions.Add(ins);

                    ins.DoInit();
                }

                //as this flow was executed we remove it
                currentFlowData.RemoveAt(anId);
            }
            
            private void ExecuteActionsEnqueuedLoopable()
            {
                for (var i = 0; i < currentLoopFlowActions.Count; ++i)
                {
                    var instruction = currentLoopFlowActions[i];
                    instruction.DoExecuteInstruction();
                }
            }

            #endregion


            #region normal flow

            private void ProcessFlowActions()
            {
                EnqueueFlowAction();
                ExecuteActionsEnqueued();
            }

            private void EnqueueFlowAction()
            {
                for (var i = 0; i < currentFlowData.Count; ++i)
                {
                    if (!CheckTriggerConditions(currentFlowData[i].TriggerConditionInstructions)) continue;

                    var allresults = currentFlowData[i].ResultsToExecuteInstructions;

                    for (var loop1 = 0; loop1 < allresults.Count; ++loop1)
                    {
                        var ins = allresults[loop1];

                        //if it was executed don't add it again      
                        if (ins.WasExecuted) continue;

                        instructionsToExecute.Add(ins);
                        ins.DoInit();
                    }
                }
            }

            private void ExecuteActionsEnqueued()
            {
                if (totalInstructionsToProcess >= 1) //as long as we have an instruction execute it!
                {
                    if (instructionsSystem.GetTotalInstructions > 0)
                    {
                        instructionsSystem.ExecuteInstruction();
                    }

                    return;
                }

                //no more instructions, check what else to do...
                for (var i = 0; i < instructionsToExecute.Count; ++i)
                {
                    totalInstructionsToProcess++;
                    var instruction = instructionsToExecute[i];

                    //we don't care about the result, the flow keeps going
                    instructionsSystem.AddInstruction(instruction, OnInstructionProcessedAndFinishedCallback);
                }

                //any instruction at all, we call this recursively until no more instructions left in the queue
                if (totalInstructionsToProcess > 0)
                {
                    ExecuteActionsEnqueued(); //loop until execute the instructions
                }
            }

            #endregion


            #region check conditions

            private bool CheckTriggerConditions(List<Instruction> aConditions)
            {
                //first run the init method once
                for (var i = 0; i < aConditions.Count; ++i)
                {
                    var instruction = aConditions[i];
                    
                    if (!instruction.WasExecuted)
                    {
                        instruction.DoInit();
                    }
                }
                
                // Iterate over all the conditions, ALL must be true
                for (var i = 0; i < aConditions.Count; ++i)
                {
                    var instruction = aConditions[i];

                    if (!instruction.CheckInstruction())
                    {
                        return false;
                    }
                }

                // By default (no conditions) returns true
                // or the conditions above returned true...
                return true;
            }

            #endregion



            private void OnInstructionProcessedAndFinishedCallback(Instruction anInstruction)
            {
                totalInstructionsToProcess--;

                instructionsExecuted.Add(anInstruction);

                CheckNextStep(anInstruction);
            }

            private void CheckNextStep(Instruction anInstruction)
            {
                if (totalInstructionsToProcess <= 0)
                {
                    // all instructions completed, do something...
                    // basically level ended (or whatever you are processing), so change the flow type to finish
                    // and execute more instructions (if so)
                    totalInstructionsToProcess = 0;

                    instructionsToExecute.Clear();
                    instructionsSystem.ClearAll();

                    ProcessInstructionFinished(true, anInstruction);

                    return;
                }

                //process more instructions!!! we are not done yet!
                ProcessInstructionFinished(false, anInstruction);
            }

            private void ProcessInstructionFinished(bool endofInstructions, Instruction anInstruction)
            {
                var parsedParams = anInstruction.GetParameters;

                var currentFlowStateActive = System.String.Empty;
                object nextState;

                //try casting if this instruction contains another flow to execute
                parsedParams.TryGetValue(Constants.ParametersToParse.FlowToExecute, out nextState);

                if (nextState != null)
                {
                    //yay!!! we have another flow to execute...
                    currentFlowStateActive = nextState.ToString();
                    totalInstructionsToProcess = 0;
                    instructionsToExecute.Clear();
                    currentFlowData.Clear();
                    instructionsSystem.ClearAll();
                }
                else
                {
                    if (!endofInstructions)
                    {
                        ExecuteActionsEnqueued();
                        return;
                    }
                }

                //the completed instruction comes with a flow
                //try to execute that flow
                if (!System.String.IsNullOrEmpty(currentFlowStateActive) && !currentFlowStateActive.Equals("None"))
                {
                    //check if we have another list of data or its a null object
                    var tmp = GetFlowData(nextState.ToString());

                    if (tmp != null)
                    {
                        currentFlowData = tmp;
                    }
                }

                //found a flow?, then keep going
                //otherwise our job here has finished and the flow stops here
                //the flow shouldn't be loopable, otherwise the flow is loopable
                //set the list and wait for the update method to work it's magic
                if (currentFlowData.Count > 0)
                {
                    if (!currentFlowData[0].Loopable)
                    {
                        ProcessNonLoopableFlow();
                        
                        //stop all loopable stuff
                        for (var i = 0; i < currentLoopFlowActions.Count; ++i)
                        {
                            currentLoopFlowActions[i].Destroy();
                        }
                        currentLoopFlowActions.Clear();
                    }
                    else
                    {
                        EnqueueLoopableFlowAction(0);
                    }
                }
            }
            
            private List<GameFlowData> GetFlowData(string anId)
            {
                //try casting
                allGameFlowPool.TryGetValue(anId, out var tmp);
                
                //
                //return a new copy, we don't want to modify
                //the actual content of the dictionary
                //

                var f = new List<GameFlowData>(tmp);
                return f;
            }

        }


    }
}