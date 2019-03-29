using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetFood : MonoBehaviour {

    private GameObject[] foods;
    private readonly float speed = 0.25f;
    private readonly string meat = "Meat";
    private readonly string plant = "Plant";
    private float YPos;
    private float YRot;
    Animator anim;

	// Use this for initialization
	void Start () {
        foods = new GameObject[] { };
        anim = GetComponent<Animator>();
        YPos = transform.position.y;
        YRot = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
        foods = GameObject.FindGameObjectsWithTag(meat);
        if(foods.Length > 0)
        {
            int objectNum = FindClosest();
            //Debug.Log(foods[objectNum].name);
            //if (Vector3.Distance(foods[objectNum].transform.position, transform.position) < 0.07)
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
        Quaternion targetRot = Quaternion.LookRotation(foods[num].transform.position - transform.position);
        targetRot.x = 0f;
        targetRot.z = 0f;
        float str = Mathf.Min(5f * Time.deltaTime, 1.0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, str);

        Vector3 dir = foods[num].transform.position - transform.position;
        dir = new Vector3(dir.x, 0f, dir.z);
        if (Vector3.Distance(foods[num].transform.position, transform.position) < 0.07)
        {
            anim.SetBool("moving", false);
            anim.SetTrigger("eat");
        }
        else
        {
            anim.SetBool("moving", true);
            dir = dir.normalized;
        }
        //anim.speed = speed * 50 * Time.deltaTime;
        
        transform.position += dir * speed * Time.deltaTime;
        
        Debug.Log("moving" + transform.position);
        //dino.transform.position = Vector3.MoveTowards(dino.transform.position, foods[num].transform.position, 1.0f * Time.deltaTime);
        if (Vector3.Distance(foods[num].transform.position, transform.position) < 0.04)
        {
            Destroy(foods[num]);
            
        }
    }
}
