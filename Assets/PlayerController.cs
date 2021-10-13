using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    GameObject player;
    GameObject rangeCircle;
    private Rigidbody2D rb;
    private float OriginalX;
    private float OriginalY;
    private Transform tfCurrent;

    private float movementX;
    private float movementY;
    bool isMoving;
    bool isShowingRange;

    public float speed = 1;
    public float range = 2;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rangeCircle = GameObject.Find("Range");
        player = GameObject.Find("Bird Token Grey");
        OriginalX = player.GetComponent<Transform>().position.x;
        OriginalY = player.GetComponent<Transform>().position.y;
        rangeCircle.transform.position = new Vector2(OriginalX, OriginalY);
        rangeCircle.GetComponent<SpriteRenderer>().color = new Color(80/255f, 240/255f, 100/255f, 0f);
        isShowingRange = false;
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
        rangeCircle.GetComponent<SpriteRenderer>().color = new Color(80 / 255f, 240 / 255f, 100 / 255f, 25/255f);
        isShowingRange = true;
        //print($"onMove X: {movementX}, Y: {movementY}");

        if (!isMoving)
        {
            isMoving = true;
            OriginalX = player.GetComponent<Transform>().position.x;
            OriginalY = player.GetComponent<Transform>().position.y;
            //print($"Not isMoving = onMove X Pos: {OriginalX}, Y Pos: {OriginalY}");
        }
    }

    void OnRange()
    {
        //print("onRange Activated");
        if (isShowingRange) { rangeCircle.GetComponent<SpriteRenderer>().color = new Color(80 / 255f, 240 / 255f, 100 / 255f, 0f); isShowingRange = false; }
        else { rangeCircle.GetComponent<SpriteRenderer>().color = new Color(80 / 255f, 240 / 255f, 100 / 255f, 25/255f); isShowingRange = true; }

    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, movementY);
        rb.AddForce(movement * speed);
    }

    private void Update()
    {
        tfCurrent = player.GetComponent<Transform>();
        //print($"Update Player X Pos: {tfCurrent.position.x}, Y Pos: {tfCurrent.position.y}");

        if (RangeExceeded())
        {
            rb.velocity = Vector2.zero;
            isMoving = false;
            //print($"#################################################### Should stop moving#################################################");
            print("Your Turn is Over");
            OriginalX = player.GetComponent<Transform>().position.x;
            OriginalY = player.GetComponent<Transform>().position.y;
            rangeCircle.transform.position = new Vector2(OriginalX, OriginalY);
            rangeCircle.GetComponent<SpriteRenderer>().color = new Color(80 / 255f, 240 / 255f, 100 / 255f, 0f);
            isShowingRange = false;
        }
    }

    private bool RangeExceeded()
    {
        float deltaX = Math.Abs(tfCurrent.position.x - OriginalX);
        float deltaY = Math.Abs(tfCurrent.position.y - OriginalY);

        if (tfCurrent.position.x - OriginalX < 0) { deltaX += 0.5f; } else { deltaX += 0.5f; }
        if (tfCurrent.position.y - OriginalY < 0) { deltaY += 0.5f; } else { deltaY += 0.5f; }

        if (deltaX >= range) { return true; }
        if (deltaY >= range) { return true; }
        return false;
    }
}
