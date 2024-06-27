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

//usage examples
//
//
//            
// EG_TimerController.StartTimer(2f, this, anAction =>
// {
//     (anAction.context as YourClass).YourMethodToCallName();
// });
//
//
// stop timer
// EG_TimerController.StopTimer (cacheAction =>
// 	{
// 		(cacheAction.context as YourClass).StopDoingStuffFunction();
// 	});
//


namespace EG
{
    namespace Core.Timer
    {

        [System.Serializable]
        public class EG_TimerSystem : IDestroyable, IUpdateTimedSystems, IPauseGame
        {
            private EG_Timer[] timerList;

            //this will be used as counter to give a new id for each timer
            private uint timerID = 0;
            private int totalList = 0;
            private int currentTimerPositionId = 0;


            #region init and destroy

            public void InitTimerManager(int anAmountOfTimers)
            {
                timerList = new EG_Timer[anAmountOfTimers];

                for (var cnt = 0; cnt < anAmountOfTimers; ++cnt)
                {
                    var timer = new EG_Timer();
                    timerList[cnt] = timer;
                }

                totalList = anAmountOfTimers;
            }

            private void DestroyAndCleanUp()
            {
                for (var cnt = 0; cnt < totalList; ++cnt)
                {
                    if (timerList[cnt] != null)
                    {
                        timerList[cnt].Destroy();
                    }
                }

                System.Array.Clear(timerList, 0, timerList.Length);

                timerList = null;
            }
            
            public void IDestroy()
            {
                DestroyAndCleanUp();
            }

            public void IDestroyUnity()
            {
                DestroyAndCleanUp();
            }

            #endregion


            #region public functions

            public bool CheckIfTimerManagerWasInitiated()
            {
                return timerList != null;
            }

            public void StopAllTimers()
            {
                for (var cnt = 0; cnt < totalList; ++cnt)
                {
                    var timer = timerList[cnt];

                    timer.StopTimer();
                }
            }

            #endregion


            #region control individual timers

            /// <summary>
            /// Pause or Unpause a timer with a given id
            /// </summary>
            /// <param name="aTimerid"></param>
            /// <param name="aIspaused"></param>
            public void PauseTimerWithID(uint aTimerid, bool aIspaused)
            {
                if (aTimerid == 0) return;

                for (var cnt = 0; cnt < totalList; ++cnt)
                {
                    var timer = timerList[cnt];

                    //timer match?
                    //we don't care if it's active, just care about an Id match
                    if (timer.GetTimerID() == aTimerid)
                    {
                        timer.PauseTimer(aIspaused);
                        break;
                    }
                }
            }

            /// <summary>
            /// stop the timer with the given id if it exists
            /// and the timer id is not 0, which means no timer at all
            /// </summary>
            /// <param name="aTimerid"></param>
            public void StopTimerWithID(uint aTimerid)
            {
                if (aTimerid == 0) return;

                for (var cnt = 0; cnt < totalList; ++cnt)
                {
                    var timer = timerList[cnt];

                    //timer match?
                    //we don't care if it's active, just care about an Id match
                    if (timer.GetTimerID() != aTimerid) continue;

                    timer.StopTimer();
                    break;
                }
            }

            /// <summary>
            /// return the first unused timer
            /// </summary>
            /// <returns></returns>
            public EG_Timer GetUnusedTimer()
            {
                for (var cnt = 0; cnt < totalList; ++cnt)
                {
                    var timer = timerList[cnt];

                    if (!timer.GetIsActive())
                        return timer;
                }

                return null;
            }

            #endregion
            
            
            #region start timers

            /// <summary>
            /// start timer with a given time, the context and the callback
            /// </summary>
            /// <param name="aTimerValue"></param>
            /// <param name="aContext"></param>
            /// <param name="aNewcallback"></param>
            public void StartTimer(float aTimerValue, object aContext, Action<EG_Timer> aNewcallback)
            {
                StartTimer(aTimerValue, false, aContext, aNewcallback);
            }
            
            /// <summary>
            /// start timer with a given time, the context, a callback to get an update every tick, and the callback
            /// </summary>
            /// <param name="aTimerValue"></param>
            /// <param name="aContext"></param>
            /// <param name="aOnUpdateFrameTimer"></param>
            /// <param name="aNewcallback"></param>
            public void StartTimer(float aTimerValue, object aContext, Action<EG_Timer> aOnUpdateFrameTimer,Action<EG_Timer> aNewcallback)
            {
                StartTimer(aTimerValue, false, aContext, aOnUpdateFrameTimer, aNewcallback);
            }

            /// <summary>
            /// start timer with a given time, a flag to decide if this timer can be stopped or cannot, the context and the callback
            /// </summary>
            /// <param name="aTimerValue"></param>
            /// <param name="aCannotbePaused"></param>
            /// <param name="aContext"></param>
            /// <param name="aNewcallback"></param>
            public void StartTimer(float aTimerValue, bool aCannotbePaused, object aContext, Action<EG_Timer> aNewcallback)
            {
                var foundTimer = SetTimer(aTimerValue, aCannotbePaused, aContext, aNewcallback);

                if (foundTimer) return;

                //no timer found, restart loop and get a new one
                ResetTimerCounterPosition();
                SetTimer(aTimerValue, aCannotbePaused, aContext, aNewcallback);
            }
            
