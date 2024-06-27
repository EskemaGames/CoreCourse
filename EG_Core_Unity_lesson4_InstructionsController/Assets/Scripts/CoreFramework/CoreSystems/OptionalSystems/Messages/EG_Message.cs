//
//Attribution-ShareAlike 
//CC BY-SA
//
//This license lets others remix, tweak, and build upon your work even for commercial purposes, 
//	as long as they credit you and license their new creations under the identical terms. 
//	This license is often compared to “copyleft” free and open source software licenses. 
//	All new works based on yours will carry the same license, so any derivatives will also allow commercial use. 
//	This is the license used by Wikipedia, and is recommended for materials that would benefit from incorporating content from Wikipedia and similarly licensed projects.


namespace EG
{
    namespace Core.Messages
    {
        //
        // instructions are meant to be "quickly executable"
        // something like spawn a new NPC
        // or give player money, etc, etc
        // something that DOESN'T need a confirmation by the player
        // just something that "happens"
        //
        // in case of a network game we can add some fields like
        // ID for the character who sent this
        // bool to know if this instruction should be sent online (replication)
        //

        [System.Serializable]
        public abstract class EG_Message
        {
            public bool IsLocal { get; protected set; }
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
