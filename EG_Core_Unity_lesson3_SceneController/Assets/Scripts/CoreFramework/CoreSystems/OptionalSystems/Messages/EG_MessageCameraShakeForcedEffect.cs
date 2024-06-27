namespace EG
{
    namespace Core.Messages
    {

        /// <summary>
        /// An example of how we can shake the camera, in some point of our game we will send the message
        /// our camera controller will be listening this message and do the proper shake without further
        /// interaction in the origin point (for example when the character receives damage)
        /// </summary>
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

            public void SetData(float aDuration, float anAmount, float aDecrease, bool aIsWorldPosition, uint aSenderId)
            {
                IsLocal = true;
                SenderId = aSenderId;
                ShakeAmount = anAmount;
                ShakeDuration = aDuration;
                DecreaseShakeAmount = aDecrease;
                IsWorldPosition = aIsWorldPosition;
            }

        }

    }
}