            /// <summary>
            /// start timer with a given time, a flag to decide if this timer can be stopped or cannot, the context, a callback to get an update every tick, and the callback
            /// </summary>
            /// <param name="aTimerValue"></param>
            /// <param name="aCannotbePaused"></param>
            /// <param name="aContext"></param>
            /// <param name="aOnUpdateFrameTimer"></param>
            /// <param name="aNewcallback"></param>
            public void StartTimer(float aTimerValue, bool aCannotbePaused, object aContext, Action<EG_Timer> aOnUpdateFrameTimer, Action<EG_Timer> aNewcallback)
            {
                var foundTimer = SetTimer(aTimerValue, aCannotbePaused, aContext, aOnUpdateFrameTimer, aNewcallback);
                
                if (foundTimer) return;

                //no timer found, restart loop and get a new one
                ResetTimerCounterPosition();
                SetTimer(aTimerValue, aCannotbePaused, aContext, aOnUpdateFrameTimer, aNewcallback);
            }
            
            private bool SetTimer(float aTimerValue, bool aCannotbePaused, object aContext, Action<EG_Timer> aNewcallback)
            {
                var foundTimer = false;
                
                for (var cnt = currentTimerPositionId; cnt < totalList; ++cnt)
                {
                    var timer = timerList[cnt];

                    if (timer.GetIsActive()) continue;

                    //set the new callback and start the timer
                    timer.SetContext(aContext);
                    timer.SetTimerActionCompletedWithSelfCallback(aNewcallback);
                    timer.SetTimerOnUpdateWithSelfCallback(null);
                    timer.StartTimer(aTimerValue, aCannotbePaused, 0);

                    foundTimer = true;
                    
                    SetTimerCounterPosition(cnt);
                    IncreaseTimerCounterPosition();

                    break;
                }

                return foundTimer;
            }
            
            private bool SetTimer(float aTimerValue, bool aCannotbePaused, object aContext, Action<EG_Timer> aOnUpdateFrameTimer, Action<EG_Timer> aNewcallback)
            {
                var foundTimer = false;
                
                for (var cnt = currentTimerPositionId; cnt < totalList; ++cnt)
                {
                    var timer = timerList[cnt];

                    if (timer.GetIsActive()) continue;

                    //set the new callback and start the timer
                    timer.SetContext(aContext);
                    timer.SetTimerActionCompletedWithSelfCallback(aNewcallback);
                    timer.SetTimerOnUpdateWithSelfCallback(aOnUpdateFrameTimer);
                    timer.StartTimer(aTimerValue, aCannotbePaused, 0);

                    foundTimer = true;
                    
                    SetTimerCounterPosition(cnt);
                    IncreaseTimerCounterPosition();

                    break;
                }

                return foundTimer;
            }
            
            #endregion
            
            
            #region start timer with id

            /// <summary>
            /// start timer with a given time, the context and the callback, RETURNS and id with the current timer in order to stop it later if needed
            /// </summary>
            /// <param name="aTimerValue"></param>
            /// <param name="aContext"></param>
            /// <param name="aNewcallback"></param>
            /// <returns></returns>
            public uint StartTimerId(float aTimerValue, object aContext, Action<EG_Timer> aNewcallback)
            {
                return StartTimerId(aTimerValue, false, aContext, aNewcallback);
            }
            
            /// <summary>
            /// start timer with a given time, the context and the callback, RETURNS and id with the current timer in order to stop it later if needed
            /// </summary>
            /// <param name="aTimerValue"></param>
            /// <param name="aContext"></param>
            /// <param name="aOnUpdateFrameTimer"></param>
            /// <param name="aNewcallback"></param>
            /// <returns></returns>
            public uint StartTimerId(float aTimerValue, object aContext, Action<EG_Timer> aOnUpdateFrameTimer, Action<EG_Timer> aNewcallback)
            {
                return StartTimerId(aTimerValue, false, aContext, aOnUpdateFrameTimer, aNewcallback);
            }

            /// <summary>
            /// start timer with a given time, the context and the callback, RETURNS and id with the current timer in order to stop it later if needed
            /// RETURNS an id with the timer
            /// </summary>
            /// <param name="aTimerValue"></param>
            /// <param name="aCannotbePaused"></param>
            /// <param name="aContext"></param>
            /// <param name="aNewcallback"></param>
            /// <returns></returns>
            public uint StartTimerId(float aTimerValue, bool aCannotbePaused, object aContext, Action<EG_Timer> aNewcallback)
            {
                var foundId = SetTimerWithId(aTimerValue, aCannotbePaused, aContext, aNewcallback);

                if (foundId != 0) return timerID;

                //no timer found, restart loop and get a new one
                ResetTimerCounterPosition();
                return SetTimerWithId(aTimerValue, aCannotbePaused, aContext, aNewcallback);
            }
            
