//
//Attribution-ShareAlike 
//CC BY-SA
//
//This license lets others remix, tweak, and build upon your work even for commercial purposes, 
//	as long as they credit you and license their new creations under the identical terms. 
//	This license is often compared to “copyleft” free and open source software licenses. 
//	All new works based on yours will carry the same license, so any derivatives will also allow commercial use. 
//	This is the license used by Wikipedia, and is recommended for materials that would benefit from incorporating content from Wikipedia and similarly licensed projects.


using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace EG
{
    
    namespace Core.Util
    {
        //we really don't need a lock because unity is not multi-thread, but just in case
        //they decide to update the engine in Unity6.x or whatever with multi-thread support
        public class EG_Singleton<T> : MonoBehaviour where T : EG_Singleton<T>
        {
            private static T self;
            private static bool isAppClosing = false;
            private static bool isCreated = false;

            private bool initialized { get; set; }

            private static object syncRoot = new Object();

            public static T Self()
            {
                if (isAppClosing)
                {
                    return null;
                }

                if (!Application.isPlaying) return null;

                if (isCreated) return self;

                lock (syncRoot)
                {
                    //null, try to get the object if it's on the scene
                    self = GameObject.FindObjectOfType<T>();

                    //still null?
                    if (!self)
                    {
                        //create new instance
                        GameObject go = new GameObject(typeof(T).Name);
                        self = go.AddComponent<T>();
                    }

                    isCreated = true;

                    //initialize instance if necessary
                    if (!self.initialized)
                    {
                        self.Initialize();
                        self.initialized = true;
                    }
                }

                return self;
            }

            private void Awake()
            {
#if UNITY_EDITOR

                if (!EditorApplication.isPlaying)
                {
                    return;
                }
#endif

                if (self == null)
                {
                    Self();
                }
            }


            private void OnApplicationQuit()
            {
                isAppClosing = true;
                isCreated = false;
                self = null;
            }

            public virtual void OnDestroy()
            {
                isCreated = false;
                initialized = false;
                self = null;
            }

            protected virtual void Initialize(bool aDontDestroy = false)
            {
                if (aDontDestroy)
                {
                    DontDestroyOnLoad(self.gameObject);
                }
            }
        }
        
        
    }
}