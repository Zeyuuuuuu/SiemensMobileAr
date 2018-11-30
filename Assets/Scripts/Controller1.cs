using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//场景头部引入


public class Controller1 : MonoBehaviour {

    public List<Transform> transforms;
    public int index;
    public float moveUnit;
    public float scaleUnit = 0.1f;
    private GameObject plane;
    public float width;
    public float height;
    public float length;
    bool planePlaced;
    public GameObject room;
    Transform[] children;
    public List<GameObject> testObjs;
    //public GameObject hitCubeParent;
    public GameObject RouterPrefeb;
    public GameObject detectPrefeb;
    public float maxRayDistance = 30.0f;
    public LayerMask collisionLayer = 1<<12; 
    
    public List<GameObject> Routers;
    GameObject detectPlane;
    GameObject chosenRouter;
    bool canPlace;
    public void changeCanPlace(){
        canPlace =  true;
        detectPlane.SetActive(true);
    }
    public void InitializePlanePrefab(GameObject go)
    {
        transforms.Add(go.transform);
        plane = go;
    }
    // Use this for initialization
    void Start () {
       
    }

    // Update is called once per frame
    void Update () {
        if (planePlaced)
            PlaceRouter();
        
        MobilePick();
       
    }

    public void ReStart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlaceRouter(){
        
            // if(!canPlace&&detectPlane!=null)
            //     detectPlane.SetActive(false);


            GameObject[] objs = GameObject.FindGameObjectsWithTag("planePrefebs") as GameObject[]; //关键代码，获取所有gameobject元素给数组obj
            if (objs.Length != 0)
            {
                foreach (GameObject obj in objs)    //遍历所有gameobject
                {
                    obj.gameObject.SetActive(false);
                    //Destroy(obj.gameObject);  
                }
            }
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2,0));
          
            // Debug.Log(Physics.Raycast(ray, out hit));
            // Debug.Log(hit.transform.gameObject.layer);

            if (Physics.Raycast(ray, out hit, maxRayDistance, collisionLayer)){
                // Debug.Log(hit.transform.rotation);
                    if (hit.transform.gameObject.layer == 12)
                    {
                    //Debug.Log(3);
                        if (detectPlane == null)
                        {
                            //Debug.Log(4);

                            detectPlane = Instantiate(detectPrefeb, hit.point, hit.transform.rotation) as GameObject;
                        }
                        else
                        {
                            //Debug.Log(5);

                            detectPlane.transform.position = hit.point;
                            Debug.Log(detectPlane.transform.position.ToString("f4"));
                            detectPlane.transform.rotation = hit.transform.rotation;
                        }
                    switch(hit.transform.gameObject.tag)
                    {
                         case "ground":
                            detectPlane.transform.Rotate(new Vector3(0,0,0));
                            break;
                        case "back":
                            detectPlane.transform.Rotate(new Vector3(-90,0,-180));
                            break;
                        case "forward":
                            detectPlane.transform.Rotate(new Vector3(-90, -180, -180));
                            break;
                        case "left":
                            detectPlane.transform.Rotate(new Vector3(-90, -90, 0));
                            break;
                        case "right":
                            detectPlane.transform.Rotate(new Vector3(-90, 90, 0));
                            break;
                        case "celling":
                            detectPlane.transform.Rotate(new Vector3(-180, 0, 0));
                            break;
                     

                    }
                    // Debug.Log(detectPlane.transform.rotation);
                    // detectPlane.transform.Rotate(new Vector3(90,0,-90));

                    //     if (Input.GetTouch(0).phase == TouchPhase.Began)
                    //     {
                    //         Debug.Log(6);

                    //         Routers.Add(Instantiate(RouterPrefeb, hit.point, detectPlane.transform.rotation) as GameObject);
                    //     }
                    // }
                    // else if (hit.transform.gameObject.layer == 11)
                    // {
                    //     Debug.Log(7);


                    // }
                }

        }
    }

    public void OnNext(){
        index = (index + 1) % (testObjs.Count);
        //planePlaced = true;
    }
    public void Up()
    {
        Debug.Log("upup");
        transforms[index].position = new Vector3(transforms[index].position.x, transforms[index].position.y + moveUnit, transforms[index].position.z);
    }
    public void Down()
    {
        transforms[index].position = new Vector3(transforms[index].position.x, transforms[index].position.y - moveUnit, transforms[index].position.z);
    }
    public void Left()
    {
        transforms[index].position = new Vector3(transforms[index].position.x, transforms[index].position.y, transforms[index].position.z + moveUnit);
    }
    public void Right()
    {
        transforms[index].position = new Vector3(transforms[index].position.x, transforms[index].position.y, transforms[index].position.z - +moveUnit);
    }
    public void Forward()
    {
        transforms[index].position = new Vector3(transforms[index].position.x + moveUnit, transforms[index].position.y, transforms[index].position.z);
    }
    public void Back()
    {
        transforms[index].position = new Vector3(transforms[index].position.x - moveUnit, transforms[index].position.y, transforms[index].position.z);
    }
    public void ScaleUp()
    {
        if(scaleUnit > 0.1f){
            if(transforms[index].gameObject.tag == "selected"){
                children = room.GetComponentsInChildren<Transform>();
                foreach(Transform child in children){
                    child.localScale = transforms[index].localScale / scaleUnit;
                }
            }
            else
            {
                transforms[index].localScale = transforms[index].localScale / scaleUnit;
            }
        }
    }
    public void ScaleDown()
    {
        if (scaleUnit > 0.1f)
        {
            if (transforms[index].gameObject.tag == "selected")
            {
                children = room.GetComponentsInChildren<Transform>();
                foreach (Transform child in children)
                {
                    child.localScale = transforms[index].localScale * scaleUnit;
                }
            }
            else
            {
                transforms[index].localScale = transforms[index].localScale * scaleUnit;
            }
        }
    }
    public void OnMoveUnitChange(float value)
    {
        moveUnit = value;
    }
    public void OnScaleUnitChange(float value)
    {
        scaleUnit = value;
    }
    GameObject roomcube;
    void MobilePick()
    {

        if (Input.touchCount != 1)
            return;
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            if (!planePlaced && Physics.Raycast(ray, out hit,maxRayDistance,1<<10))//未放置房间，鼠标点击
            {
                hit.transform.gameObject.tag = "selected";
                roomcube = Instantiate(testObjs[index], hit.transform.position, hit.transform.rotation) as GameObject;
                // roomcube.transform.Rotate(new Vector3(-90, 180, 0));
                planePlaced = true;
                transforms.Add(roomcube.transform);
                Debug.Log(transforms);
            }
            else if(planePlaced){//已放置房间
                if(chosenRouter == null){//未选定
                    if(Physics.Raycast(ray, out hit,maxRayDistance,1<<11)){//点击到11.模型，则选定
                        chosenRouter = hit.transform.gameObject;
                        chosenRouter.GetComponent<MeshRenderer>().material.color = Color.red;
                        detectPlane.SetActive(false);
                    }
                    else
                    // if(canPlace)//未点击到模型，放置
                    {
                        // Debug.Log(detectPlane.transform.position.ToString("f4"));
                        Routers.Add(Instantiate(RouterPrefeb, detectPlane.transform.position, detectPlane.transform.rotation) as GameObject);
                        // canPlace = false;
                        // chosenRouter = Routers[Routers.Count-1];
                        // chosenRouter.GetComponent<MeshRenderer>().material.color = Color.red;
                        // detectPlane.SetActive(false);
                    }
                    
                }
                else{//已选定router，点击到的不是已选定，就取消选定
                    if(!(Physics.Raycast(ray, out hit,maxRayDistance,1<<11)&&hit.transform.gameObject==chosenRouter)){
                        chosenRouter.GetComponent<MeshRenderer>().material.color = Color.black;
                        chosenRouter = null;
                        detectPlane.SetActive(true);
                    }
                }
            }
        }
        else if(Input.GetTouch(0).phase == TouchPhase.Moved&&chosenRouter !=null)//移动
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            if(Physics.Raycast(ray, out hit,maxRayDistance,1<<12)){//点击到12.墙体
                chosenRouter.transform.position = hit.point;
                Debug.Log(hit.point.ToString("f4"));
                chosenRouter.transform.rotation = hit.transform.rotation;
                switch(hit.transform.gameObject.tag)
                    {
                         case "ground":
                            chosenRouter.transform.Rotate(new Vector3(0,0,0));
                            break;
                        case "back":
                            chosenRouter.transform.Rotate(new Vector3(-90,0,-180));
                            break;
                        case "forward":
                            chosenRouter.transform.Rotate(new Vector3(-90, -180, -180));
                            break;
                        case "left":
                            chosenRouter.transform.Rotate(new Vector3(-90, -90, 0));
                            break;
                        case "right":
                            chosenRouter.transform.Rotate(new Vector3(-90, 90, 0));
                            break;
                        case "celling":
                            chosenRouter.transform.Rotate(new Vector3(-180, 0, 0));
                            break;
                     

                    }
            }
        }
        else if(Input.GetTouch(0).phase == TouchPhase.Ended&&chosenRouter !=null)
        {
            // chosenRouter.transform.position = Input.GetTouch(0).position;
        }
    }
    public void Rotate(){
        roomcube.transform.Rotate(new Vector3(0, 90*moveUnit, 0));
    }
    public void UpAndDown(){
        roomcube.transform.position = new Vector3(roomcube.transform.position.x, roomcube.transform.position.y - (0.5f - moveUnit), roomcube.transform.position.z);
    }
    //width 0.2
    //height 0.3
    //length 0.5
    void CreateCube(GameObject prefabObj)
    {
        prefabObj.transform.localScale = new Vector3(length * 0.1f, 0.1f * 0.1f, width * 0.1f);
        prefabObj.transform.position = new Vector3(prefabObj.transform.position.x-length/2f, prefabObj.transform.position.y, prefabObj.transform.position.z-width/2f);

        GameObject bottom = GameObject.Instantiate(prefabObj, new Vector3(prefabObj.transform.position.x, prefabObj.transform.position.y, prefabObj.transform.position.z), prefabObj.transform.rotation) as GameObject;
        bottom.name = "bottom";
        bottom.transform.Rotate(new Vector3(0, 0, 0));
        bottom.transform.parent = room.transform;
       

        GameObject up = GameObject.Instantiate(prefabObj, new Vector3(prefabObj.transform.position.x, prefabObj.transform.position.y + height, prefabObj.transform.position.z), prefabObj.transform.rotation) as GameObject;
        up.name = "up";
        up.transform.Rotate(new Vector3(0, 0, 0));
        up.transform.parent = room.transform;


        prefabObj.transform.localScale = new Vector3(length * 0.1f, 0.1f * 0.1f, height * 0.1f);

        GameObject left = GameObject.Instantiate(prefabObj, new Vector3(prefabObj.transform.position.x, prefabObj.transform.position.y + height / 2.0f, prefabObj.transform.position.z + width / 2.0f), prefabObj.transform.rotation) as GameObject;
        left.name = "left";
        left.transform.Rotate(new Vector3(90, 0, 0));
        left.transform.parent = room.transform;


        GameObject right = GameObject.Instantiate(prefabObj, new Vector3(prefabObj.transform.position.x, prefabObj.transform.position.y + height / 2.0f, prefabObj.transform.position.z - width / 2.0f), prefabObj.transform.rotation) as GameObject;
        right.name = "right";
        right.transform.Rotate(new Vector3(90, 0, 0));
        right.transform.parent = room.transform;
       


        prefabObj.transform.localScale = new Vector3(height * 0.1f, 0.1f * 0.1f, width * 0.1f);

        GameObject forward = GameObject.Instantiate(prefabObj, new Vector3(prefabObj.transform.position.x - length / 2.0f, prefabObj.transform.position.y + height / 2.0f, prefabObj.transform.position.z), prefabObj.transform.rotation) as GameObject;
        forward.name = "forward";
        forward.transform.Rotate(new Vector3(0, 0, 90));
        forward.transform.parent = room.transform;

        GameObject back = GameObject.Instantiate(prefabObj, new Vector3(prefabObj.transform.position.x + length / 2.0f, prefabObj.transform.position.y + height / 2.0f, prefabObj.transform.position.z), prefabObj.transform.rotation) as GameObject;
        back.name = "back";
        back.transform.Rotate(new Vector3(0, 0, 90));
        back.transform.parent = room.transform;

        Debug.Log("bottom");
        Debug.Log(bottom.transform.position.ToString("f4"));
        Debug.Log(bottom.transform.localScale.ToString("f4"));
        Debug.Log("up");
        Debug.Log(up.transform.position.ToString("f4"));
        Debug.Log(up.transform.localScale.ToString("f4"));
        Debug.Log("left");
        Debug.Log(left.transform.position.ToString("f4"));
        Debug.Log(left.transform.localScale.ToString("f4"));
        Debug.Log("right");
        Debug.Log(right.transform.position.ToString("f4"));
        Debug.Log(right.transform.localScale.ToString("f4"));
        Debug.Log("f");
        Debug.Log(forward.transform.position.ToString("f4"));
        Debug.Log(forward.transform.localScale.ToString("f4"));
        Debug.Log("b");
        Debug.Log(back.transform.position.ToString("f4"));
        Debug.Log(back.transform.localScale.ToString("f4"));
        prefabObj.SetActive(false);
    }
    public void ScaleRoom()
    {
        //plane.transform.localScale = new Vector3(width, height, 1.0f);
        //CreateCube(testObjs);
    }
    string status = "forward";
    public void change(){
        switch (status) {
            case "left":
                status = "right";
                break;
            case "right":
                status = "forward";
                break;
            case "forward":
                status = "back";
                break;
            case "back":
                status = "left";
                break;
        }

    }
    public void MoveLeftRight(){
        children = room.GetComponentsInChildren<Transform>();
        foreach(Transform child in children){
            switch(status){
                case "left":
                    if (child.gameObject.name == "left")
                    {
                        child.position = new Vector3(child.position.x + 0.2f * (0.5f - moveUnit), child.position.y, child.position.z);
                    }
                    break;
                case "right":
                    if (child.gameObject.name == "right")
                        child.position = new Vector3(child.position.x - 0.2f * (0.5f-moveUnit), child.position.y, child.position.z);
                    break;
                case "forward":
                    if (child.gameObject.name == "forward")
                        child.position = new Vector3(child.position.x, child.position.y, child.position.z - 0.2f * (0.5f-moveUnit));
                    break;
                case "back":
                    if (child.gameObject.name == "back")
                        child.position = new Vector3(child.position.x, child.position.y, child.position.z + 0.2f * (0.5f-moveUnit));
                break;
                default:
                    break;
            }
        }

    }
    //public void MoveForwardBack()
    //{
    //    children = room.GetComponentsInChildren<Transform>();
    //    foreach (Transform child in children)
    //    {
    //        switch (child.gameObject.name)
    //        {
    //            //case "left":
    //            //    child.position = new Vector3(child.position.x - 0.2f * moveUnit, child.position.y, child.position.z);
    //            //    break;
    //            //case "right":
    //                //child.position = new Vector3(child.position.x + 0.2f * moveUnit, child.position.y, child.position.z);
    //                //break;
    //            case "forward":
    //                child.position = new Vector3(child.position.x, child.position.y, child.position.z - 0.2f * (0.5f-moveUnit));
    //                break;
    //            case "back":
    //                child.position = new Vector3(child.position.x, child.position.y, child.position.z + 0.2f * (0.5f-moveUnit));
    //                break;
    //            default:
    //                break;
    //        }
    //    }

    //}
    public void MoveCubeIn()
    {
        children = room.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            switch (status)
            {
                case "left":
                    if (child.gameObject.name == "left")
                        child.position = new Vector3(child.position.x, child.position.y, child.position.z - 0.2f * (0.5f - moveUnit));
                    break;
                case "right":
                    if (child.gameObject.name == "right")
                        child.position = new Vector3(child.position.x, child.position.y, child.position.z - 0.2f * (0.5f - moveUnit));
                    break;
                case "forward":
                    if (child.gameObject.name == "forward")
                        child.position = new Vector3(child.position.x - 0.2f * (0.5f - moveUnit), child.position.y, child.position.z);
                    break;
                case "back":
                    if (child.gameObject.name == "back")
                        child.position = new Vector3(child.position.x - 0.2f * (0.5f - moveUnit), child.position.y, child.position.z);
                    break;
                default:
                    break;
            }
        }

    }
    public void MoveUpDown()
    {
        children = room.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            child.position = new Vector3(child.position.x, child.position.y - 0.2f * (0.5f - moveUnit), child.position.z);
        }

    }


}
