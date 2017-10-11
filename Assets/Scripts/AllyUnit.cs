using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AllyUnit : Unit
{
    List<Weapon> weaponList;
    private void Awake()
    {
        weaponList = new List<Weapon>();
    }
}

