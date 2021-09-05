using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private Camera cam;
    [SerializeField] private float zoomStep, minCameraSize, maxCameraSize;
    [SerializeField] private Transform rectVisualTop, rectVisualBot, rectVisualLeft, rectVisualRight;



    private Vector3 dragOrigin;

    private float mapMinX, mapMaxX, mapMinY, mapMaxY;

    // Start is called before the first frame update
    void Start()
    {
        UpdateMapSize(-20,20);
    }

    // Update is called once per frame
    void Update()
    {
        MoveAndZoomCamera();

    }

    public void UpdateMapSize(float minY, float maxY)
    {
        maxCameraSize = maxY;
        mapMinY = minY;
        mapMaxY = maxY;
        mapMinX = mapMinY * cam.aspect;
        mapMaxX = mapMaxY * cam.aspect;

        
    }

    private void CaculateVisualRect()
    {
        float height = cam.orthographicSize;
        float width = height * cam.aspect;
        rectVisualTop.localPosition = new Vector3(0, height, 100);
        rectVisualTop.localScale = new Vector3(width*2, 1,1);
        rectVisualBot.localPosition = new Vector3(0, -height, 100);
        rectVisualBot.localScale = new Vector3(width * 2, 1, 1);
        rectVisualLeft.localPosition = new Vector3(-width, 0, 100);
        rectVisualLeft.localScale = new Vector3(1, height*2,0);
        rectVisualRight.localPosition = new Vector3(width, 0, 100);
        rectVisualRight.localScale = new Vector3(1, height * 2, 0);
    }

    public void MoveAndZoomCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 diff = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            //cam.transform.position += diff;
            cam.transform.position = ClampCamera(cam.transform.position + diff);
            CaculateVisualRect();
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            //Debug.Log(Input.mouseScrollDelta.y);
            float newSize = cam.orthographicSize - Input.mouseScrollDelta.y * Time.deltaTime * zoomStep;
            cam.orthographicSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);

            cam.transform.position = ClampCamera(cam.transform.position);
            CaculateVisualRect();
        }
        
    }

    public void ZoomIn()
    {
        float newSize = cam.orthographicSize - zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);

        cam.transform.position = ClampCamera(cam.transform.position);
    }

    public void ZoomOut()
    {
        float newSize = cam.orthographicSize + zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);

        cam.transform.position = ClampCamera(cam.transform.position);
    }

    private Vector3 ClampCamera(Vector3 targetPos)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minY = mapMinY + camHeight;
        float maxY = mapMaxY - camHeight;

        float newX = Mathf.Clamp(targetPos.x,minX,maxX);
        float newY = Mathf.Clamp(targetPos.y, minY, maxY);

        return new Vector3(newX,newY, targetPos.z);

    }

}
