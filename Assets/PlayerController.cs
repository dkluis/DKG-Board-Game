using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    GameObject player;
    private Rigidbody2D rb;
    private float OriginalX;
    private float OriginalY;
    private Transform tfCurrent;

    private float movementX;
    private float movementY;
    bool isMoving;

    public float speed = 1;
    public float range = 2;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Bird Token Grey");
        OriginalX = player.GetComponent<Transform>().position.x;
        OriginalY = player.GetComponent<Transform>().position.y;
        
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
        //Debug.Log($"onMove X: {movementX}, Y: {movementY}");

        if (!isMoving)
        {
            isMoving = true;
            OriginalX = player.GetComponent<Transform>().position.x;
            OriginalY = player.GetComponent<Transform>().position.y;
            //Debug.Log($"Not isMoving = onMove X Pos: {OriginalX}, Y Pos: {OriginalY}");
        }
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, movementY);
        rb.AddForce(movement * speed);
    }

    private void Update()
    {
        tfCurrent = player.GetComponent<Transform>();
        //Debug.Log($"Update Player X Pos: {tfCurrent.position.x}, Y Pos: {tfCurrent.position.y}");

        if (RangeExceeded())
        {
            rb.velocity = Vector2.zero;
            isMoving = false;
            //Debug.Log($"#################################################### Should stop moving#################################################");
            print("Your Turn is Over");
            OriginalX = player.GetComponent<Transform>().position.x;
            OriginalY = player.GetComponent<Transform>().position.y;
        }
    }

    private bool RangeExceeded()
    {
        float deltaX = Math.Abs(tfCurrent.position.x - OriginalX);
        float deltaY = Math.Abs(tfCurrent.position.y - OriginalY);
        //Debug.Log($"RangeExceeded: Deltas X {deltaX}, Y {deltaY}");

        if (deltaX >= range) { return true; }
        if (deltaY >= range) { return true; }
        return false;
    }
}
