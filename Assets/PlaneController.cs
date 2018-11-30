using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour {

 //   public Controller controller;
 //   public float width;
 //   public float height;
 //   public float length;
 //   public GameObject room;
 //   // Use this for initialization
 //   void Start () {
 //       //gameObject
 //   }
	
	//// Update is called once per frame
	//void Update () {
    //    MobilePick();
    //    MousePick();
    //}
    ////void OnClick()
    ////{
    ////    gameObject.transform.localScale = new Vector3 (gameObject.transform.localScale.x * 2f, gameObject.transform.localScale.y * 2f, gameObject.transform.localScale.z * 2f);
    ////}
    //void MobilePick()
    //{
    //    if (Input.touchCount != 1)
    //        return;
    //    Debug.Log(Input.GetTouch(0).phase);
    //    if (Input.GetTouch(0).phase == TouchPhase.Began)
    //    {
    //        RaycastHit hit;
    //        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

    //        if (Physics.Raycast(ray, out hit))
    //        {
    //            //Debug.Log(hit.transform.name);
    //            hit.transform.gameObject.tag = "selected";
    //            hit.transform.parent = room.transform;
    //            CreateCube(hit.transform.gameObject);
    //            GameObject[] obj = FindObjectsOfType(typeof(GameObject)) as GameObject[]; //关键代码，获取所有gameobject元素给数组obj
    //            foreach (GameObject child in obj)    //遍历所有gameobject
    //            {
    //                if (child.gameObject.tag == "planePrefebs" && child.gameObject.name == "debugPlanePrefab")    //进行操作
    //                {
    //                    Debug.Log("del");
    //                    Debug.Log(child.gameObject.name);  //可以在unity控制台测试一下是否成功获取所有元素

    //                    Debug.Log(child.gameObject.tag);  //可以在unity控制台测试一下是否成功获取所有元素

    //                    child.gameObject.SetActive(false);
    //                    //Destroy(child.gameObject);
    //                }
    //                else{
    //                    Debug.Log("notdel");

    //                    Debug.Log(child.gameObject.name);  //可以在unity控制台测试一下是否成功获取所有元素

    //                    Debug.Log(child.gameObject.tag);

    //                }
    //            }
    //            //controller.InitializePlanePrefab(hit.transform.gameObject);
    //            //hit.transform.localScale = new Vector3(width, 1f, length);
    //            //hit.transform.parent = room;
    //            gameObject.SetActive(false);
    //            //controller.AddTransforms(room.transform);
    //            //Debug.Log(hit.transform.tag);
    //        }
    //    }
    //}

    //void CreateCube(GameObject prefabObj)
    //{
    //    GameObject up = GameObject.Instantiate(prefabObj, new Vector3(prefabObj.transform.position.x, prefabObj.transform.position.y+height, prefabObj.transform.position.z), Quaternion.Euler(0, 0, 0)) as GameObject;
    //    up.transform.parent = room.transform;

    //    GameObject left = GameObject.Instantiate(prefabObj, new Vector3(prefabObj.transform.position.x-width/2, prefabObj.transform.position.y+ height/2, prefabObj.transform.position.z), Quaternion.Euler(0,0,90)) as GameObject;
    //    left.transform.parent = room.transform;

    //    GameObject right = GameObject.Instantiate(prefabObj, new Vector3(prefabObj.transform.position.x+width/2, prefabObj.transform.position.y + height / 2, prefabObj.transform.position.z), Quaternion.Euler(0, 0, 90)) as GameObject;
    //    right.transform.parent = room.transform;

    //    GameObject forward = GameObject.Instantiate(prefabObj, new Vector3(prefabObj.transform.position.x, prefabObj.transform.position.y + height / 2, prefabObj.transform.position.z-length/2), Quaternion.Euler(90, 0, 0)) as GameObject;
    //    forward.transform.parent = room.transform;

    //    GameObject back = GameObject.Instantiate(prefabObj, new Vector3(prefabObj.transform.position.x, prefabObj.transform.position.y + height / 2, prefabObj.transform.position.z+length/2), Quaternion.Euler(90, 0, 0)) as GameObject;
    //    back.transform.parent = room.transform;

    //}

    //void MousePick()
    //{
    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit hit;

    //        if (Physics.Raycast(ray, out hit))
    //        {
    //            Debug.Log(hit.transform.name);
    //            //Debug.Log(hit.transform.tag);
    //        }
    //    }
    //}

}
