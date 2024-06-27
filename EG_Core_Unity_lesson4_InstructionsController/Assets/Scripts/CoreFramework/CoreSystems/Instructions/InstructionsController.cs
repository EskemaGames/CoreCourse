using System.Collections.Generic;
using UnityEngine;


namespace EG
{
    namespace Core.FlowStateMachine
    {

        [System.Serializable]
        public class InstructionsController : IRestart
        {
            //assume an initial amount of 50 to avoid changed the size often
            private Queue<Instruction> instructions = new Queue<Instruction>(50);

            private bool isRunning = false;
            private uint timerId = 0;

            private System.Action onErrorInstruction = null; //for the internal queue only, not exposed
            private System.Action onCompleteInstruction = null; //for the internal queue only, not exposed
            private System.Action onAllInstructionsProcessedAndExecuted = null;

            public int GetTotalInstructions => instructions.Count;



            #region init and destroy

            public void Init()
            {
                instructions.Clear();
            }

            public void IDestroy()
            {
                EG_Core.Self().StopCoreTimerWithID(timerId);
                
                onAllInstructionsProcessedAndExecuted = null;
                onCompleteInstruction = null;
                onErrorInstruction = null;

                DestroyAndCleanInstructions();
            }

            public void IDestroyUnity()
            {
                IDestroy();
            }

            public void ClearAll()
            {
                instructions.Clear();
            }

            #endregion

            
            #region restart

            public void IOnRestart()
            {
                EG_Core.Self().StopCoreTimerWithID(timerId);
                
                onAllInstructionsProcessedAndExecuted = null;
                onErrorInstruction = null;
                onCompleteInstruction = null;

                RestartAndCleanInstructions();
            }
            
            #endregion
            

            #region instructions to process

            public void AddInstruction(Instruction anInstruction, System.Action<Instruction> aOnCompleteInstruction)
            {
                anInstruction.SetOnFinish(aOnCompleteInstruction);
                
                instructions.Enqueue(anInstruction);
            }

            //one or many, the callback is the same, cause we wait until ALL instructions have been executed
            public void ExecuteInstruction(System.Action onAllInstructionsExecuted = null)
            {
                onAllInstructionsProcessedAndExecuted = onAllInstructionsExecuted;
                ProcessInstruction();
            }

            //one or many, the callback is the same, cause we wait until ALL instructions have been executed
            public void ExecuteAllInstructions(System.Action onAllInstructionsProcessed)
            {
                onAllInstructionsProcessedAndExecuted = onAllInstructionsProcessed;
                ProcessAllInstructions();
            }

            #endregion
            

            #region private process and queue instructions
            
            private void RestartAndCleanInstructions()
            {
                //recursively restart all instructions
                if (instructions.Count <= 0) return;
                
                Instruction instruction = instructions.Dequeue();
                instruction.OnRestart();
                
                //a small "delay" to allow the instruction to restart
                //of course we DON'T know if that time will be enough
                //it's just a way to help the system a bit, nothing else and nothing more
                timerId = EG_Core.Self().StartCoreTimerId(0.08f, this,
                    cacheAction =>
                    {
                        (cacheAction.Context as InstructionsController).RestartAndCleanInstructions();
                    });
            }
            
            private void DestroyAndCleanInstructions()
            {
                //recursively restart all instructions
                if (instructions?.Count <= 0) return;
                
                Instruction instruction = instructions?.Dequeue();
                instruction?.Destroy();
                
                //a small "delay" to allow the instruction to restart
                //of course we DON'T know if that time will be enough
                //it's just a way to help the system a bit, nothing else and nothing more
                timerId = EG_Core.Self().StartCoreTimerId(0.08f, this,
                    cacheAction =>
                    {
                        (cacheAction.Context as InstructionsController).DestroyAndCleanInstructions();
                    });
            }

            private Instruction GetOneInstruction()
            {
                //someone called the function again, but we are still processing callbacks
                //we return null and get the hell out of here
                if (isRunning) return null;
                
                //green light to process that instruction
                isRunning = true;

                var instruction = instructions.Dequeue();

                return instruction;
            }

            private bool CheckIfCanProcessMoreInstructions()
            {
                if (instructions.Count != 0) return true;
                
                isRunning = false;
                return false;
            }

            #endregion

            
            #region process instructions
            
            /// <summary>
            /// Process one and only one instruction
            /// </summary>
            private void ProcessInstruction()
            {
                if (!CheckIfCanProcessMoreInstructions())
                {
                    onAllInstructionsProcessedAndExecuted?.Invoke();
                    return;
                }

                Instruction instruction = GetOneInstruction();

                if (instruction != null)
                {
                    // for now the error is not treated and we use the same callback to keep the flow going
                    // I only have error thing here just in case I want to expand the system and do something
                    // with errors
                    // for now you have to program everything to not care about errors
                    // like instruction always execute, there's no "error" like check if player is alive otherwise throw an error (for example)
                    ExecuteInstruction(instruction, OnOneInstructionExecuted, OnOneInstructionExecuted); 
                    
                    return;
                }

                // if instruction was null, it means another one is already executing
                // so we wait a bit and try to execute again,
                // the system is busy processing another instruction right now
                timerId = EG_Core.Self().StartCoreTimerId(0.07f, this,
                    cacheAction =>
                    {
                        (cacheAction.Context as InstructionsController).ProcessInstruction();
                    });
            }

            /// <summary>
            /// Recursive function that will be called until no more instructions left in our queue
            /// </summary>
            private void ProcessAllInstructions()
            {
                var instruction = GetOneInstruction();

                if (instruction != null)
                {
                    // for now the error is not treated and we use the same callback to keep the flow going
                    // I only have error thing here just in case I want to expand the system and do something
                    // with errors
                    // for now you have to program everything to not care about errors
                    // like instruction always execute, there's no "error" like check if player is alive otherwise throw an error (for example)
                    ExecuteInstruction(instruction, OnInstructionExecuted, OnInstructionExecuted);

                    return;
                }

                // if instruction was null, it means another one is already executing
                // so we wait a bit and try to execute again,
                // the system is busy processing another instruction right now
                timerId = EG_Core.Self().StartCoreTimerId(0.07f, this,
                    cacheAction =>
                    {
                        (cacheAction.Context as InstructionsController).OnInstructionExecuted();
                    });
            }

            private void ExecuteInstruction(Instruction anInstruction,
                System.Action aOnCompleteInstruction,
                System.Action anOnErrorInstruction)
            {
                onErrorInstruction = anOnErrorInstruction;
                onCompleteInstruction = aOnCompleteInstruction;
                
                if (anInstruction.ExecuteInstruction())
                {
                    onCompleteInstruction?.Invoke();
                    isRunning = false;
                    return;
                }
                
                onErrorInstruction?.Invoke();
                isRunning = false;
            }
            
            /// <summary>
            /// callback for the ProcessAllInstructions
            /// </summary>
            private void OnInstructionExecuted()
            {
                if (!CheckIfCanProcessMoreInstructions())
                {
                    onAllInstructionsProcessedAndExecuted?.Invoke();
                    
                    return;
                }

                ProcessAllInstructions();
            }
            
            /// <summary>
            /// callback for the ProcessInstruction, which means only 1 executed
            /// </summary>
            private void OnOneInstructionExecuted()
            {
                onAllInstructionsProcessedAndExecuted?.Invoke();
            }

            #endregion


    

        }


    }
}