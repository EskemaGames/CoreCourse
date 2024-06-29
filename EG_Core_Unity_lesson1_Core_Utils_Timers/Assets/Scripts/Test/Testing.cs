using System;
using System.Collections;
using EG.Core;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private uint timerId = 0;
    
    // // Start is called before the first frame update
    // void Start()
    // {
    //     StartCoroutine(Fire());
    //     
    //     FireWithTimer();
    // }
    //
    // private void OnDestroy()
    // {
    //     EG_Core.Self().StopTimerWithID(timerId);
    // }
    //
    //
    // private IEnumerator Fire()
    // {
    //     weapon.Shoot();
    //     weapon.Disable();
    //     var time = GetComponent<Animation>().PlayShootAnim();
    //     
    //     yield return new WaitForSeconds(time);
    //     
    //     weapon.Enable();
    // }
    //
    // private void FireWithTimer()
    // {
    //     weapon.Shoot();
    //     var time = GetComponent<Animation>().PlayShootAnim();
    //     
    //     timerId = EG_Core.Self().StartTimerId(time, this, cacheAction => { (cacheAction.Context as Testing).EnableWeapon(1); });
    // }
    //
    // private void EnableWeapon(int weaponId)
    // {
    //     weapon.Enable();
    // }
    
    
}
