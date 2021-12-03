using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level2Controller : MonoBehaviour
{

    public static Level2Controller instance;

    public GameObject pile1;
    public GameObject pile2;
    public GameObject pile3;

    public List<GameObject> piles = new List<GameObject>();
    public GameObject container;

    //integer to check how much sand has been deposited into the container
    private int sandDeposited = 0;



    private void Awake()
    {
        instance = this;

    }


    private void Start()
    {

        piles.Add(pile1);
        piles.Add(pile2);
        piles.Add(pile3);
    }

    public void FillCointainer()
    {
        piles[sandDeposited].SetActive(true);
        sandDeposited++;

        if (sandDeposited == 3)
        {
            container.transform.position += new Vector3(0, -5f, 0);
        }

    }



}
