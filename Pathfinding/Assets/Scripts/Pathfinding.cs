using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Pathfinding : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private float timeBetweenStep;

    public void StartMe(GameManager manager)
    {
        gameManager = manager;
    }

    public void FindPath(int[] startPos, int[] targetPos)
    {
        Node startNode = new(startPos[0], startPos[1]);
        Node targetNode = new(targetPos[0], targetPos[1]);

        List<Node> openSet = new();
        HashSet<Node> closedSet = new();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].StartEndCost < currentNode.StartEndCost || openSet[i].StartEndCost == currentNode.StartEndCost)
                {
                    if (openSet[i].distanceFromTheEndNode < currentNode.distanceFromTheEndNode)
                    {
                        currentNode = openSet[i];
                    }

                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode.Equals(targetNode))
            {
                StartCoroutine(RetracePath(startNode, currentNode));
                return;
            }


            //Debug.Log($"        Current: [{currentNode.m_position[0]},{currentNode.m_position[1]}]");


            foreach (Node neighbour in gameManager.GetNeighbours(currentNode))
            {
                // Debug.Log($"Neighbour: [{neighbour.m_position[0]},{neighbour.m_position[1]}]");
                if (closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour;


                if (!gameManager.enableDiagonals)
                {
                    newCostToNeighbour = Convert.ToInt32(currentNode.distanceFromStart + Calculator.CheckDistanceToObj(currentNode, neighbour));
                }
                else
                {
                    newCostToNeighbour = currentNode.distanceFromStart + GetDistance(currentNode, neighbour);
                }


                if (newCostToNeighbour < neighbour.distanceFromStart || !openSet.Contains(neighbour))
                {

                    neighbour.distanceFromStart = newCostToNeighbour;

                    if (!gameManager.enableDiagonals)
                    {
                        neighbour.distanceFromTheEndNode = Convert.ToInt32(Calculator.CheckDistanceToObj(currentNode, neighbour));
                    }
                    else
                    {
                        neighbour.distanceFromTheEndNode = GetDistance(neighbour, targetNode);
                    }

                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }

                }
            }        
        }
    }

    private IEnumerator RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new();

        Node currentNode = endNode;

        while (!currentNode.Equals(startNode))
        {

            path.Add(currentNode);
            gameManager.path.Add(currentNode);
            currentNode = currentNode.parent;

        }
        path.Add(currentNode);
        path.Reverse();
 

        foreach (Node nextNode in path)
        {
            yield return new WaitForSeconds(timeBetweenStep);


            gameManager.token1.transform.position = Calculator.GetPositionFromMatrix(nextNode.m_position);
            if (nextNode.Equals(endNode))
            {
                gameManager.InstantiateMe();
            }

        }
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        //LA DISTANCIA HORIZONTAL ENTRE UN NODO Y OTRO ES DE 1unidad
        //LA DISTANCIA DIAGONAL ENTRE UN NODO Y OTRO SERÍA √2 = 1,4...
        //Así que multiplicamos por 10 las dos y nos queda 10h 14diagonal

        int distX = Mathf.Abs(nodeA.m_position[0] - nodeB.m_position[0]);
        int distY = Mathf.Abs(nodeA.m_position[1] - nodeB.m_position[1]);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);


        return 14 * distX + 10 * (distY - distX);


    }

}

