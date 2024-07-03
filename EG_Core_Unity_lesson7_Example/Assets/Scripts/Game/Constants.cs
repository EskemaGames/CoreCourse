

public class Constants
{

        
        public const int LanguageVersionId = 1;
        public const int DataVersionId = 1;
        public const string PreferencesNameData = "UserData";
        public const string SaveGameNameData = "SaveGame";
        public const string SaveExtension = ".data";
        
        public const uint EmptyId = 0; //used for any "non-used" stuff


        /// <summary>
        /// used by the state machine "GameFlowCore.json"
        /// of course can be used anywhere...
        /// </summary>
        public class ParametersToParse
        {
                public const string Number = "Number";
                public const string TotalScenes = "TotalScenes";
                public const string TotalUnityScenes = "TotalUnityScenes";
                public const string UnityScene = "UnityScene";
                public const string SceneName = "SceneName";
                public const string Bootstrap = "Bootstrap";
                public const string DecisionId = "DecisionId";
                public const string NextFlowAction = "NextFlowAction";
                public const string FlowToExecute = "FlowToExecute";

        }


        public class FlowStates
        {
                //some hardcoded strings for easy access to the core flow states
                public const string FlowExitFromGameplay = "FlowExitFromGameplay";
                public const string FlowMenuSceneToPlayAdventureMode = "FlowMenuSceneToPlayAdventureMode";
        }






}
