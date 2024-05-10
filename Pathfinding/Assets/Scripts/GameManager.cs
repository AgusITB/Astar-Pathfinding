using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject token1, token2, token3;
    private int[,] GameMatrix; //0 not chosen, 1 player, 2 enemy
    private readonly int[] startPos = new int[2];
    private readonly int[] objectivePos = new int[2];

    [SerializeField] private bool isDebug;
    [SerializeField] private GameObject mePrefab;

    [SerializeField] private Node[,] allNodes;
    public List<Node> path;
    private bool enableDiagonals;


    private Pathfinding pfd;
    private void Awake()
    {
        pfd = GetComponent<Pathfinding>();
    }

    public void StartPathFinding(Toggle toggle)
    {
        enableDiagonals = toggle.isOn;

        GameMatrix = new int[Calculator.length, Calculator.length];

        for (int i = 0; i < Calculator.length; i++) //fila
            for (int j = 0; j < Calculator.length; j++) //columna
                GameMatrix[i, j] = 0;

        //randomitzar pos final i inicial;
        var rand1 = Random.Range(0, Calculator.length);
        var rand2 = Random.Range(0, Calculator.length);
        startPos[0] = rand1;
        startPos[1] = rand2;
        SetObjectivePoint(startPos);

        GameMatrix[startPos[0], startPos[1]] = 1;
        GameMatrix[objectivePos[0], objectivePos[1]] = 2;

        token1 = InstantiateToken(token1, startPos);
        token2 = InstantiateToken(token2, objectivePos);
        ShowMatrix();
        CreateNodes();
        pfd.StartMe(this);
        pfd.FindPath(startPos, objectivePos);

    }

    private void CreateNodes()
    {
        allNodes = new Node[GameMatrix.GetLength(0), GameMatrix.GetLength(1)];

        for (int i = 0; i < GameMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < GameMatrix.GetLength(1); j++)
            {
                allNodes[i, j] = (new(i, j));
            }

        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }
    public List<Node> GetNeighbours(Node node)
    {
        if (enableDiagonals)
        {
            List<Node> neighbours = new();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    int checkX = node.m_position[0] + x;
                    int checkY = node.m_position[1] + y;

                    if (checkX >= 0 && checkX < GameMatrix.GetLength(0) && checkY >= 0 && checkY < GameMatrix.GetLength(1))
                    {
                        neighbours.Add(allNodes[checkX, checkY]);
                    }
                }
            }
            return neighbours;
        }
        else
        {
            List<Node> neighbours = new();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {

                    if (System.Math.Abs(x) != System.Math.Abs(y))
                    {
                        if (x == 0 && y == 0) // Skip the center element
                        {
                            continue;
                        }

                        int checkX = node.m_position[0] + x;
                        int checkY = node.m_position[1] + y;

                        if (checkX >= 0 && checkX < GameMatrix.GetLength(0) && checkY >= 0 && checkY < GameMatrix.GetLength(1))
                        {
                            neighbours.Add(allNodes[checkX, checkY]);
                        }
                    }

                }
            }
            return neighbours;

        }

    }

    public void InstantiateMe()
    {
        if (isDebug) return;
        Destroy(token1);
        Destroy(token2);
        Instantiate(mePrefab, Calculator.GetPositionFromMatrix(objectivePos), Quaternion.identity);

    }

    private GameObject InstantiateToken(GameObject token, int[] position)
    {
        return Instantiate(token, Calculator.GetPositionFromMatrix(position), Quaternion.identity);
    }
    private void SetObjectivePoint(int[] startPos)
    {
        var rand1 = Random.Range(0, Calculator.length);
        var rand2 = Random.Range(0, Calculator.length);
        if (rand1 != startPos[0] || rand2 != startPos[1])
        {
            objectivePos[0] = rand1;
            objectivePos[1] = rand2;
        }
    }

    private void ShowMatrix()
    {
        string matrix = "";
        for (int i = 0; i < Calculator.length; i++)
        {
            for (int j = 0; j < Calculator.length; j++)
            {
                matrix += GameMatrix[i, j] + " ";
            }
            matrix += "\n";
        }
    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        GUIStyle style = new()
        {
            fontStyle = FontStyle.Bold
        };

        if (allNodes != null)
        {
            foreach (Node n in allNodes)
            {
                style.fontSize = 15;
                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = Color.red;
                if (path != null)
                    if (path.Contains(n))
                    {
                        style.normal.textColor = Color.blue;
                        if(n.Equals(new Node(objectivePos[0], objectivePos[1])))
                        {
                            style.fontSize = 25;
                            style.normal.textColor = Color.yellow;
                        }
                    } 
                    else if (n.Equals(new Node(startPos[0], startPos[1])))
                    {
                        style.fontSize = 25;
                        style.normal.textColor = Color.yellow;
                    }

                Vector3 pos = Calculator.GetPositionFromMatrix(n.m_position);
                Handles.Label(pos, $"{n.StartEndCost}", style);

                style.normal.textColor = Color.black;
                pos.y -= 0.25F;
                Handles.Label(pos, $"[{n.distanceFromStart},{n.distanceFromTheEndNode}]", style);

                pos.y += 0.5f;
                Handles.Label(pos, $"[{n.m_position[0]},{n.m_position[1]}]", style);

            }
        }
    }
#endif
}
