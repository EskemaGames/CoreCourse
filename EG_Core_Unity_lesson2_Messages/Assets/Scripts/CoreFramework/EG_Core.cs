﻿//
//Attribution-ShareAlike 
//CC BY-SA
//
//This license lets others remix, tweak, and build upon your work even for commercial purposes, 
//	as long as they credit you and license their new creations under the identical terms. 
//	This license is often compared to “copyleft” free and open source software licenses. 
//	All new works based on yours will carry the same license, so any derivatives will also allow commercial use. 
//	This is the license used by Wikipedia, and is recommended for materials that would benefit from incorporating content from Wikipedia and similarly licensed projects.

using UnityEngine;
using System;
using System.Collections.Generic;
using DG.Tweening;
using EG.Core.Timer;
using EG.Core.Util;


namespace EG
{
    namespace Core
    {
        public class EG_Core : EG_Singleton<EG_Core>
        {
            // Editor links
            // ------------------------------------------------
            [SerializeField] private int totalTimersPooled = 2500;
            [SerializeField] private bool setGame30FPS = false;
            [SerializeField] private bool slowPhysicsFor60FPS = false;
            [SerializeField] private bool usesMouse = false;
            [SerializeField] private bool useGamepad = false;

            // Core Services
            // ------------------------------------------------
            private EG_TimerController timer = null;
            private EG_TimerController timerCore = null;
      
            private List<IDestroyable> destroyableSystems = new List<IDestroyable>(10);
            private List<IUpdateTimedSystems> updateTimedSystems = new List<IUpdateTimedSystems>(3);
      
            
            //fixed size for all internal operations, should be enough
            //otherwise just increase the amount
            private int totalTimersCore = 1500; 
            private bool gamePaused = false;
            private bool unityPlayerInPause = false;
            private int totalUpdateSystems = 0;
            

            #region setters/getters
            
            public bool GetUseMouse => usesMouse;
            
            public bool GetUseGamepad => useGamepad;

            public bool Is30FPS => setGame30FPS;
            
            #endregion




            #region pause game

            private System.Action<bool> onGamePaused = null; //anyone can subscribe to this and get notified if the game is paused

            public bool GamePaused => gamePaused;
            
            public void SubscribeToPause(System.Action<bool> onFunction)
            {
                onGamePaused += onFunction;
            }

            public void UnSubscribeToPause(System.Action<bool> onFunction)
            {
                onGamePaused -= onFunction;
            }

            public void SetCorePaused()
            {
                gamePaused = true;
                timerCore.IOnGamePaused(gamePaused);
                timer.IOnGamePaused(gamePaused);
                onGamePaused?.Invoke(gamePaused);
            }

            public void SetCoreUnPaused()
            {
                gamePaused = false;
                timerCore.IOnGamePaused(gamePaused);
                timer.IOnGamePaused(gamePaused);
                onGamePaused?.Invoke(gamePaused);
            }

            #endregion


            #region monobehaviours and init

            protected override void Initialize(bool aDontDestroy = false)
            {
                base.Initialize(true);

                gamePaused = false;
                
                Application.targetFrameRate = 60;

                LoadCoreSystems();
            }

            private void Start()
            {
                if (setGame30FPS)
                {
                    Application.targetFrameRate = 30;
                    Time.fixedDeltaTime = 0.033333f; //physics at 30fps
                }
                else
                {
                    Application.targetFrameRate = 60;
                    Time.fixedDeltaTime = slowPhysicsFor60FPS ? 0.02f : 0.0167f;
                }
                
                // Load libraries
                InitExternalPlugins();
            }

            public override void OnDestroy()
            {
                updateTimedSystems.Clear();

                for (var i = destroyableSystems.Count -1; i > -1; --i)
                {
                    destroyableSystems[i]?.IDestroyUnity();
                }

                destroyableSystems.Clear();

                base.OnDestroy();
            }

            private void Update()
            {
                //if we go outside the app focus (minimize or something like that)
                if (unityPlayerInPause) return;
                
                for (var i = 0; i < totalUpdateSystems; ++i)
                {
                    updateTimedSystems[i].IOnUpdate(1f);
                }
            }

            //unity went to background or came from it... (not a game pause)
            private void OnApplicationPause(bool pause)
            {
                unityPlayerInPause = pause;
            }

            #endregion


            #region load internals
            
            //any base system like sound controller, timers, language. Any BASE and DEFAULT thing
            //common to ANY games that you can create, don't put anything specific for your game here
            //in this "manager" class
            private void LoadCoreSystems()
            {
                timerCore = new EG_TimerController();
                System.GC.KeepAlive(timerCore);
                timerCore.InitTimerManager(totalTimersCore);
                
                timer = new EG_TimerController();
                System.GC.KeepAlive(timer);
                timer.InitTimerManager(totalTimersPooled);
                
                destroyableSystems.Add(timerCore);
                destroyableSystems.Add(timer);

                AddUpdatableSystem(timerCore);
                AddUpdatableSystem(timer);
            }

            private void AddUpdatableSystem(IUpdateTimedSystems aSystem)
            {
                updateTimedSystems.Add(aSystem);
                totalUpdateSystems++;
            }

            private void InitExternalPlugins()
            {
                DOTween.Init().SetCapacity(400, 10);
            }

