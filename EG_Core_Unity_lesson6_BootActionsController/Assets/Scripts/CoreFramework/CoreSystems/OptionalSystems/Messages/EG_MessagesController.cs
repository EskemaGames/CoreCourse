//
//Attribution-ShareAlike 
//CC BY-SA
//
//This license lets others remix, tweak, and build upon your work even for commercial purposes, 
//	as long as they credit you and license their new creations under the identical terms. 
//	This license is often compared to “copyleft” free and open source software licenses. 
//	All new works based on yours will carry the same license, so any derivatives will also allow commercial use. 
//	This is the license used by Wikipedia, and is recommended for materials that would benefit from incorporating content from Wikipedia and similarly licensed projects.


using System;
using System.Collections.Generic;



namespace EG
{
    namespace Core.Messages
    {

        public static class EG_MessagesController<U>
        {
            private static Dictionary<int, List<Action<U>>> messageTable = new Dictionary<int, List<Action<U>>>(500);


            public static void AddObserver(int messageType, Action<U> handler)
            {
                List<Action<U>> list = null;
                if (!messageTable.TryGetValue(messageType, out list))
                {
                    list = new List<Action<U>>();
                    messageTable.Add(messageType, list);
                }

                if (!list.Contains(handler))
                    messageTable[messageType].Add(handler);
            }


            public static void RemoveObserver(int messageType, Action<U> handler)
            {
                List<Action<U>> list = null;
                if (messageTable.TryGetValue(messageType, out list))
                {
                    if (list.Contains(handler))
                        list.Remove(handler);
                }
            }


            public static void Post(int messageType, U param)
            {
                List<Action<U>> list = null;
                if (messageTable.TryGetValue(messageType, out list))
                {
                    if (list.Count == 0) return;

                    for (var i = list.Count - 1; i > -1; --i)
                    {
                        list[i](param);
                    }
                }
            }


            public static void ClearMessageTable(int messageType)
            {
                if (messageTable.ContainsKey(messageType))
                    messageTable.Remove(messageType);
            }


            public static void ClearMessageTable()
            {
                messageTable.Clear();
            }

        }

        
    }
}