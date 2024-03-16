using System;
using UnityEngine;

namespace EG
{
    namespace Core.Timer
    {
        [System.Serializable]
        public class EG_Timer
        {
            private float waitingTime = float.MaxValue; //total time to complete the timer
            private double timer = 0.0; //the actual timer counter
            private uint idTimer = 0; //the id of this timer in order to stop it
            private bool timerInUse = false;
            private Action<EG_Timer> cachedDelegateAction = null;
            private Action<EG_Timer> onUpdateFramecachedDelegateAction = null;
            private double timerWhenEnterPause = 0.0;

            public bool CannotBePaused { private set; get; }
            public bool IsActive { get; private set; }
            public object Context { get; private set; }


            public EG_Timer()
            {
                timerWhenEnterPause = 0.0;
                CannotBePaused = false;
                waitingTime = float.MaxValue;
                timer = 0.0;
                idTimer = 0;
                IsActive = false;
                timerInUse = false;
            }

            public void SetContext(object aContext)
            {
                Context = aContext;
            }

            public void SetTimerActionCompletedWithSelfCallback(Action<EG_Timer> aNewcallback)
            {
                cachedDelegateAction = aNewcallback;
            }
            
            public void SetTimerOnUpdateWithSelfCallback(Action<EG_Timer> aNewcallback)
            {
                onUpdateFramecachedDelegateAction = aNewcallback;
            }


            public bool GetIsActive()
            {
                return IsActive && timerInUse;
            }

            public double GetTimerValue()
            {
                return timer;
            }

            public uint GetTimerID()
            {
                return idTimer;
            }

            public float GetWaitingTimer()
            {
                return waitingTime;
            }

            public void Destroy()
            {
                timerWhenEnterPause = 0.0;
                CannotBePaused = false;
                waitingTime = float.MaxValue;
                timer = 0.0;
                idTimer = 0;
                IsActive = false;
                timerInUse = false;
                SetContext(null);
                SetTimerActionCompletedWithSelfCallback(null);
                SetTimerOnUpdateWithSelfCallback(null);
            }


            #region timer functionality

            //pause the active timer or unpause the inactive timer
            public void PauseTimer(bool aGameIsPaused)
            {
                if (CannotBePaused) return;

                if (IsActive && aGameIsPaused)
                {
                    timerWhenEnterPause = Time.time;
                    timerInUse = true;
                    IsActive = false;
                }
                else if (!aGameIsPaused && timerInUse)
                {
                    var totalTimeInPause = timerWhenEnterPause != 0.0 ? timerWhenEnterPause : timer;
                    var elapsed = Time.time - totalTimeInPause;

                    //increase the amount of time the timer has been paused
                    waitingTime += (float) elapsed;
                    timerWhenEnterPause = 0.0;
                    IsActive = true;
                }
            }


            public void StartTimer(float aWaitValue, bool aCannotbePaused, uint aTimerid)
            {
                timerWhenEnterPause = 0.0;
                waitingTime = aWaitValue;
                timer = Time.time;
                CannotBePaused = aCannotbePaused;
                IsActive = true;
                idTimer = aTimerid;
                timerInUse = true;
            }

            public void StopTimer()
            {
                Context = null;
                cachedDelegateAction = null;
                timerWhenEnterPause = 0.0;
                CannotBePaused = false;
                waitingTime = float.MaxValue;
                timer = 0.0;
                idTimer = 0;
                IsActive = false;
                timerInUse = false;
            }

            public void UpdateTimer()
            {
                if (!IsActive) return;

                //calculate the difference from start to the current time
                var elapsed = Time.time - timer;

                onUpdateFramecachedDelegateAction?.Invoke(this);
                
                //condition met, time to fire up the delegate on completion
                if (!(elapsed >= waitingTime)) return;

                cachedDelegateAction?.Invoke(this);

                StopTimer();
            }

            #endregion
            
        }
    }
}