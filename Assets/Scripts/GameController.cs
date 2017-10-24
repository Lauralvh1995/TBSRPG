using System;
using System.Collections.Generic;
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

    Grid grid;
    MouseController mouseController;
    AIController aiController;

    List<Unit> units;
    Unit currentUnit;

    int initiativeCount = 0;

    void Awake()
    {
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

        units.Add(unit1);
        units.Add(unit2);
        units.Add(unit3);
        units.Add(unit4);

        units.Sort();

        currentUnit = units[0];
        mouseController.SetUnit(currentUnit);
        mouseController.selectionCube = Instantiate(SelectorPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        aiController.selectionCube = Instantiate(AISelectorPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        mouseController.lineRenderer = Instantiate(lineRenderer);
        aiController.lineRenderer = Instantiate(lineRenderer);
    }

    private void Update()
    {
        if (currentUnit is AllyUnit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseController.MouseDown();
                NextTurn();
            }
        }
        else
        {
            aiController.GetPath();
            NextTurn();
        }
    }

    void NextTurn()
    {
        initiativeCount++;
        if(initiativeCount > units.Count)
        {
            initiativeCount = 0;
        }
        currentUnit = units[initiativeCount];
        if(currentUnit is AllyUnit)
        {
            mouseController.SetUnit(currentUnit);
            aiController.SetUnit(null);
        }
        else
        {
            aiController.SetUnit(currentUnit);
            mouseController.SetUnit(null);
        }
    }
}

