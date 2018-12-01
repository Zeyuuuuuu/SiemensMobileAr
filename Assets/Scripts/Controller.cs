using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;//场景头部引入
using System.Threading;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour
{
    [Header("Prefebs")]
    public GameObject RoomPrefeb;
    public GameObject RouterPrefeb;
    public GameObject RouterPreviewerPrefeb;
    public GameObject CheckPointPreviewerPrefeb;

    public GameObject heatPrefeb;

    public GameObject CheckPointPrefeb;

    [Header("UI")]
    public GameObject joystick;    //手柄
    public GameObject hideButton;
    public GameObject addRouterButton;
    public GameObject checkButton;
    public GameObject restartButton;
    public GameObject editButton;
    public GameObject delButton;
    public GameObject hideRouterButton;
    public GameObject hideListButton;
    public GameObject hideHeatButton;
    public GameObject SwitchCheckPointButton;
    public GameObject DelCheckPointButton;
    public GameObject RenameBox;
    public GameObject DelBox;
    public GameObject InputText;
    public GameObject DelText;



    public GameObject Menu;
    public GameObject Mask;
    public Sprite HideHeat;
    public Sprite ShowHeat;
    public Sprite HideList;
    public Sprite ShowList;
    public Sprite RedCheckPoint;
    public Sprite YellowCheckPoint;
    public Sprite GreenCheckPoint;


    [Header("Parameters")]
    public float maxRayDistance = 30.0f;



    [Header("")]
    public GameObject GenerateImageAnchor;    //检测marker
    public Material roomMaterial;   //房间材质

    [Header("Debug")]
    public GameObject originObject;
    public Transform parentTransForm;

    bool isSearchingRoom = false;
    bool isPlacingRouter = false; //处于放置状态
    bool isPlacingCheckPoint = false;
    bool UItouched = false;
    bool RoomSetted = false;   //是否已放置房间
    bool isDeleting = false;
    bool isRenaming = false;
    bool heatAllHiden = false;
    float roomTransparency = 0.0f;    //房间透明度
    GameObject RouterPreviewer;    //预览router对象
    GameObject CheckPointPreviewer;
    GameObject chosenRouter;    //选定router对象
    GameObject chosenCheckpoint;    //选定checkpoint对象
    GameObject roomcube;    //房间对象
    string IdToBeDel;//待删除对象id
    string IdTobeRename;//待改名对象id
    List<GameObject> Heats = new List<GameObject>();
    List<GameObject> Routers = new List<GameObject>();
    List<GameObject> CheckPoints = new List<GameObject>();
    List<RouterInfo> routerInfos = new List<RouterInfo>();



    float moveUnit = 0.0f;    //移动缩放尺度
    float scaleUnit = 0.1f;
    int routerId = 1;
    int checkpointId = 1;


    void Start()
    {
        joystick.GetComponent<ETCJoystick>().visible = false;
        joystick.GetComponent<ETCJoystick>().activated = false;
    }
    void Update()
    {
        if (RoomSetted && !isPlacingRouter && RouterPreviewer != null)//房间已经放置，但是不处于放置状态，但是却有预览
            RouterPreviewer.SetActive(false);
        if (RoomSetted && !isPlacingCheckPoint && CheckPointPreviewer != null)
            CheckPointPreviewer.SetActive(false);

        if (CheckPoints.Count != 0)
        {
            foreach (GameObject point in CheckPoints)
            {
                point.transform.LookAt(Camera.main.transform.position);
                point.transform.rotation = Quaternion.Euler(0, point.transform.rotation.eulerAngles.y, 0);
                // point.transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(Camera.main.transform.position - point.transform.position), point.transform.rotation, 10 * Time.deltaTime);
            }
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {

            // Check if finger is over a UI element
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                Debug.Log("Touched the UI");
                UItouched = true;
                return;
            }
            else
            {
                UItouched = false;
            }
        }

        if (RoomSetted)
        {
            RouterPreview();
            CheckPointPreview();
            MobilePick();
        }
        if (isSearchingRoom)
            Searching();
    }
    void RouterPreview()    //预览router放置位置
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("planePrefebs") as GameObject[];
        if (objs.Length != 0)
        {
            foreach (GameObject obj in objs)    //遍历所有gameobject
            {
                obj.gameObject.SetActive(false);
            }
        }
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (isPlacingRouter && Physics.Raycast(ray, out hit, maxRayDistance, 1 << 12))
        {
            if (RouterPreviewer == null)
            {
                RouterPreviewer = Instantiate(RouterPreviewerPrefeb, hit.point, hit.transform.rotation) as GameObject;
            }
            else
            {
                RouterPreviewer.transform.position = hit.point;
                RouterPreviewer.transform.rotation = hit.transform.rotation;
            }
            switch (hit.transform.gameObject.tag)
            {
                case "ground":
                    RouterPreviewer.transform.Rotate(new Vector3(0, 0, 0));
                    break;
                case "back":
                    RouterPreviewer.transform.Rotate(new Vector3(-90, 0, -180));
                    break;
                case "forward":
                    RouterPreviewer.transform.Rotate(new Vector3(-90, -180, -180));
                    break;
                case "left":
                    RouterPreviewer.transform.Rotate(new Vector3(-90, -90, 0));
                    break;
                case "right":
                    RouterPreviewer.transform.Rotate(new Vector3(-90, 90, 0));
                    break;
                case "celling":
                    RouterPreviewer.transform.Rotate(new Vector3(-180, 0, 0));
                    break;


            }

        }
    }
    void CheckPointPreview()    //预览router放置位置
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (isPlacingCheckPoint && Physics.Raycast(ray, out hit, maxRayDistance, 1 << 12) && hit.transform.gameObject.tag == "ground")
        {
            if (CheckPointPreviewer == null)
            {
                CheckPointPreviewer = Instantiate(CheckPointPreviewerPrefeb, hit.point, hit.transform.rotation) as GameObject;
            }
            else
            {
                CheckPointPreviewer.transform.position = hit.point;
            }

        }
    }

    void Searching()
    {

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (!RoomSetted && Physics.Raycast(ray, out hit, maxRayDistance, 1 << 10))
        {
            hit.transform.gameObject.tag = "selected";
            roomcube = Instantiate(RoomPrefeb, hit.transform.position, Quaternion.Euler(new Vector3(0, hit.transform.rotation.eulerAngles.y, 0))) as GameObject;
            roomcube.transform.localPosition = new Vector3(roomcube.transform.localPosition.x + 0.10f, roomcube.transform.localPosition.y, roomcube.transform.localPosition.z + 0.15f);
            GenerateImageAnchor.SetActive(false);
            RoomSetted = true;
            hideButton.SetActive(true);
            isSearchingRoom = false;
            restartButton.SetActive(true);
            Mask.SetActive(false);
        }

    }
    void MobilePick()    //触屏点击
    {
        if (isDeleting || isRenaming)
            return;
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if (isPlacingRouter)//放置
                {
                    Routers.Add(Instantiate(RouterPrefeb, RouterPreviewer.transform.position, RouterPreviewer.transform.rotation) as GameObject);
                    isPlacingRouter = false;
                    foreach (Transform r in Routers[Routers.Count - 1].GetComponentsInChildren<Transform>())
                    {
                        if (r.gameObject.name == "GameObject")
                            r.gameObject.name = routerId.ToString();
                    }
                    RouterInfo tempRouterInfo = new RouterInfo(routerId.ToString(), routerId.ToString() + "号设备");
                    routerInfos.Add(tempRouterInfo);
                    Debug.Log(tempRouterInfo._name);
                    Heats.Add(
                        Instantiate(
                            heatPrefeb,
                            new Vector3(RouterPreviewer.transform.position.x, roomcube.transform.position.y, RouterPreviewer.transform.position.z),
                            heatPrefeb.transform.rotation)
                            );
                    Heats[Heats.Count - 1].name = routerId.ToString();
                    routerId++;
                    RouterPreviewer.SetActive(false);
                    addRouterButton.SetActive(true);
                    checkButton.SetActive(true);
                    restartButton.SetActive(true);
                    hideHeatButton.SetActive(true);
                    hideListButton.SetActive(true);
                }
                else if (isPlacingCheckPoint)
                {
                    CheckPoints.Add(Instantiate(CheckPointPrefeb, CheckPointPreviewer.transform.position, CheckPointPreviewer.transform.rotation) as GameObject);
                    isPlacingCheckPoint = false;
                    CheckPoints[CheckPoints.Count - 1].name = checkpointId.ToString();
                    checkpointId++;
                    CheckPointPreviewer.SetActive(false);
                    addRouterButton.SetActive(true);
                    checkButton.SetActive(true);
                    restartButton.SetActive(true);
                    hideHeatButton.SetActive(true);
                    hideListButton.SetActive(true);

                }
                else//不处于放置状态
                {
                    if (chosenRouter != null && chosenCheckpoint == null)//取消router的选定
                    {
                        foreach (Transform child in chosenRouter.transform)
                        {
                            if (child.gameObject.name == "Circle" || child.gameObject.name == "RouterName" || child.gameObject.name == "NameBoard")
                            {
                                child.gameObject.SetActive(false);
                            }
                            else
                            {
                                child.gameObject.GetComponent<MeshRenderer>().material.color = new Vector4(0.2745f, 0.2745f, 0.2745f); ;
                            }
                        }
                        UnchoseRouter();
                    }
                    else if (chosenRouter == null && chosenCheckpoint != null)//取消选定checkpoint，
                    {

                        foreach (Transform child in chosenCheckpoint.transform)
                        {
                            if (child.gameObject.name == "Circle")
                            {
                                child.gameObject.SetActive(false);
                            }
                        }
                        UnchoseCheckPoint();
                    }
                    if (Physics.Raycast(ray, out hit, maxRayDistance, 1 << 11))//点击到11.模型，则选定
                    {
                        chosenRouter = hit.transform.gameObject;
                        ChoseRouter();
                    }
                    else if (Physics.Raycast(ray, out hit, maxRayDistance, 1 << 14))//点击到14.标定点，则选定
                    {
                        chosenCheckpoint = hit.transform.gameObject;
                        ChoseCheckPoint();
                    }

                }
            }
            else if (!UItouched && Input.GetTouch(0).phase == TouchPhase.Moved)//移动
            {

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

                if (Physics.Raycast(ray, out hit, maxRayDistance, 1 << 12))//点击到12.墙体
                {
                    if (chosenRouter != null)
                    {
                        chosenRouter.transform.parent.gameObject.transform.position = hit.point;
                        chosenRouter.transform.parent.gameObject.transform.rotation = hit.transform.rotation;
                        chosenRouter.transform.position = hit.point;
                        switch (hit.transform.gameObject.tag)
                        {
                            case "ground":
                                chosenRouter.transform.parent.gameObject.transform.Rotate(new Vector3(0, 0, 0));
                                break;
                            case "back":
                                chosenRouter.transform.parent.gameObject.transform.Rotate(new Vector3(-90, 0, -180));
                                break;
                            case "forward":
                                chosenRouter.transform.parent.gameObject.transform.Rotate(new Vector3(-90, -180, -180));
                                break;
                            case "left":
                                chosenRouter.transform.parent.gameObject.transform.Rotate(new Vector3(-90, -90, 0));
                                break;
                            case "right":
                                chosenRouter.transform.parent.gameObject.transform.Rotate(new Vector3(-90, 90, 0));
                                break;
                            case "celling":
                                chosenRouter.transform.parent.gameObject.transform.Rotate(new Vector3(-180, 0, 0));
                                break;
                        }
                        foreach (GameObject heat in Heats)
                        {
                            if (heat.name == chosenRouter.name)
                            {
                                // Debug.Log(heat.name);
                                heat.transform.position = new Vector3(chosenRouter.transform.position.x, heat.transform.position.y, chosenRouter.transform.position.z);
                            }
                        }
                    }
                    else if (chosenCheckpoint != null && hit.transform.gameObject.tag == "ground")
                    {
                        chosenCheckpoint.transform.position = hit.point;
                    }
                }
            }
        }
    }
    void ChoseRouter()
    {
        chosenCheckpoint = null;
        Debug.Log(chosenRouter.name);
        foreach (RouterInfo r in routerInfos)
        {
            if (chosenRouter.name == r._id)
            {
                Debug.Log(r._name);
                foreach (Transform child in chosenRouter.transform)
                {
                    if (child.gameObject.name == "RouterName" && child.gameObject.GetComponent<TextMesh>().text != r._name)
                    {
                        child.gameObject.GetComponent<TextMesh>().text = r._name;
                        break;
                    }

                }
            }
        }
        joystick.GetComponent<ETCJoystick>().visible = true;
        joystick.GetComponent<ETCJoystick>().activated = true;

        //显示圈圈
        foreach (Transform child in chosenRouter.transform)
        {
            if (child.gameObject.name == "Circle" || child.gameObject.name == "RouterName" || child.gameObject.name == "NameBoard")
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.GetComponent<MeshRenderer>().material.color = new Vector4(0.4863f, 0.6667f, 0.9529f);
            }
        }
        addRouterButton.SetActive(false);
        checkButton.SetActive(false);
        restartButton.SetActive(false);
        DelCheckPointButton.SetActive(false);
        SwitchCheckPointButton.SetActive(false);
        editButton.SetActive(true);
        delButton.SetActive(true);
        hideRouterButton.SetActive(true);
    }
    void ChoseCheckPoint()
    {
        chosenRouter = null;
        //显示圈圈
        foreach (Transform child in chosenCheckpoint.transform)
        {
            if (child.gameObject.name == "Circle")
            {
                child.gameObject.SetActive(true);
            }

        }
        addRouterButton.SetActive(false);
        checkButton.SetActive(false);
        restartButton.SetActive(false);
        editButton.SetActive(false);
        delButton.SetActive(false);
        hideRouterButton.SetActive(false);
        DelCheckPointButton.SetActive(true);
        SwitchCheckPointButton.SetActive(true);
    }
    void UnchoseRouter()    //取消选定router
    {
        chosenRouter = null;
        joystick.GetComponent<ETCJoystick>().visible = false;
        joystick.GetComponent<ETCJoystick>().activated = false;
        // RouterPreviewer.SetActive(true);
        //隐藏圈圈

        //按钮
        addRouterButton.SetActive(true);
        checkButton.SetActive(true);
        restartButton.SetActive(true);
        editButton.SetActive(false);
        delButton.SetActive(false);
        hideRouterButton.SetActive(false);
    }
    void UnchoseCheckPoint()
    {
        foreach (Transform child in chosenCheckpoint.transform)
        {
            if (child.gameObject.name == "Page" && child.gameObject.activeSelf)
            {
                SwitchCheckPoint();
            }
        }
        chosenCheckpoint = null;
        addRouterButton.SetActive(true);
        checkButton.SetActive(true);
        restartButton.SetActive(true);
        DelCheckPointButton.SetActive(false);
        SwitchCheckPointButton.SetActive(false);
    }
    GameObject delObj(GameObject o)
    {
        if (o != null)
        {
            Destroy(o, 0.01f);
        }
        return null;
    }
    //按钮事件


    public void ReStart()    //重启
    {
        if (isDeleting || isRenaming)
            return;
        if (RoomSetted)
        {
            roomcube = delObj(roomcube);
            isSearchingRoom = false;
            isPlacingRouter = false; //处于放置状态
            isPlacingCheckPoint = false;
            UItouched = false;
            RoomSetted = false;   //是否已放置房间
            isDeleting = false;
            isRenaming = false;
            heatAllHiden = false;
            RouterPreviewer = delObj(RouterPreviewer);
            chosenCheckpoint = delObj(chosenCheckpoint);
            roomcube = delObj(roomcube);
            IdToBeDel = null;//待删除对象id
            IdTobeRename = null;//待改名对象id
            for (int i = Heats.Count - 1; i >= 0; i--)
            {
                Destroy(Heats[i]);
                Destroy(Routers[i]);
            }
            Heats = new List<GameObject>();
            Routers = new List<GameObject>();
            for (int i = CheckPoints.Count - 1; i >= 0; i--)
            {
                Destroy(CheckPoints[i]);
            }
            CheckPoints = new List<GameObject>();
            routerInfos = new List<RouterInfo>();
            moveUnit = 0.0f;    //移动缩放尺度
            scaleUnit = 0.1f;
            routerId = 1;
            checkpointId = 1;

        }
        Mask.SetActive(true);
        isSearchingRoom = true;
        restartButton.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(45f, 56f, 0f);
        addRouterButton.SetActive(false);
        checkButton.SetActive(false);
        hideHeatButton.SetActive(false);
        hideListButton.SetActive(false);
        hideButton.SetActive(false);
        restartButton.SetActive(false);
    }
    public void AddRouter()    //放置router
    {
        if (isDeleting || isRenaming)
            return;
        if (RoomSetted && chosenRouter == null)
        {
            isPlacingRouter = true;
            if (RouterPreviewer != null)
                RouterPreviewer.SetActive(true);
            addRouterButton.SetActive(false);
            checkButton.SetActive(false);
            restartButton.SetActive(false);
            hideHeatButton.SetActive(false);
            hideListButton.SetActive(false);
        }

    }
    public void AddCheckPoint()    //放置checkpoint
    {
        if (isDeleting || isRenaming)
            return;
        if (RoomSetted && chosenCheckpoint == null)
        {
            isPlacingCheckPoint = true;
            if (CheckPointPreviewer != null)
                CheckPointPreviewer.SetActive(true);
            addRouterButton.SetActive(false);
            checkButton.SetActive(false);
            restartButton.SetActive(false);
            hideHeatButton.SetActive(false);
            hideListButton.SetActive(false);
        }

    }
    // public void AddCheckPoint()
    // {
    //     if (isDeleting || isRenaming)
    //         return;
    //     if (!RoomSetted || isPlacingRouter)
    //         return;
    //     RaycastHit hit;
    //     Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
    //     Debug.Log("hit");
    //     if (Physics.Raycast(ray, out hit))
    //     {
    //         Debug.Log(hit.transform.gameObject.layer);

    //         if (hit.transform.gameObject.tag == "ground")
    //         {
    //             CheckPoints.Add(Instantiate(CheckPointPrefeb, hit.point, hit.transform.rotation) as GameObject);
    //             CheckPoints[CheckPoints.Count - 1].name = checkpointId.ToString();
    //             checkpointId++;
    //         }
    //     }


    // }
    public void DelRouter()
    {
        if (isDeleting || isRenaming || chosenRouter == null)
            return;
        IdToBeDel = chosenRouter.name;
        foreach (RouterInfo info in routerInfos)
        {
            if (info._id == IdToBeDel)
            {
                DelText.GetComponent<Text>().text = DelText.GetComponent<Text>().text + info._name + "?";
                break;
            }
        }
        isDeleting = true;
        DelBox.SetActive(true);
    }
    public void ConfirmDel()
    {
        if (isDeleting && IdToBeDel != null)
        {
            //Heats里的
            //routerinfors里的
            //Routers里的
            //每个checkpoints中page里的
            for (int i = Heats.Count - 1; i >= 0; i--)
            {
                if (Heats[i].name == IdToBeDel)
                {
                    Destroy(Heats[i], 0.01f);
                    Destroy(Routers[i], 0.01f);
                    Heats.Remove(Heats[i]);
                    routerInfos.Remove(routerInfos[i]);
                    Routers.Remove(Routers[i]);

                    break;
                }
            }
            DelText.GetComponent<Text>().text = "确认删除";
            IdToBeDel = null;
            isDeleting = false;
            DelBox.SetActive(false);
            UnchoseRouter();
        }
    }
    public void CancleDel()
    {
        if (isDeleting)
        {
            DelText.GetComponent<Text>().text = "确认删除";
            IdToBeDel = null;
            isDeleting = false;
            DelBox.SetActive(false);
        }
    }
    public void SwitchCheckPoint()
    {
        if (isDeleting || isRenaming)
            return;
        bool isPage = false;
        int showNum = Routers.Count > 6 ? 6 : Routers.Count;
        int inforIndex = 0;
        if (chosenCheckpoint != null)
        {
            foreach (Transform child in chosenCheckpoint.transform)
            {
                if (child.gameObject.name == "Page")
                {
                    child.gameObject.SetActive(!child.gameObject.activeSelf);
                    if (child.gameObject.activeSelf)
                        isPage = true;
                }
                else if (child.gameObject.name == "CheckPoint")
                {
                    child.gameObject.SetActive(!child.gameObject.activeSelf);
                }
                else if (child.gameObject.name == "Text")
                {
                    child.gameObject.SetActive(!child.gameObject.activeSelf);
                }
                else if (isPage && inforIndex < showNum && child.gameObject.name == "RouterInfo" + inforIndex)
                {
                    foreach (Transform grandchild in child.transform)
                    {
                        if (grandchild.gameObject.name == "RouterName" && grandchild.GetComponent<TextMesh>().text != routerInfos[inforIndex]._name)
                        {
                            grandchild.GetComponent<TextMesh>().text = routerInfos[inforIndex]._name;
                        }
                        else if (grandchild.gameObject.name == "SignalStrength")
                        {
                            grandchild.GetComponent<TextMesh>().text = ((float)(Mathf.Round(Vector3.Distance(Heats[inforIndex].transform.position, chosenCheckpoint.transform.position) * 1000)) / 1000).ToString();
                        }
                    }
                    child.gameObject.SetActive(true);
                    inforIndex++;
                }
                else if ((isPage && inforIndex < 6 && inforIndex >= showNum && child.gameObject.name == "RouterInfo" + inforIndex)
                || (!isPage && inforIndex < 6 && child.gameObject.name == "RouterInfo" + inforIndex))
                {
                    child.gameObject.SetActive(false);
                    inforIndex++;
                }

            }
            if (isPage)
            {
                chosenCheckpoint.GetComponent<BoxCollider>().center = new Vector3(0, 2.05f, 0);
                chosenCheckpoint.GetComponent<BoxCollider>().size = new Vector3(2.1f, 4.1f, 0.2f);
            }
            else
            {
                chosenCheckpoint.GetComponent<BoxCollider>().center = new Vector3(0, 0.63f, 0);
                chosenCheckpoint.GetComponent<BoxCollider>().size = new Vector3(1.1f, 1.25f, 0.2f);

            }
        }
    }
    public void DelCheckPoint()
    {
        if (isDeleting || isRenaming)
            return;
        if (chosenCheckpoint != null)
        {
            string chosenCheckpointName = chosenCheckpoint.name;
            for (int i = CheckPoints.Count - 1; i >= 0; i--)
            {
                if (CheckPoints[i].name == chosenCheckpointName)
                {
                    Destroy(CheckPoints[i], 0.01f);
                    CheckPoints.Remove(CheckPoints[i]);
                    break;
                }
            }
            UnchoseCheckPoint();
        }
    }
    public void RenameRouter()
    {
        if (isDeleting || isRenaming || chosenRouter == null)
            return;
        IdTobeRename = chosenRouter.name;
        foreach (RouterInfo info in routerInfos)
        {
            Debug.Log(info._id);
            if (info._id == IdTobeRename)
            {
                InputText.GetComponent<InputField>().text = info._name;
                break;
            }
        }
        RenameBox.SetActive(true);
        isRenaming = true;
    }
    public void ConfirmRename()
    {
        if (isRenaming && IdTobeRename != null)
        {
            string NewName = InputText.GetComponent<InputField>().text;
            foreach (RouterInfo info in routerInfos)
            {
                Debug.Log(info._id);
                if (info._id == IdTobeRename)
                {
                    info._name = NewName;
                    break;
                }
            }
            foreach (GameObject router in Routers)
            {
                foreach (Transform routerBox in router.transform)
                {
                    if (routerBox.name == IdTobeRename)
                    {
                        foreach (Transform child in routerBox.transform)
                        {
                            if (child.gameObject.name == "RouterName" && child.gameObject.GetComponent<TextMesh>().text != NewName)
                            {
                                child.gameObject.GetComponent<TextMesh>().text = NewName;
                                IdTobeRename = null;
                                isRenaming = false;
                                RenameBox.SetActive(false);
                                return;
                            }

                        }
                    }
                }

            }



        }
    }
    public void CancleRename()
    {
        if (isRenaming)
        {
            InputText.GetComponent<InputField>().text = "";
            IdTobeRename = null;
            isRenaming = false;
            RenameBox.SetActive(false);
        }
    }

    public void HideRouter()    //隐藏选定router的heat
    {
        if (isDeleting || isRenaming)
            return;
        if (chosenRouter != null)
        {
            foreach (GameObject heat in Heats)
            {
                if (heat.name == chosenRouter.name)
                {
                    if (heat.activeSelf)
                        heat.SetActive(false);
                    else
                        heat.SetActive(true);

                }
            }
        }
    }
    public void HideRoom()    //隐藏房间
    {
        if (isDeleting || isRenaming)
            return;
        // if (roomTransparency == 0.0f)
        //     roomTransparency = roomMaterial.color.a;
        roomMaterial.color = new Color(roomMaterial.color.r, roomMaterial.color.g, roomMaterial.color.b, 0);
        hideButton.SetActive(false);
        restartButton.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(45f, 188f, 0f);
        hideHeatButton.SetActive(true);
        hideListButton.SetActive(true);
        addRouterButton.SetActive(true);
        checkButton.SetActive(true);

    }
    // public void ShowRoom()    //显示房间
    // {
    //     if (isDeleting || isRenaming)
    //         return;
    //     roomMaterial.color = new Color(roomMaterial.color.r, roomMaterial.color.g, roomMaterial.color.b, roomTransparency);
    // }
    public void OnMove(Vector2 vector)    //摇杆控制
    {
        if (isDeleting || isRenaming)
            return;
        if (chosenRouter != null)
        {
            chosenRouter.transform.localPosition = new Vector3(chosenRouter.transform.localPosition.x + vector.x * 0.003f, chosenRouter.transform.localPosition.y, chosenRouter.transform.localPosition.z + vector.y * 0.003f);
        }
        foreach (GameObject heat in Heats)
        {
            if (heat.name == chosenRouter.name)
            {
                heat.transform.position = new Vector3(chosenRouter.transform.position.x, heat.transform.position.y, chosenRouter.transform.position.z);
            }
        }
    }
    public void List()
    {
        if (isDeleting || isRenaming)
            return;
        if (Menu.activeSelf)
        {
            Menu.SetActive(false);
            hideListButton.GetComponent<Image>().sprite = ShowList;
        }
        else
        {
            Menu.SetActive(true);
            hideListButton.GetComponent<Image>().sprite = HideList;
        }
    }
    public void EmptyInput()
    {
        InputText.GetComponent<InputField>().text = "";
    }
    public void hideAllHeat()
    {
        if (isDeleting || isRenaming)
            return;
        if (!heatAllHiden)
        {
            foreach (GameObject heat in Heats)
            {
                heat.SetActive(false);
            }
            hideHeatButton.GetComponent<Image>().sprite = ShowHeat;
        }
        else
        {
            foreach (GameObject heat in Heats)
            {
                heat.SetActive(true);
            }
            hideHeatButton.GetComponent<Image>().sprite = HideHeat;
        }
        heatAllHiden = !heatAllHiden;
    }
    public void twistRouter(HedgehogTeam.EasyTouch.Gesture gesture)
    {
        Debug.Log("t");
        if (isDeleting || isRenaming || chosenRouter == null)
            return;
        chosenRouter.transform.parent.gameObject.transform.Rotate(0, gesture.twistAngle, 0);
    }

    //测试用

    public void InstantiateList()
    {
        if (isDeleting || isRenaming)
            return;
        GameObject.Instantiate(originObject, parentTransForm);
    }

    public void NextModel()    //下一个设计方案
    {
        bool find = false;
        foreach (Transform child in roomcube.transform)
        {
            if (find)
            {
                Debug.Log(child.gameObject.name);
                child.gameObject.SetActive(true);
                return;
            }
            if (child.gameObject.activeSelf && child.gameObject.layer != 11)
            {
                Debug.Log(child.gameObject.name);
                child.gameObject.SetActive(false);
                find = true;
            }
        }
        if (find)
        {
            foreach (Transform child in roomcube.transform)
            {
                Debug.Log(child.gameObject.name);
                child.gameObject.SetActive(true);
                return;
            }
        }
    }
    public void UpAndDown()    //房间y轴移动
    {
        roomcube.transform.position = new Vector3(roomcube.transform.position.x, roomcube.transform.position.y - (0.5f - moveUnit), roomcube.transform.position.z);
    }
    public void Rotate()    //房间旋转

    {
        roomcube.transform.Rotate(new Vector3(0, 90 * moveUnit, 0));
    }
    public void AntiRotate()
    {
        roomcube.transform.Rotate(new Vector3(0, -90 * moveUnit, 0));
    }
    public void OnMoveUnitChange(float value)    //移动缩放尺度调整

    {
        moveUnit = value;
    }
    public void OnScaleUnitChange(float value)
    {
        scaleUnit = value;
    }


}
