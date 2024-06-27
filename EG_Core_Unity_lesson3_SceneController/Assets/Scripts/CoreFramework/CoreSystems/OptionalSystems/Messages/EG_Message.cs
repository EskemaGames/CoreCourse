namespace EG
{
    namespace Core.Messages
    {
        //
        // messages are the observable pattern 
        // you suscribe to a message and listen for the incoming "information"
        // messages CANNOT be replied, the just come and go
        //
        // in case of a network game we can add some fields like
        // ID for the character who sent this
        // bool to know if this instruction should be sent online (replication)
        //

        [System.Serializable]
        public abstract class EG_Message
        {
            public bool IsLocal { get; protected set; } // just in case we do some multiplayer stuff, this is an example
            public uint SenderId { get; protected set; } //whose entity is using this message


            //lazy constructor
            public EG_Message()
            {
                IsLocal = true;
                SenderId = 0;
            }

            public EG_Message(uint aSenderId)
            {
                IsLocal = true;
                SenderId = aSenderId;
            }



        }

    }
}