using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TestFixture]
public class CollisionFlagsTesting
{
    [Test]
    public void TestCollisionFlag()
    {
        CollisionFlags bullet = CollisionFlags.Bullet;

        Assert.IsTrue(Utils.Collides(bullet, CollisionFlags.Bullet));
        Assert.IsTrue(Utils.Collides(bullet, CollisionFlags.EnemyBullet));
        Assert.IsTrue(Utils.Collides(bullet, CollisionFlags.PlayerBullet));

        Assert.IsFalse(Utils.Collides(bullet, CollisionFlags.Player));
        Assert.IsFalse(Utils.Collides(bullet, CollisionFlags.Enemy));
        


        //Assert.IsFalse(Utils.Collides(enemyBullet, bullet));

        //Assert.IsFalse




    }
}
