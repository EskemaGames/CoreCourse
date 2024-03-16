using System;
using System.Collections.Generic;



namespace EG
{
    namespace Core.Messages
    {
        
        /// <summary>
        /// Class from Prime31 (thanks my friend)
        /// with some minor changes
        ///
        /// USAGE like
        ///
        /// prepare the object
        /// EG_MessageCameraShakeForcedEffect messageShakeCamera = new EG_MessageCameraShakeForcedEffect();
        ///
        /// set the data for the message
        /// messageShakeCamera.SetData(bla bla bla);
        /// 
        /// finally send the message to the messages controller with the Id of the message
        /// EG_MessagesController<EG_MessageCameraShakeForcedEffect>.Post(1, messageShakeCamera);
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        public static class EG_MessagesController<U>
        {
            private static Dictionary<int, List<Action<U>>> messageTable = new Dictionary<int, List<Action<U>>>(500);


            public static void AddObserver(int aMessageIdType, Action<U> handler)
            {
                List<Action<U>> list = null;
                if (!messageTable.TryGetValue(aMessageIdType, out list))
                {
                    list = new List<Action<U>>();
                    messageTable.Add(aMessageIdType, list);
                }

                if (!list.Contains(handler))
                    messageTable[aMessageIdType].Add(handler);
            }


            public static void RemoveObserver(int aMessageIdType, Action<U> handler)
            {
                List<Action<U>> list = null;
                if (messageTable.TryGetValue(aMessageIdType, out list))
                {
                    if (list.Contains(handler))
                        list.Remove(handler);
                }
            }


            public static void Post(int aMessageIdType, U param)
            {
                List<Action<U>> list = null;
                if (messageTable.TryGetValue(aMessageIdType, out list))
                {
                    if (list.Count == 0) return;

                    for (var i = list.Count - 1; i > -1; --i)
                    {
                        list[i](param);
                    }
                }
            }


            public static void ClearMessageTable(int aMessageIdType)
            {
                if (messageTable.ContainsKey(aMessageIdType))
                    messageTable.Remove(aMessageIdType);
            }


            public static void ClearMessageTable()
            {
                messageTable.Clear();
            }

        }

        
    }
}