            #endregion


            #region api timer
            
            public bool CheckIfTimerManagerWasInitiated()
            {
                return timer.CheckIfTimerManagerWasInitiated();
            }
            
            public void StopAllTimers()
            {
                timer.StopAllTimers();
            }

            public void StopTimerWithID(uint aTimerid)
            {
                timer.StopTimerWithID(aTimerid);
            }

            public EG_Timer GetUnusedTimer()
            {
                return timer.GetUnusedTimer();
            }

            public void StartTimer(float aTimerValue, object aContext, Action<EG_Timer> aNewcallback)
            {
                timer.StartTimer(aTimerValue, aContext, aNewcallback);
            }
            public void StartTimer(float aTimerValue, object aContext, Action<EG_Timer> aOnUpdateFrame, Action<EG_Timer> aNewcallback)
            {
                timer.StartTimer(aTimerValue, aContext, aOnUpdateFrame, aNewcallback);
            }

            public void StartTimer(float aTimerValue, bool aCannotbePaused, object aContext, Action<EG_Timer> aNewcallback)
            {
                timer.StartTimer(aTimerValue, aCannotbePaused, aContext, aNewcallback);
            }
            
            public void StartTimer(float aTimerValue, bool aCannotbePaused, object aContext, Action<EG_Timer> aOnUpdateFrame, Action<EG_Timer> aNewcallback)
            {
                timer.StartTimer(aTimerValue, aCannotbePaused, aContext, aOnUpdateFrame, aNewcallback);
            }

            public uint StartTimerId(float aTimerValue, object aContext, Action<EG_Timer> aNewcallback)
            {
                return timer.StartTimerId(aTimerValue, aContext, aNewcallback);
            }
            
            public uint StartTimerId(float aTimerValue, object aContext, Action<EG_Timer> aOnUpdateFrame, Action<EG_Timer> aNewcallback)
            {
                return timer.StartTimerId(aTimerValue, aContext, aOnUpdateFrame, aNewcallback);
            }

            public uint StartTimerId(float aTimerValue, bool aCannotbePaused, object aContext,
                Action<EG_Timer> aNewcallback)
            {
                return timer.StartTimerId(aTimerValue, aCannotbePaused, aContext, aNewcallback);
            }
            
            public uint StartTimerId(float aTimerValue, bool aCannotbePaused, object aContext,
                Action<EG_Timer> aOnUpdateFrame, Action<EG_Timer> aNewcallback)
            {
                return timer.StartTimerId(aTimerValue, aCannotbePaused, aContext, aOnUpdateFrame, aNewcallback);
            }

            #endregion
            
            
            #region api timer core

            public bool CheckIfTimerCoreWasInitiated()
            {
                return timerCore.CheckIfTimerManagerWasInitiated();
            }
            
            public void StopAllCoreTimers()
            {
                timerCore.StopAllTimers();
            }

            public void StopCoreTimerWithID(uint aTimerid)
            {
                timerCore.StopTimerWithID(aTimerid);
            }
            
            public void StartCoreTimer(float aTimerValue, object aContext, Action<EG_Timer> aNewcallback)
            {
                timerCore.StartTimer(aTimerValue, aContext, aNewcallback);
            }
            public void StartCoreTimer(float aTimerValue, object aContext, Action<EG_Timer> aOnUpdateFrame, Action<EG_Timer> aNewcallback)
            {
                timerCore.StartTimer(aTimerValue, aContext, aOnUpdateFrame, aNewcallback);
            }

            public void StartCoreTimer(float aTimerValue, bool aCannotbePaused, object aContext, Action<EG_Timer> aNewcallback)
            {
                timerCore.StartTimer(aTimerValue, aCannotbePaused, aContext, aNewcallback);
            }
            
            public void StartCoreTimer(float aTimerValue, bool aCannotbePaused, object aContext, Action<EG_Timer> aOnUpdateFrame, Action<EG_Timer> aNewcallback)
            {
                timerCore.StartTimer(aTimerValue, aCannotbePaused, aContext, aOnUpdateFrame, aNewcallback);
            }

            public uint StartCoreTimerId(float aTimerValue, object aContext, Action<EG_Timer> aNewcallback)
            {
                return timerCore.StartTimerId(aTimerValue, aContext, aNewcallback);
            }
            
            public uint StartCoreTimerId(float aTimerValue, object aContext, Action<EG_Timer> aOnUpdateFrame, Action<EG_Timer> aNewcallback)
            {
                return timerCore.StartTimerId(aTimerValue, aContext, aOnUpdateFrame, aNewcallback);
            }

            public uint StartCoreTimerId(float aTimerValue, bool aCannotbePaused, object aContext,
                Action<EG_Timer> aNewcallback)
            {
                return timerCore.StartTimerId(aTimerValue, aCannotbePaused, aContext, aNewcallback);
            }
            
            public uint StartCoreTimerId(float aTimerValue, bool aCannotbePaused, object aContext,
                Action<EG_Timer> aOnUpdateFrame, Action<EG_Timer> aNewcallback)
            {
                return timerCore.StartTimerId(aTimerValue, aCannotbePaused, aContext, aOnUpdateFrame, aNewcallback);
            }

            #endregion
            

        }
        

    } //end namespace
}