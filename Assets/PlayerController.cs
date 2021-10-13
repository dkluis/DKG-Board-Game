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
    bool turnInProgress;

    public float speed = 1;
    public float range = 2;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rangeCircle = GameObject.Find("Range");
        player = GameObject.Find("Bird Token Grey");
        turnInProgress = false;
        OnNewTurn();
        RangeViewToggle(false);
        isMoving = false;
    }

    void OnMove(InputValue movementValue)
    {
        if (!turnInProgress) { return; }

        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
        RangeViewToggle(true);
        if (!isMoving)
        {
            isMoving = true;
        }

    }

    void OnRange()
    {
        //print("onRange Activated");
        if (isShowingRange) { RangeViewToggle(false); } else { RangeViewToggle(true); }

    }

    private void OnStopMoving()
    {
        //print("onStop Activated");
        if (isMoving) { StopMoving(); }
    }

    private void OnTurnOff()
    {
        print("OnTurnOff activated");
        if (isMoving) { StopMoving(); }
        turnInProgress = false;
    }

    private void OnNewTurn()
    {
        if (isMoving) { return; }
        if (turnInProgress) { return; }
        //print("New Turn Activiated");
        turnInProgress = true;
        OriginalX = player.GetComponent<Transform>().position.x;
        OriginalY = player.GetComponent<Transform>().position.y;
        rangeCircle.transform.position = new Vector2(OriginalX, OriginalY);
    }

    void FixedUpdate()
    {
        if (CheckIfRangeWillBeExceeded()) { StopMoving();  return; };

        Vector3 movement = new Vector3(movementX, movementY);
        rb.AddForce(movement * speed);
    }

    private void Update()
    {
        if (RangeExceeded() || !turnInProgress)
        {
            if (isMoving) { StopMoving(); }
        }
    }

    private bool RangeExceeded()
    {
        tfCurrent = player.GetComponent<Transform>();
        float deltaX = Math.Abs(tfCurrent.position.x - OriginalX);
        float deltaY = Math.Abs(tfCurrent.position.y - OriginalY);

        if (tfCurrent.position.x - OriginalX < 0) { deltaX += 0.5f; } else { deltaX += 0.5f; }
        if (tfCurrent.position.y - OriginalY < 0) { deltaY += 0.5f; } else { deltaY += 0.5f; }

        if (deltaX >= range) { return true; }
        if (deltaY >= range) { return true; }

        return false;
    }

    private bool CheckIfRangeWillBeExceeded()
    {
        tfCurrent = player.GetComponent<Transform>();
        //print($"Check if Range will be Exceeded");
        if (Math.Abs(tfCurrent.position.x + movementX - OriginalX + 0.5f) >= range) { return true; }
        if (Math.Abs(tfCurrent.position.y + movementY - OriginalY + 0.5f) >= range) { return true; }

        return false;
    }

    private void StopMoving()
    {
        rb.velocity = Vector2.zero;
        isMoving = false;
    }

    private void RangeViewToggle(bool TurnOn)
    {
        if (TurnOn) { rangeCircle.GetComponent<SpriteRenderer>().color = new Color(80 / 255f, 240 / 255f, 100 / 255f, 25 / 255f); isShowingRange = true; }
        else { rangeCircle.GetComponent<SpriteRenderer>().color = new Color(80 / 255f, 240 / 255f, 100 / 255f, 0f); isShowingRange = false; }
    }
}
