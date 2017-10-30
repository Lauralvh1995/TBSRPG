using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

class GameController : MonoBehaviour
{
    Unit unit1;
    Unit unit2;
    Unit unit3;
    Unit unit4;


    public Transform AllyPrefab;
    public Transform EnemyPrefab;

    public Transform SelectorPrefab;
    public Transform AISelectorPrefab;

    public LineRenderer lineRenderer;

    public static GameController instance = null;

    Grid grid;
    MouseController mouseController;
    AIController aiController;

    List<Unit> units;
    Unit currentUnit;

    //Coroutine routine;
    [HideInInspector]
    public bool wait;
    

    int initiativeCount = 0;

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
        grid = GetComponent<Grid>();
        mouseController = GetComponent<MouseController>();
        aiController = GetComponent<AIController>();
        units = new List<Unit>();

        Setup();
    }

    void Setup()
    {
        unit1 = Instantiate(AllyPrefab, new Vector3(-2.5f, 0, -2.5f), Quaternion.Euler(0, 0, 0)).GetComponent<AllyUnit>();
        unit2 = Instantiate(EnemyPrefab, new Vector3(-1.5f, 0, -2.5f), Quaternion.Euler(0, 0, 0)).GetComponent<EnemyUnit>();
        unit3 = Instantiate(EnemyPrefab, new Vector3(2.5f, 0, 2.5f), Quaternion.Euler(0, 0, 0)).GetComponent<EnemyUnit>();
        unit4 = Instantiate(AllyPrefab, new Vector3(-2.5f, 0, -0.5f), Quaternion.Euler(0, 0, 0)).GetComponent<AllyUnit>();

        unit1.SetTarget(unit1.transform);
        unit2.SetTarget(unit2.transform);
        unit3.SetTarget(unit3.transform);
        unit4.SetTarget(unit4.transform);

        units.Add(unit1);
        units.Add(unit2);
        units.Add(unit3);
        units.Add(unit4);

        units.Sort();

        currentUnit = units[0];

        if (currentUnit is AllyUnit)
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
        aiController.lineRenderer = Instantiate(lineRenderer);
        mouseController.lineRenderer.enabled = false;
        aiController.lineRenderer.enabled = false;
        if(units[0] is AllyUnit)
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
        if (currentUnit is AllyUnit && wait == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseController.MouseDown();
            }
        }
        else if(currentUnit is EnemyUnit && wait == false)
        {
            aiController.GetPath();
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
        if(currentUnit is AllyUnit)
        {
            mouseController.SetUnit(currentUnit);
            //aiController.SetUnit(null);
            grid.CheckPassability(true);
        }
        else if(currentUnit is EnemyUnit)
        {
            aiController.SetUnit(currentUnit);
            //mouseController.SetUnit(null);
            grid.CheckPassability(false);
        }
        Debug.Log(initiativeCount + " " + (currentUnit is AllyUnit));
    }

    public bool Done(bool done)
    {
        if(done)
        {
            NextTurn();
        }
        return done;
    }
}

