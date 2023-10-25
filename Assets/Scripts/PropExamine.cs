﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropExamine : MonoBehaviour
{
    public Vector3 smallPos;
    public Vector3 smallRot;
    public Vector3 smallScale;
    public Vector3 bigPos;
    public Vector3 bigRot;
    public Vector3 bigScale;
    public Texture2D cursor;
    //private float speed = 6f;

    private Vector3 targetPos;
    private Vector3 targetRot;
    private Vector3 targetScale;
    private bool isBig;

    private void Start()
    {
        transform.localPosition = smallPos;
        transform.eulerAngles = smallRot;
        transform.localScale = smallScale;
        targetScale = transform.localScale;
        isBig = false;
    }

    private void Update()
    {
        
    }

    private void OnMouseDown()
    {
        // Toggle the target scale on mouse click
        if (Time.timeScale != 0f)
            ToggleSize();

        if (transform.localScale != targetScale)
        {
            // Transform to the new position/scale over a certain time, set rotation (Lerp causes it to spin weirdly)
            transform.position = targetPos; //Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
            transform.eulerAngles = targetRot;
            transform.localScale = targetScale; //Vector3.Lerp(transform.localScale, targetScale, speed * Time.deltaTime);
        }
    }

    void OnMouseEnter()
    {
        if (Time.timeScale != 0f)
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
    }

    private void ToggleSize()
    {
        // Toggle the "isBig" bool
        isBig = !isBig;
        // If isBig, first condition (big pos/rot/scale), if !isBig, second condition (small pos/rot/scale)
        Vector3 position = isBig ? bigPos : smallPos;
        Vector3 rotation = isBig ? bigRot : smallRot;
        Vector3 scale = isBig ? bigScale : smallScale;
        // Set the target for next click
        targetPos = position;
        targetRot = rotation;
        targetScale = scale;
    }
    
}
