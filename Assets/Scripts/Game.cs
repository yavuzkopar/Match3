using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    Match3Skin match3;

    bool isDragging;
    Vector3 startPos;
    private void Awake()
    {
        match3.StartNewGame();
        Debug.Log("S3");
    }
    void Update()
    {
        if(match3.IsPlaying)
        {
            if(!match3.IsBusy)
            {
                HandleInput();
            }
            match3.DoWork();
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            match3.StartNewGame();
        }
    }

    private void HandleInput()
    {
        if(!isDragging && Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            isDragging= true;
        }
        else if(isDragging&& Input.GetMouseButton(0))
        {
            isDragging = match3.EveluateDrag(startPos, Input.mousePosition);
        }
        else
        {
            isDragging = false;
        }
    }
}
