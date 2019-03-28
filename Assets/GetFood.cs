using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetFood : MonoBehaviour {

    private GameObject[] foods;
    private readonly float speed = 1.0f;
    private readonly string meat = "Meat";
    private readonly string plant = "Plant";

	// Use this for initialization
	void Start () {
        foods = new GameObject[] { };
	}
	
	// Update is called once per frame
	void Update () {
        foods = GameObject.FindGameObjectsWithTag(meat);
        if(foods.Length > 0)
        {
            int objectNum = FindClosest();
            //Debug.Log(foods[objectNum].name);
            Move(objectNum);
        }
	}

    private int FindClosest()
    {
        double min = 99999999999.00000000000000000;
        int foodObject = -1;
        for(int i = 0; i < foods.Length; i++)
        {
            double a = (transform.position - foods[i].transform.position).sqrMagnitude;
            //Debug.Log(foods[i].name + " " +a);
            if (a < min)
            {
                min = a;
                foodObject = i;
                Debug.Log("changing min");
            }
            Debug.Log("end of loop");
        }

        return foodObject;
    }

    private void Move(int num)
    {
        Vector3 dir = foods[num].transform.position - transform.position;
        transform.position += dir * speed * Time.deltaTime;
        Debug.Log("moving" + transform.position);
        //dino.transform.position = Vector3.MoveTowards(dino.transform.position, foods[num].transform.position, 1.0f * Time.deltaTime);
    }
}
