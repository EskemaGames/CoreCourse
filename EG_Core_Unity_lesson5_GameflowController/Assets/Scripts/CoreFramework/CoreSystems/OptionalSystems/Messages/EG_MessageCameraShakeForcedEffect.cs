

namespace EG
{
    namespace Core.Messages
    {

        //
        // an example message that I use to send information to do a shake camera effect
        //
        
        [System.Serializable]
        public class EG_MessageCameraShakeForcedEffect : EG_Message
        {

            public float ShakeDuration { get; private set; }
            public float ShakeAmount { get; private set; }
            public float DecreaseShakeAmount { get; private set; }
            public bool IsWorldPosition { get; private set; }

            //lazy constructor
            public EG_MessageCameraShakeForcedEffect()
            {
                IsLocal = true;
                SenderId = 0;
                ShakeAmount = 0f;
                ShakeDuration = 0f;
                DecreaseShakeAmount = 0;
            }

            public EG_MessageCameraShakeForcedEffect(
                uint senderId) : base(senderId)
            {
            }

            public void SetData(float aDuration, float anAmount, float aDecrease, bool isworldPosition, uint aSenderId)
            {
                IsLocal = true;
                SenderId = aSenderId;
                ShakeAmount = anAmount;
                ShakeDuration = aDuration;
                DecreaseShakeAmount = aDecrease;
                IsWorldPosition = isworldPosition;
            }

        }

    }
}