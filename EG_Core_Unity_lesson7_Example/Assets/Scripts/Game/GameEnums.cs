using System;

public class GameEnums
{
    
    [Serializable]
    public enum Difficulty
    {
        EASY,
        NORMAL,
        HARD,
        MAX
    };

    [Serializable]
    public enum GroupTypes
    {
        Players,
        Enemies,
        Npc,
        MixedPlEne,
        Boss,
        Max
    };
    

}



