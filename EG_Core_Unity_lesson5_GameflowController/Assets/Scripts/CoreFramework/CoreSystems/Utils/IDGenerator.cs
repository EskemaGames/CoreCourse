public static class IdGenerator
{

    private static ulong idLong = 0;
    private static uint id = 10; //first 10 reserved for manual asignation, just because I want
    private static int idRandoms = 0;
    private static uint idAudio = 0;
    
    public static uint GetNewUId()
    {
        id++;

        if (id >= uint.MaxValue)
        {
            id = 10;
        }

        return id;
    }
    
    public static ulong GetNewULId()
    {
        idLong++;

        if (idLong >= ulong.MaxValue)
        {
            idLong = 1;
        }

        return idLong;
    }
    
    public static int GetNewId()
    {
        idRandoms++;
    
        if (idRandoms >= int.MaxValue)
        {
            idRandoms = 1;
        }
    
        return idRandoms;
    }

    public static uint GetNewAudioId()
    {
        idAudio++;

        if (idAudio >= uint.MaxValue)
        {
            idAudio = 1;
        }

        return idAudio;
    }
}
