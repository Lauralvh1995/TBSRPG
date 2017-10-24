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
        unit2 = Instantiate(EnemyPrefab, new Vector3(2.5f, 0, 2.5f), Quaternion.Euler(0, 0, 0)).GetComponent<EnemyUnit>();

        units.Add(unit1);
        units.Add(unit2);

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

                if (initiativeCount % 2 != 0)
                {
                    initiativeCount++;
                    currentUnit = units[0];
                }
                else if (initiativeCount % 2 == 0)
                {
                    initiativeCount++;
                    currentUnit = units[1];
                }
            }
            mouseController.SetUnit(currentUnit);
        }
        else
        {
            aiController.SetUnit(currentUnit);
            aiController.GetPath(aiController.Move());
            if (initiativeCount % 2 != 0)
            {
                initiativeCount++;
                currentUnit = units[0];
            }
            else if (initiativeCount % 2 == 0)
            {
                initiativeCount++;
                currentUnit = units[1];
            }
            else
            {

            }
        }
    }
}

