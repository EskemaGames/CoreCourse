using System.Collections.Generic;


namespace EG
{
    namespace Core.FlowStateMachine
    {

        //
        // instructions are meant to be "quickly executable"
        // something like spawn a new NPC
        // or give player money, etc, etc
        // something that DOESN'T need a confirmation in the sense that there's no way to access this instruction
        // from the outside, so any logic happens in here.
        // It's a class that just "happens"
        // but we can decide if they will be executed or not via the bool flag
        // cause maybe you want to hold the instruction for later in a queue
        //
        

        [System.Serializable]
        public abstract class Instruction
        {
            protected Dictionary<string, object> parameters = new Dictionary<string, object>();
            protected bool wasExecuted = false;
            protected System.Action<Instruction> onCompleteInstruction;

            public Dictionary<string, object> GetParameters => parameters;
            public bool WasExecuted => wasExecuted;

            
            #region constructor

            public Instruction() { }

            public Instruction(List<string> parameters)
            {
                // WE EXPECT 2 PARAMETERS PER "KEY" in the Json file ("Level, 2", "Money, 100", things like that...)
                //now we convert all those strings passed in a dictionary for "easy" use
                for (var counter = 0; counter < parameters.Count; ++counter)
                {
                    var split = parameters[counter].Split(',');
                    var trimEmpty = split[1].Trim(' ');
                    AddParameters(split[0], trimEmpty);
                }

                wasExecuted = false;
            }
            
            #endregion
            

            public virtual void DoInit() { }

            public virtual void Destroy()
            {
                onCompleteInstruction = null;
                parameters.Clear();
            }

            private void AddParameters(string aName, object anObj)
            {
                parameters.Add(aName, anObj);
            }

            public void SetOnFinish(System.Action<Instruction> anOnCompleteInstruction)
            {
                onCompleteInstruction = anOnCompleteInstruction;
            }

            /// <summary>
            /// we might add conditions to check if we can execute this instruction
            /// </summary>
            /// <returns></returns>
            public abstract bool CheckInstruction();
            
            /// <summary>
            /// put whatever code to execute this instruction, bool to return ok or false if can't execute
            /// also remember to put the callback to finish the instruction
            /// </summary>
            /// <returns></returns>
            public abstract bool DoExecuteInstruction();
            
            /// <summary>
            /// After the above execute function, call the completion when the instruction is ready to be finished
            /// </summary>
            public void DoExecuteOnCompletion()
            {
                onCompleteInstruction?.Invoke(this);
                onCompleteInstruction = null;
            }
            
            public virtual void OnRestart()
            {
                wasExecuted = false;
                onCompleteInstruction = null;
            }

        }


    }//end namespace
}