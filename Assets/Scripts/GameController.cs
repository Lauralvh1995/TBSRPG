using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class GameController : MonoBehaviour
{
    public Unit unit1;
    public Unit unit2;

    public Transform AllyPrefab;
    public Transform EnemyPrefab;

    public Transform SelectorPrefab;

    public LineRenderer lineRenderer;

    Grid grid;
    MouseController mouseController;

    List<Unit> units;
    Unit currentUnit;

    int initiativeCount = 0;

    void Awake()
    {
        grid = GetComponent<Grid>();
        mouseController = GetComponent<MouseController>();
        units = new List<Unit>();

        Setup();
    }

    void Setup()
    {
        unit1 = Instantiate(AllyPrefab, new Vector3(-2.5f, 0, -2.5f), Quaternion.Euler(0, 0, 0)).GetComponent<AllyUnit>();
        unit2 = Instantiate(EnemyPrefab, new Vector3(2.5f, 0, 2.5f), Quaternion.Euler(0, 0, 0)).GetComponent<EnemyUnit>();

        units.Add(unit1);
        units.Add(unit2);

        units.Sort();

        currentUnit = units[0];
        mouseController.SetUnit(currentUnit);
        mouseController.selectionCube = Instantiate(SelectorPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        mouseController.lineRenderer = Instantiate(lineRenderer);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            bool legalMove = mouseController.MouseDown();
            if(legalMove)
            {
                if (initiativeCount % 2 != 0)
                {
                    initiativeCount++;
                    currentUnit = units[0];
                }
                else if(initiativeCount%2 == 0)
                {
                    initiativeCount++;
                    currentUnit = units[1];
                }
                else
                {

                }
            }
            mouseController.SetUnit(currentUnit);
        }
    }

}

