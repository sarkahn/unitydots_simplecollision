using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

[SerializeField]
public struct Drift : IComponentData
{
    public float angle;
    public float speed;
}

[SerializeField]
public class DriftComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public float speed_ = 5;

    [Range(0,360f)]
    public float angle_ = 0;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Drift { speed = speed_, angle = angle_ });
    }

    private void OnDrawGizmos()
    {
        var t = transform;
        var fwd = t.up;
        var p = transform.position;
        p.z -= 3;
        var dir = ((p + fwd) - p).normalized;

        dir = (Quaternion.Euler(new Vector3(0, 0, angle_)) * dir).normalized;

        var p2 = p + dir;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(p, p2);
    }
}

