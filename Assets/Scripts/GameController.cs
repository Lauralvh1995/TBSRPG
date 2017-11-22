using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Mode
{
    Walk,
    Attack,
    Skill1,
    Skill2,
    Skill3
}

class GameController : MonoBehaviour
{
    Unit unit1;
    Unit unit2;
    Unit unit3;
    Unit unit4;

    Weapon club;
    Weapon shotgun;
    Weapon pistol;
    Weapon knife;

    public Transform AllyPrefab;
    public Transform EnemyPrefab;

    public Transform SelectorPrefab;
    public Transform AISelectorPrefab;

    public GameObject victory;
    public GameObject defeat;
    public GameObject restart;
    public GameObject quit;

    public LineRenderer lineRenderer;

    public static GameController instance = null;

    NodeGrid grid;
    MouseController mouseController;
    AIController aiController;

    public List<Unit> units;
    Unit currentUnit;

    //Coroutine routine;
    [HideInInspector]
    public bool wait;
    [HideInInspector]
    public bool aboveUI;

    Mode currentMode;
    

    int initiativeCount = 0;
    private void Start()
    {
        victory.SetActive(false);
        defeat.SetActive(false);
        restart.SetActive(false);
        quit.SetActive(false);
    }


    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != null)
        {
            Destroy(gameObject);
        }
        grid = GetComponent<NodeGrid>();
        mouseController = GetComponent<MouseController>();
        aiController = GetComponent<AIController>();
        units = new List<Unit>();

        Setup();
    }

    public void SetMode(Mode mode)
    {
        currentMode = mode;
        Debug.Log("Current Mode: " + currentMode.ToString());
    }

    void Setup()
    {
        unit1 = Instantiate(AllyPrefab, new Vector3(-2.5f, 0, -2.5f), Quaternion.Euler(0, 0, 0)).GetComponent<Unit>();
        unit2 = Instantiate(EnemyPrefab, new Vector3(-1.5f, 0, -2.5f), Quaternion.Euler(0, 0, 0)).GetComponent<Unit>();
        unit3 = Instantiate(EnemyPrefab, new Vector3(2.5f, 0, 2.5f), Quaternion.Euler(0, 0, 0)).GetComponent<Unit>();
        unit4 = Instantiate(AllyPrefab, new Vector3(-2.5f, 0, -0.5f), Quaternion.Euler(0, 0, 0)).GetComponent<Unit>();

        unit1.SetTarget(unit1.transform);
        unit2.SetTarget(unit2.transform);
        unit3.SetTarget(unit3.transform);
        unit4.SetTarget(unit4.transform);

        club = new Weapon(1, 3, 70);
        shotgun = new Weapon(3, 3, 70);
        pistol = new Weapon(3, 1, 100);
        knife = new Weapon(1, 1, 100);

        unit1.equipped = club;
        unit2.equipped = knife;
        unit3.equipped = pistol;
        unit4.equipped = shotgun;

        units.Add(unit1);
        units.Add(unit2);
        units.Add(unit3);
        units.Add(unit4);

        units.Sort();

        currentUnit = units[0];

        if (currentUnit.ally)
        {
            mouseController.SetUnit(currentUnit);
        }
        else
        {
            aiController.SetUnit(currentUnit);
        }

        mouseController.selectionCube = Instantiate(SelectorPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        aiController.selectionCube = Instantiate(AISelectorPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

        mouseController.lineRenderer = Instantiate(lineRenderer);
        mouseController.lineRenderer.enabled = false;
        aiController.lineRenderer = Instantiate(lineRenderer);
        aiController.lineRenderer.enabled = false;

        if(units[0].ally)
        {
            grid.CheckPassability(true);
        }
        else
        {
            grid.CheckPassability(false);
        }
    }

    private void Update()
    {
        //Update is van zichzelf een loop. Dus hier hoeft geen while(iets) omheen.
        //Begin met kijken wat voor soort unit aan de beurt is.
        if (currentUnit.ally && wait == false)
        {
            if (Input.GetMouseButtonDown(0) && !aboveUI)
            {
                switch (currentMode)
                {
                    case Mode.Walk:
                        mouseController.MouseDown();
                        break;
                    case Mode.Attack:
                        Done(mouseController.Attack());
                        break;
                }
            }
        }
        else if(!currentUnit.ally && wait == false)
        {
            if (currentUnit != null)
            {
                aiController.GetPath();
            }
        }
        //In de coroutines zelf wordt de wait boolean op true gezet.
        //Als er moet worden gewacht, wat zo is als het commando om te lopen wordt gegeven, start met wachten.
        if(wait)
        {
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        //Doe niks, maar laat het programma wel lopen, zolang er moet worden gewacht
        while (wait)
        {
            yield return null;
        }
    }
    void NextTurn()
    {
        initiativeCount++;
        Debug.Log(initiativeCount);
        if(initiativeCount >= units.Count)
        {
            initiativeCount = 0;
        }
        currentUnit = units[initiativeCount];
        if(currentUnit.ally)
        {
            CheckVictory(true);
            mouseController.SetUnit(currentUnit);
            //aiController.SetUnit(null);
            grid.CheckPassability(true);
        }
        else if(!currentUnit.ally)
        {
            CheckVictory(false);
            aiController.SetUnit(currentUnit);
            //mouseController.SetUnit(null);
            grid.CheckPassability(false);
        }
        Debug.Log(initiativeCount + " " + (currentUnit.ally));
    }

    public bool Done(bool done)
    {
        if(done)
        {
            NextTurn();
        }
        else
        {
            wait = false;
        }
        return done;
    }

    public void PurgeList()
    {
        foreach(Unit unit in units.ToList())
        {
            if(unit.hp <=0)
            {
                units.Remove(unit);
            }
        }
    }

    void CheckVictory(bool ally)
    {
        List<Unit> checkList = new List<Unit>();
        foreach(Unit unit in units)
        {
            if(unit.ally == ally)
            {
                checkList.Add(unit);
            }
        }
        if(units.Count == checkList.Count)
        {
            foreach (Unit unit in units)
            {
                if (unit.ally)
                {
                    PlayerWins();
                    return;
                }
                else
                {
                    PlayerLoses();
                    return;
                }
            }
        }
    }

    public void PlayerWins()
    {
        victory.SetActive(true);
        restart.SetActive(true);
        quit.SetActive(true);
    }

    public void PlayerLoses()
    {
        defeat.SetActive(true);
        restart.SetActive(true);
        quit.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.GetActiveScene();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

