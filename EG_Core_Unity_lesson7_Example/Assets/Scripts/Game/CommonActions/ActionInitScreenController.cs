
namespace EG
{
    namespace Core.BootLoader
    {
        [System.Serializable]
        public class ActionInitScreenController : EG_BootAction
        {

            public ActionInitScreenController(bool aValue, int anArrayAmount)  : base (aValue, anArrayAmount){ }
            
            
            protected override void DoStart()
            {
                //for example here I'm preparing the UI screens to feed them to the core
                // I'm just getting the screens controller which is a monobehaviour cause it's a visual object
                //
                //EG_ScreensController sm = GameObject.FindObjectOfType<EG_ScreensController>();
            
                // then I pass it to the core
                // so you can get screens from your game at any point
                // thanks to the core facade
                //
                //EG_Core.Self().SetScreensController(sm);
                
                //
                //and call the init with some parameters, to know if the screens will be controlled by a gamepad or a mouse
                //
                //EG_Core.Self().InitScreensController(EG_Core.Self().GetGamepad(), EG_Core.Self().GetUseMouse);
            
                EG_Core.Self().StartTimer(0.1f, this, cacheAction => { (cacheAction.Context as ActionInitScreenController).Wait(); });
            }
            
            
            private void Wait()
            {
                DoOnComplete();
            }
        }
    }
}