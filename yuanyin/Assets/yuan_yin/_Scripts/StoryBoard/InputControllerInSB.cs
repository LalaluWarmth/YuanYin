using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputControllerInSB : MonoBehaviour
{
    private static InputControllerInSB _instance;

    public static InputControllerInSB GetInstance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType(typeof(InputControllerInSB)) as InputControllerInSB;
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                obj.hideFlags = HideFlags.HideAndDontSave;
                _instance = obj.AddComponent(typeof(InputControllerInSB)) as InputControllerInSB;
            }
        }

        return _instance;
    }

    public GameObject canvas;

    void Start()
    {
        CheckDeviceType();
    }


    void Update()
    {
        CheckInput();
    }

    private void CheckDeviceType() // CheckDeviceType
    {
#if UNITY_EDITOR
        Debug.Log("Unity Editor");
#endif

#if UNITY_IOS
        Debug.Log("Iphone");
#endif

#if UNITY_STANDALONE_OSX
        Debug.Log("Stand Alone OSX");
#endif

#if UNITY_STANDALONE_WIN
        Debug.Log("Stand Alone Windows");
#endif
    }

    private void CheckInput() // CheckInput
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(i).phase == TouchPhase.Began))
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                {
                    GameObject targetGO = EventSystem.current.currentSelectedGameObject;
                    // Debug.Log(targetGO);
                    Note targetNote = targetGO.GetComponent<Note>();
                    if (targetNote)
                    {
                        targetNote.Onhit();
                    }
                }
                else
                    Debug.Log("No UI On Touch");
            }

            if (Input.touchCount > 0 && Input.GetTouch(i).phase == TouchPhase.Moved)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                {
                    PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                    pointerEventData.position = Input.mousePosition;
                    GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();
                    List<RaycastResult> results = new List<RaycastResult>();
                    gr.Raycast(pointerEventData, results);
                    foreach (var item in results)
                    {
                        GameObject targetGO = item.gameObject;
                        Note targetNote = targetGO.GetComponent<Note>();
                        if (targetNote && targetNote.CompareTag("Xinggui"))
                        {
                            targetNote.Onhit();
                        }
                    }
                }
                else
                {
                    Debug.Log("Not Moving On sth");
                }
            }
        }
    }
}