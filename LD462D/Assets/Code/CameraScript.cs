using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Camera camera_;

    public GameObject[] objectsOnScreen; 

    private float lastZoomBabyTime_ = float.MinValue;

    private int zoomIndex_;
    private float originalZoom_;
    private float originalX_;
    private float originalY_;

    void Awake()
    {
        camera_ = GetComponent<Camera>();
        originalZoom_ = camera_.orthographicSize;
        originalX_ = transform.position.x;
        originalY_ = transform.position.y;
    }

    public void ZoomObject(int index)
    {
        lastZoomBabyTime_ = Time.time;
        zoomIndex_ = index;
    }

    void Update()
    {
        if (Time.time - lastZoomBabyTime_ < 0.25f)
        {
            camera_.orthographicSize = 5;
            camera_.transform.position = new Vector3(objectsOnScreen[zoomIndex_].transform.position.x, objectsOnScreen[zoomIndex_].transform.position.y, -10);  
            return;
        }
        else 
        {
            camera_.transform.position = new Vector3(originalX_,originalY_, -10);  
        }

        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;
        
        foreach (var obj in objectsOnScreen)
        {
            if (obj.transform.position.x < minX)
                minX = obj.transform.position.x;
            if (obj.transform.position.x > maxX)
                maxX = obj.transform.position.x;
            if (obj.transform.position.y < minY)
                minY = obj.transform.position.y;
            if (obj.transform.position.y > maxY)
                maxY = obj.transform.position.y;
        }
        
        float xDiff;// = maxX - minX;
        if (Mathf.Abs(maxX) > Mathf.Abs(minX))
            xDiff = Mathf.Abs(maxX);
        else 
            xDiff = Mathf.Abs(minX);

        float yDiff;// = maxY - minY;
        if (Mathf.Abs(maxY) > Mathf.Abs(minY))
            yDiff = Mathf.Abs(maxY);
        else 
            yDiff = Mathf.Abs(minY);

        xDiff /=2;

        float largestDiff; 
        if (xDiff > yDiff)
            largestDiff = xDiff;
        else 
            largestDiff = yDiff;

        camera_.orthographicSize = Mathf.Max(largestDiff*1.2f, originalZoom_);  
    }
}
