using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum CollisionFlags
{
    Player = 1 << 0,
    Enemy = 1 << 1,
    Bullet = 1 << 2,
    PlayerBullet = Player | Bullet,
    EnemyBullet = Enemy | Bullet,
};
