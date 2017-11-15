using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour {

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            GameController.instance.aboveUI = true;
        }
        else
        {
            GameController.instance.aboveUI = false;
        }
    }

    public void SetWalk()
    {
        GameController.instance.SetMode(Mode.Walk);
    }
    public void SetAttack()
    {
        GameController.instance.SetMode(Mode.Attack);
    }
    public void SetSkill1()
    {
        GameController.instance.SetMode(Mode.Skill1);
    }
    public void SetSkill2()
    {
        GameController.instance.SetMode(Mode.Skill2);
    }
    public void SetSkill3()
    {
        GameController.instance.SetMode(Mode.Skill3);
    }

}
