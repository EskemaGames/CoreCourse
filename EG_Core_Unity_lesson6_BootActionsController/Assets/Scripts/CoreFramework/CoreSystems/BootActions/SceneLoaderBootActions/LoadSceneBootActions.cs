namespace EG
{
    namespace Core.BootLoader
    {
        public class LoadSceneBootActions : EG_BootActionsSystem
        {

            //just a mockup class to avoid using the original
            //to keep the original "protected" using this frontend
            public override void DoInit(string aFileName, System.Action onActionsDone = null)
            {
                base.DoInit(aFileName, onActionsDone);
            }
        }
    }
}