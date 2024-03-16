public interface IUpdateTimedSystems
{
    void IOnUpdate(float aDeltaTime = 1f);
    void IOnFixedUpdate(float aDeltaTime = 1f);
    void IOnLateUpdate(float aDeltaTime = 1f);
}