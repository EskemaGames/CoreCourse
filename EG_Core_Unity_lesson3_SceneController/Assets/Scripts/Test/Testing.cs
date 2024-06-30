using System;
using System.Collections;
using EG.Core;
using EG.Core.Messages;
using EG.Core.Scenes;
using UnityEngine;

public class Testing : MonoBehaviour
{

    
    
    void Start()
    {
        var menuScene = new MenuScene();
        EG_Core.Self().LoadNewScene(menuScene, OnLoaded);
    }

    private void OnLoaded()
    {
        //do something with the new scene loaded
    }


   
    
}
