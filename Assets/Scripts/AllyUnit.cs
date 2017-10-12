using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AllyUnit : Unit
{
    public int threatValue;

    List<Weapon> weaponList;
    private void Awake()
    {
        weaponList = new List<Weapon>();
    }
}

