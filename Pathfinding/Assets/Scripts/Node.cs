using System;


[Serializable]public class Node
{
    public int[] m_position = new int[2];

    public int distanceFromStart; // Distáncia respecto al punto inicial
    public int distanceFromTheEndNode; // Distáncia respecto al punto objetivo

    public int StartEndCost // Suma de distancia respecto al punto inicial y distáncia respecto al punto objetivo
    {
        get
        {
            return distanceFromStart + distanceFromTheEndNode;
        }
    }


    public Node parent; // Desde el nodo por el cual ha llegado hasta este nodo


    public bool Equals(Node node)
    {
        return node.m_position[0] == m_position[0] && node.m_position[1] == m_position[1];
    }
    public Node(int positionX, int positionY)
    {
        m_position[0] = positionX;
        m_position[1] = positionY;
    }


}