            /// <summary>
            /// start timer with a given time, a flag to decide if this timer can be stopped or cannot, the context, a callback to get an update every tick, and the callback
            /// RETURNS an id with the timer
            /// </summary>
            /// <param name="aTimerValue"></param>
            /// <param name="aCannotbePaused"></param>
            /// <param name="aContext"></param>
            /// <param name="aOnUpdateFrameTimer"></param>
            /// <param name="aNewcallback"></param>
            /// <returns></returns>
            public uint StartTimerId(float aTimerValue, bool aCannotbePaused, object aContext, Action<EG_Timer> aOnUpdateFrameTimer, Action<EG_Timer> aNewcallback)
            {
                var foundId = SetTimerWithId(aTimerValue, aCannotbePaused, aContext, aOnUpdateFrameTimer, aNewcallback);

                if (foundId != 0) return timerID;

                //no timer found, restart loop and get a new one
                ResetTimerCounterPosition();
                return SetTimerWithId(aTimerValue, aCannotbePaused, aContext, aOnUpdateFrameTimer, aNewcallback);
            }

            
            private uint SetTimerWithId(float aTimerValue, bool aCannotbePaused, object aContext, Action<EG_Timer> aNewcallback)
            {
                IncreaseTimerId();

                for (var cnt = currentTimerPositionId; cnt < totalList; ++cnt)
                {
                    var timer = timerList[cnt];

                    if (timer.GetIsActive()) continue;

                    //set the new callback and start the timer
                    timer.SetContext(aContext);
                    timer.SetTimerActionCompletedWithSelfCallback(aNewcallback);
                    timer.SetTimerOnUpdateWithSelfCallback(null);
                    timer.StartTimer(aTimerValue, aCannotbePaused, timerID);
                    
                    SetTimerCounterPosition(cnt);
                    IncreaseTimerCounterPosition();

                    return timerID;
                }

                return 0;
            }
            
            private uint SetTimerWithId(float aTimerValue, bool aCannotbePaused, object aContext,  Action<EG_Timer> aOnUpdateFrameTimer, Action<EG_Timer> aNewcallback)
            {
                IncreaseTimerId();

                for (var cnt = currentTimerPositionId; cnt < totalList; ++cnt)
                {
                    var timer = timerList[cnt];

                    if (timer.GetIsActive()) continue;

                    //set the new callback and start the timer
                    timer.SetContext(aContext);
                    timer.SetTimerActionCompletedWithSelfCallback(aNewcallback);
                    timer.SetTimerOnUpdateWithSelfCallback(aOnUpdateFrameTimer);
                    timer.StartTimer(aTimerValue, aCannotbePaused, timerID);

                    SetTimerCounterPosition(cnt);
                    IncreaseTimerCounterPosition();

                    return timerID;
                }

                return 0;
            }
            
            #endregion


            #region loop timer internal functions

            private void IncreaseTimerId()
            {
                timerID++;

                //if overflow start again in 1, 0 is the reserved value for a "not used" timer
                if (timerID >= uint.MaxValue)
                    timerID = 1;
            }

            private void IncreaseTimerCounterPosition()
            {
                //kind of a way to loop the whole list.
                //each time we found a valid timer we increase this position
                //that way instead of starting the loop from 0 each frame
                //we start from the last position we used as valid
                currentTimerPositionId = (currentTimerPositionId + 1) % totalList;
            }

            private void SetTimerCounterPosition(int aPosition)
            {
                currentTimerPositionId = aPosition;
            }

            private void ResetTimerCounterPosition()
            {
                //after looping and not finding any free timer, we start from 0
                currentTimerPositionId = 0;
            }
            
            #endregion

            
            #region pause
            
            public void IOnGamePaused(bool aGameIsPaused)
            {
                for (var cnt = 0; cnt < totalList; ++cnt)
                {
                    var timer = timerList[cnt];

                    timer.PauseTimer(aGameIsPaused);
                }
            }
            
            #endregion
            

            #region update
            
            public void IOnUpdate(float aDeltaTime = 1)
            {
                // process 2 timers ot once to reduce the N(0) complexity of the list
                // BUT this will cause an error is the list is not a multiple of 2 number, like 1000, 1002, etc, etc
                // a number like 1001 will cause a crash
                for (var cnt = 0; cnt < totalList; cnt += 2)
                {
                    var timer = timerList[cnt];
                    timer.UpdateTimer();

                    var timer2 = timerList[cnt + 1];
                    timer2.UpdateTimer();
                }
            }

            //this functions comes from the interface but they are not needed in this class at all
            public void IOnFixedUpdate(float aDeltaTime = 1) { }
            public void IOnLateUpdate(float aDeltaTime = 1) { }
            
            #endregion
            
            
        }

    }
}