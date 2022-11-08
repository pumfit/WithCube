using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteMode : MonoBehaviour
{
    [Header("삭제 모드 구조")]
    private Stack<Vector3> deleteBlockStack;//삭제한 위치
    public GameObject spawnPrefab;
    public GameObject mainBlock;
    public bool isDeleteModeOn;

    [Header("삭제 모드 UI")]
    public GameObject[] invisibleAIIUI;
    public GameObject deleteModeUI;
    public GameObject ARManager;
    public Text blockNumText;
    public int blockNum;


    // Start is called before the first frame update
    void Start()
    {
        deleteBlockStack = new Stack<Vector3>();
        deleteModeUI.SetActive(false);
        isDeleteModeOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDeleteModeOn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("GameController"))//블럭을 선택한다면
                    {
                        blockNum--;
                        blockNumText.text = "" + blockNum;
                        hit.collider.gameObject.transform.GetChild(0).transform.GetComponent<Animation>().Play("Destroy");
                        deleteBlockStack.Push(hit.collider.gameObject.transform.position);//스택에 추가
                        Destroy(hit.collider?.gameObject,1f);
                    }
                }
            }
        }
    }

    public void onClickedDeleteMode()
    {
        isDeleteModeOn = true;
        blockNum = ARManager.GetComponent<ARManager>().blockNum;
        ARManager.SetActive(false);
        deleteModeUI.SetActive(true);
        foreach (GameObject obj in invisibleAIIUI)
        {
            obj.SetActive(false);
        }
    }

    public void onClickedDeleteModeExit()
    {
        isDeleteModeOn = false;
        //알림창 띄우기

        //실제 적용
        deleteBlockStack.Clear();
        //스택 초기화
        deleteModeUI.SetActive(false);
        foreach (GameObject obj in invisibleAIIUI)
        {
            obj.SetActive(true);
        }
        ARManager.SetActive(true);
        ARManager.GetComponent<ARManager>().blockNum = blockNum;
    }

    public void UndoDeleteBlock()
    {
        if (deleteBlockStack.Count > 0)//스택이 비어있지않다면
        {
            Vector3 prePos = deleteBlockStack.Pop();
            Quaternion spwanQt = new Quaternion(0, 0, 0, 0);
            GameObject prefab = Instantiate(spawnPrefab, prePos, spwanQt);
            prefab.transform.SetParent(mainBlock.transform);

            blockNum++;
            blockNumText.text = "" + blockNum;
        }
    }
}
