using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CockBoardRequests : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public List<GameObject> requests = new List<GameObject>();
    public GameObject requestSpots;

    void Start()
    {
        
    }

    // Update is called once per frame
    void UpdateBoard()
    {
        for (int i = 0; i < requests.Count; i++)
        {
            Vector3 tempPos = requestSpots.transform.position;
            requests[i].transform.position = tempPos;
        }
    }
}
