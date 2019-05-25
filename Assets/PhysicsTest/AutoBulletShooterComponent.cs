using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct AutoBulletShooter : IComponentData
{
    public float shotTimer;
    public float cooldown;
    public float angle;
}

public class AutoBulletShooterComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public float cooldown_ = .25f;
    [Range(0f,360f)]
    public float angle_;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new AutoBulletShooter { cooldown = cooldown_ });
    }

    

    private void OnDrawGizmos()
    {
        //var t = transform;
        //var fwd = t.up;
        //var p = transform.position;
        //p.z -= 3;
        //var dir = ((p + fwd) - p).normalized;

        //dir = (Quaternion.Euler(new Vector3(0, 0,  angle_)) * dir).normalized;

        //var p2 = p + dir;

        float3 p = transform.position;
        float3 fwd = transform.up;

        var angledDir = Utils.GetAngleDir(p, fwd, angle_);

        Vector3 p2 = p + angledDir;

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(p, p2);
    }
}
