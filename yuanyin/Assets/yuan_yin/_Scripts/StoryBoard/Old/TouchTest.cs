using System.Collections.Generic;
using UnityEngine;

public class TouchTest : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void OnGUI()
    {
        Vector3 point2 = new Vector3(); //鼠标
        Event currentEvent = Event.current;
        Vector2 mousePos = new Vector2();
        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;
        point2 = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));


        Vector3 point = new Vector3(); //触摸
        Touch touch = new Touch();
        Vector2 touchPos = new Vector2();

        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            touchPos = touch.rawPosition;
            point = cam.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, cam.nearClipPlane));
        }


        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Screen pixels: " + cam.pixelWidth + ":" + cam.pixelHeight);
        GUILayout.Label("Touch position: " + touchPos);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + point.ToString("F3"));
        GUILayout.Label("World position - Mouse: " + point2.ToString("F3"));
        GUILayout.EndArea();
    }
}