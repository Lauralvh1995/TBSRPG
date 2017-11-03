using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    int attackRange;
    int damage;
    int accuracy;


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
