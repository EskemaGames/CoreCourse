using UnityEngine;

public interface ITarget
{
    Transform IGetTransform { get; }
    Vector3 IGetCenterTransformPosition { get; }
    bool IsAlive { get; }
    uint IGetId { get; }
    GameEnums.GroupTypes IGetGroupType { get; }
}