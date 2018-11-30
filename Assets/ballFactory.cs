using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballFactory : MonoBehaviour {

    public GameObject ball;
    public float scale;
    public int step;

	// Use this for initialization
	void Start () {
        for (int x = -10; x < 10; x++)
        {
            for (int y = -10; y < 10; y++)
            {
                for (int z = -10; z < 10; z++)
                {

                    GameObject newObj = GameObject.Instantiate(ball);
                    newObj.transform.position = new Vector3(x, y, z);
                    newObj.transform.localScale = new Vector3(scale,scale,scale);
                    newObj.transform.parent = transform;
                    if (x > 0 && x < 5 && y > 0 && y < 5 && z > 0 && z < 5)
                    {
                        newObj.GetComponent<MeshRenderer>().materials[0].color = Color.blue;
                    }
                    else{
                        newObj.GetComponent<MeshRenderer>().materials[0].color = new Color(Random.Range(0.0f, 1.0f),Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

                    }
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
