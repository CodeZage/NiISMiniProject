using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public List<GameObject> cameraUsedForScenes;
    [SerializeField]private List<Vector3> cameraPosition;
    [SerializeField]private List<Vector3> camerRotation;
    private GameObject _camera;

    [SerializeField] private int cameraPositionIndex = 0;
    private int currentCameraIndex = 0;
    private float timer = 0;

    public AudioManager _audioManager;

    private void Start()
    {
        
        //Finds the initial camera, and stores all the transforms at the proper places. 
        _camera = GameObject.Find("Main Camera");
        //camera.GetComponent<Camera>().enabled = true;
        cameraUsedForScenes.Insert(0, _camera);
        for (int i = 0; i < cameraUsedForScenes.Count; i++)
        {
            //cameraUsedForScenes[i].GetComponent<Camera>().enabled = false;
            cameraPosition.Add(cameraUsedForScenes[i].transform.position);
            camerRotation.Add(cameraUsedForScenes[i].transform.eulerAngles);
        }
    }

    private void Update() //Lerps between precious and current index when not at current index. 
    {
        if(currentCameraIndex == cameraPositionIndex) return;
        
        if (_camera.transform.position != cameraPosition[cameraPositionIndex] &&
            _camera.transform.eulerAngles != camerRotation[cameraPositionIndex]){
            timer += Time.deltaTime;
            _camera.transform.position = Vector3.Lerp(cameraPosition[currentCameraIndex],
                cameraPosition[cameraPositionIndex], timer);
            
            _camera.transform.eulerAngles = Vector3.Lerp(camerRotation[currentCameraIndex],
                camerRotation[cameraPositionIndex], timer);
            
        }
        else
        {
            currentCameraIndex = cameraPositionIndex;
            timer = 0;
            _audioManager.PlayNextTrack();
        }
    }

    public void NextScene()
    {
        SetCameraPositionIndex(currentCameraIndex + 1);
    }

    public void SetCameraPositionIndex(int index) //Used to switch between camera positions
    {
        cameraPositionIndex = index;
    }
    
}
