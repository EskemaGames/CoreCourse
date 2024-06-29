using System;
using System.Collections;
using EG.Core;
using EG.Core.Messages;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public enum MessageIds
    {
        None = 0,
        CameraShake = 1,
        UpdateEnemyHealthUI = 2
    }
    
    private EG_MessageCameraShakeForcedEffect messageCameraShakeForcedEffect = null;

    
    
    void Start()
    {
        messageCameraShakeForcedEffect = new EG_MessageCameraShakeForcedEffect();
        
        DoCameraShake();
        
        //subscribe and unsubscribe, this goes in whatever class want to listen to this message
        EG_MessagesController<EG_MessageCameraShakeForcedEffect>.AddObserver((int) MessageIds.CameraShake, OnCameraShake);
        EG_MessagesController<EG_MessageCameraShakeForcedEffect>.RemoveObserver((int) MessageIds.CameraShake, OnCameraShake);
    }

    private void DoCameraShake()
    {
        messageCameraShakeForcedEffect.SetData(1f, 1f, 0.5f, true, 1);
        EG_MessagesController<EG_MessageCameraShakeForcedEffect>.Post( (int)MessageIds.CameraShake, messageCameraShakeForcedEffect);
    }


    private void OnCameraShake(EG_MessageCameraShakeForcedEffect aMessage)
    {
        //do whatever you want to do with the message
    }
    
    
   
    
}
