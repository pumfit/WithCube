using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;


public class ARManager : MonoBehaviour
{
    [Header("AR Session Origin")]
    public ARSession ARSession;
    public ARPlaneManager ARPlaneManager;

    [Header("터치 UI")]
    public GameObject canvas;
    public Image progressBar;
    float currentValue;

    [Header("영역 설정 grid 관련")]
    public GameObject prefabGrid;
    public GameObject grid;
    public GameObject sprite;
    public GameObject spriteWhite;
    private int gridX,gridY, gridZ;

    [Header("영역 설정 UI")]
    public int blockNum;
    public Text blockNumText;
    public Text blockSizeText;

    public InputField InputWidth;
    public InputField InputHeight;

    public int gridWidth;
    public int gridHeigth;

    public GameObject DeleteModeManager;
    public GameObject mainBlock;
    public GameObject mainExplodedView;


    private bool isFirst = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PlacePrefab();
    }

    #region 바닥에 프리팹 놓기

    [Header("바닥에 프리팹 설치 전용")]
    public ARRaycastManager arRaycater;
    public GameObject spawnPrefab;
    public float gridSize;

    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void PlacePrefab()
    {
        if (Input.touchCount == 0 ) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began) return;

        if (arRaycater.Raycast(touch.position, hits, TrackableType.Planes)|| Input.GetMouseButtonDown(0))//바닥을 검출한 후
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Pose hitPose = hits[0].pose;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                Vector3 spwanPos;
                spwanPos.x = Mathf.Round(hitPose.position.x / gridSize) * gridSize;
                spwanPos.y = Mathf.Round(hitPose.position.y / gridSize) * gridSize;
                spwanPos.z = Mathf.Round(hitPose.position.z / gridSize) * gridSize;
                Quaternion spwanQt = new Quaternion(0, 0, 0, 0);

                if (Physics.Raycast(ray, out hit))//한층 위
                {
                    if (isFirst == true) // 첫 생성이라면 grid 생성
                    {
                        if (!(grid == null))
                        {
                            Destroy(grid);
                        }
                        grid = Instantiate(prefabGrid, spwanPos, spwanQt);
                        grid.transform.Rotate(90, 0, 0);
                        gridY = (int)grid.transform.position.y;
                        gridX = (int)grid.transform.position.x;
                        gridZ = (int)grid.transform.position.z;
                        setGrid();

                        isFirst = false;
                    }
                    else
                    {
                        if (hit.collider.CompareTag("GameController"))
                        {
                            spwanPos.y = hit.collider.transform.position.y + gridSize;

                            if ((gridX - gridWidth / 2 <= spwanPos.x) && (spwanPos.x <= gridX + gridWidth / 2)
                                && (gridZ - gridHeigth / 2 <= spwanPos.z) && (spwanPos.z <= gridZ + gridHeigth / 2))
                            {
                                GameObject prefab = Instantiate(spawnPrefab, spwanPos, spwanQt);
                                prefab.transform.SetParent(mainBlock.transform);
                                blockNum++;
                            }
                        }
                        else
                        {
                            spwanPos.y = gridY;

                            if ((gridX - gridWidth / 2 <= spwanPos.x) && (spwanPos.x <= gridX + gridWidth / 2)
                                && (gridZ - gridHeigth / 2 <= spwanPos.z) && (spwanPos.z <= gridZ + gridHeigth / 2))
                            {
                                GameObject prefab = Instantiate(spawnPrefab, spwanPos, spwanQt);
                                prefab.transform.SetParent(mainBlock.transform);
                                blockNum++;
                            }

                        }
                        blockNumText.text = "" + blockNum;
                    }
                }
            }  

        }
    }

    #endregion

    void setGrid()
    {
        grid.GetComponent<RectTransform>().sizeDelta = new Vector2(gridWidth, gridHeigth);

        for (int i = 0; i < gridWidth * gridHeigth; i++)
        {
            GameObject child = Instantiate(sprite) as GameObject;
            child.transform.SetParent(grid.transform);
            child.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    public void onClickedSetGrid()
    {
        //짝수 인 경우 -1 홀수 인경우 그냥
        int getWidth = int.Parse(InputWidth.text);
        int getHeight = int.Parse(InputHeight.text);

        if (getWidth % 2 == 0)
        {
            gridWidth = getWidth-1;
        } else
        {
            gridWidth = getWidth;
        }

        if (getHeight % 2 == 0)
        {
            gridHeigth = getHeight-1;
        }
        else
        {
            gridHeigth = getHeight;
        }

        var child = grid.transform.GetComponentsInChildren<Transform>();

        foreach (var iter in child)
        {
            if (iter != grid.transform)
            {
                Destroy(iter.gameObject);
            }
        }

        if (mainBlock.transform.childCount > 0)
        {
            //이미  존재하는 경우
            for (int i = 0; i < mainBlock.transform.childCount; i++)
            {
                Destroy(mainBlock.transform.GetChild(i).gameObject);
            }
        }

        blockSizeText.text = gridWidth + " X " + gridHeigth;
        blockNum = 0;
        blockNumText.text = "" + blockNum;
        setGrid();
    }

    public void SetSessionReset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //ARSession.Reset();
    }

    public void view2D()
    {
        if (grid != null)
        {
            if (mainExplodedView.transform.childCount > 0)
            {
                //이미  존재하는 경우
                for (int i = 0; i < mainExplodedView.transform.childCount; i++)
                {
                    Destroy(mainExplodedView.transform.GetChild(i).gameObject);
                }
            }
            else
            {
                int[,] array = new int[gridHeigth, gridWidth]; //z,x,h
                int maxHeight = 0;

                for (int i = 0; i < mainBlock.transform.childCount; i++)
                {
                    int x = (int)(mainBlock.transform.GetChild(i).gameObject.transform.position.x + gridWidth / 2 - gridX);
                    int z = (int)(mainBlock.transform.GetChild(i).gameObject.transform.position.z + gridHeigth / 2 - gridZ);
                    array[x, z] += 1;
                }

                for (int i = 0; i < gridHeigth; i++)//최대값 찾기
                {
                    for (int j = 0; j < gridHeigth; j++)
                    {
                        if (maxHeight < array[i, j])
                            maxHeight = array[i, j];
                    }
                }

                set2DGrid(array, gridX, gridY + maxHeight + (int)gridSize, gridZ, gridWidth, gridHeigth);

                int[,] arrayFront = new int[maxHeight, gridWidth]; //z,x,h
                                                                   //앞에서 본 모습
                for (int z = 0; z < gridHeigth; z++)
                {

                    int maxCol = 0;

                    for (int x = 0; x < gridWidth; x++)
                    {
                        if (array[z, x] > maxCol)
                        {
                            maxCol = array[z, x];
                        }

                    }
                    for (int i = 0; i < maxCol; i++)
                    {
                        arrayFront[i, z] = 1;
                    }

                }

                set2DFrontSideGrid(arrayFront, gridX, gridY, gridZ - gridHeigth - (int)gridSize, gridWidth, maxHeight, 0);

                int[,] arraySide = new int[maxHeight, gridHeigth]; //z,x,h

                //옆에서 본 모습
                for (int z = 0; z < gridWidth; z++)
                {
                    int maxCol = 0;

                    for (int x = 0; x < gridHeigth; x++)
                    {
                        if (array[x, z] > maxCol)
                        {
                            maxCol = array[x, z];
                        }

                    }
                    for (int i = 0; i < maxCol; i++)
                    {
                        arraySide[i, z] = 1;
                        Debug.Log(i + "" + z);
                    }

                }

                set2DFrontSideGrid(arraySide, gridX + gridWidth + (int)gridSize, gridY, gridZ, gridHeigth, maxHeight, 90);
            }          
        }
        else
        {
            //그리드를 먼저 설치해주세요
        }

        void set2DGrid(int[,] arr,int printX,int printY,int printZ,int width,int height)
        {
            GameObject grid2D = Instantiate(prefabGrid,new Vector3(0,0,0), new Quaternion(0,0,0,0));
            grid2D.transform.SetParent(mainExplodedView.transform);
            grid2D.transform.position = new Vector3(printX, printY, printZ);
            grid2D.transform.Rotate(90, 0, 0);
            grid2D.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);


            for (int z = height - 1; z >= 0; z--)
            {

                for (int x = 0; x < gridWidth; x++)
                {
                    if (arr[x, z] > 0)
                    {
                        GameObject child = Instantiate(spriteWhite) as GameObject;
                        child.transform.SetParent(grid2D.transform);
                        child.transform.localPosition = new Vector3(0, 0, 0);
                    }
                    else
                    {
                        GameObject child = Instantiate(sprite) as GameObject;
                        child.transform.SetParent(grid2D.transform);
                        child.transform.localPosition = new Vector3(0, 0, 0);
                    }

                }

            }
        }

        void set2DFrontSideGrid(int[,] arr, int printX, int printY, int printZ, int width, int height,int rot)
        {
            GameObject grid2D = Instantiate(prefabGrid, new Vector3(0,0,0), new Quaternion(0, 0, 0, 0));
            grid2D.transform.SetParent(mainExplodedView.transform);
            grid2D.transform.position = new Vector3( printX, printY,  printZ);
            grid2D.transform.Rotate(90, 0, rot);
            grid2D.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);


            for (int z = height - 1; z >= 0; z--)//위에서 보는 경우
            {

                for (int x = 0; x < gridWidth; x++)
                {
                    if (arr[z, x] > 0)
                    {
                        GameObject child = Instantiate(spriteWhite) as GameObject;
                        child.transform.SetParent(grid2D.transform);
                        child.transform.localPosition = new Vector3(0, 0, 0);
                    }
                    else
                    {
                        GameObject child = Instantiate(sprite) as GameObject;
                        child.transform.SetParent(grid2D.transform);
                        child.transform.localPosition = new Vector3(0, 0, 0);
                    }

                }

            }
        }

    }
 
}
