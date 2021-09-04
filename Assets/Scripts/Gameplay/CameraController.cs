using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private Camera cam;
    [SerializeField] private float zoomStep, minCameraSize, maxCameraSize;
    

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
        MoveCamera();

    }

    public void UpdateMapSize(float minY, float maxY)
    {
        maxCameraSize = maxY;
        mapMinY = minY;
        mapMaxY = maxY;
        mapMinX = mapMinY * cam.aspect;
        mapMaxX = mapMaxY * cam.aspect;
    }

    public void MoveCamera()
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
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            //Debug.Log(Input.mouseScrollDelta.y);
            float newSize = cam.orthographicSize - Input.mouseScrollDelta.y * Time.deltaTime * zoomStep;
            cam.orthographicSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);

            cam.transform.position = ClampCamera(cam.transform.position);
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
