using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Weapon
{
    public int attackRange;
    public int damage;
    public int accuracy;

    public Weapon(int attRange, int dmg, int acc)
    {
        attackRange = attRange;
        damage = dmg;
        accuracy = acc;
    }
    public int GetAccuracy()
    {
        return accuracy;
    }

    public int GetRange()
    {
        return attackRange;
    }

    public int GetDamage()
    {
        return damage;
    }
}
