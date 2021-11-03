using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private GameObject _player;
    private GameObject _rangeCircle;
    private Rigidbody2D _rb;
    private float _originalX;
    private float _originalY;
    private Transform _tfCurrent;

    private float _movementX;
    private float _movementY;
    private bool _isMoving;
    private bool _isShowingRange;
    private bool _turnInProgress;

    private Vector2 _point;

    public float speed = 1;
    public float range = 2;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rangeCircle = GameObject.Find("Range");
        _player = GameObject.Find("Bird Token Grey");
        _turnInProgress = false;
        OnNewTurn();
        RangeViewToggle(false);
        _isMoving = false;
    }

    private void OnMove(InputValue movementValue)
    {
        if (!_turnInProgress) return;
        var movementVector = movementValue.Get<Vector2>();
        _movementX = movementVector.x;
        _movementY = movementVector.y;
        RangeViewToggle(true);
        if (_isMoving) return;
        _isMoving = true;
    }

    private void OnRange()
    {
        RangeViewToggle(!_isShowingRange);
    }

    private void OnStopMoving()
    {
        if (_isMoving) StopMoving();
    }

    private void OnTurnOff()
    {
        if (_isMoving) StopMoving(); 
        _turnInProgress = false;
    }

    private void OnNewTurn()
    {
        if (_isMoving) return;
        if (_turnInProgress) return;
        _turnInProgress = true;
        _originalX = _player.GetComponent<Transform>().position.x;
        _originalY = _player.GetComponent<Transform>().position.y;
        _rangeCircle.transform.position = new Vector2(_originalX, _originalY);
    }

    private void OnDragAndMove()
    {
        //if (_isMoving) return;
        var mousePos = Mouse.current.position.ReadValue();
        var mouseCalcPos = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));
        print($"Mouse ({mouseCalcPos.x}, {mouseCalcPos.y}, {mouseCalcPos.z})");
        var playerCurrentPos = _player.transform.position;
        print($"Player ({playerCurrentPos.x}, {playerCurrentPos.y})");
        var deltaX = Math.Abs(playerCurrentPos.x - mouseCalcPos.x);
        var deltaY = Math.Abs(playerCurrentPos.y - mouseCalcPos.y);
        print($"Deltas ({deltaX}, {deltaY})");
        if (deltaX > range || deltaY > range) return;
        if (deltaX < range * -1f || deltaY < range * -1) return;
        _point = mouseCalcPos;
        //_isMoving = true;
    }

    private void FixedUpdate()
    {
        if (CheckIfRangeWillBeExceeded()) { StopMoving(); return; }
        var movement = new Vector3(_movementX, _movementY);
        _rb.AddForce(movement * speed);
        GameObject ranger = GameObject.Find("Range");
        ranger.transform.localScale = new Vector3(range * 2, range * 2, 1f);
    }

    
    private void Update()
    {
        _player.transform.position = Vector3.MoveTowards(_player.transform.position, _point, Time.deltaTime * speed);
        _rangeCircle.transform.position = _point;
    }

    private bool RangeExceeded()
    {
        _tfCurrent = _player.GetComponent<Transform>();
        var position = _tfCurrent.position;
        var deltaX = Math.Abs(position.x - _originalX);
        var deltaY = Math.Abs(position.y - _originalY);
        if (_tfCurrent.position.x - _originalX < 0) { deltaX += 0.5f; } else { deltaX += 0.5f; }
        if (_tfCurrent.position.y - _originalY < 0) { deltaY += 0.5f; } else { deltaY += 0.5f; }
        return deltaX >= range || deltaY >= range;
    }

    private bool CheckIfRangeWillBeExceeded()
    {
        _tfCurrent = _player.GetComponent<Transform>();
        if (Math.Abs(_tfCurrent.position.x - _originalX) + 0.5f >= range)
        {
            if (_tfCurrent.position.x > 0 && _movementX > 0) return true;
            else
            if (_tfCurrent.position.x < 0 && _movementX < 0) return true;
        }
        if (!(Math.Abs(_tfCurrent.position.y - _originalY) + 0.75f >= range)) return false;
        if (_tfCurrent.position.y > 0 && _movementY > 0) return true;
        return _tfCurrent.position.y < 0 && _movementY < 0;
    }

    private void StopMoving()
    {
        _rb.velocity = Vector2.zero;
        _isMoving = false;
    }

    private void RangeViewToggle(bool turnOn)
    {
        if (turnOn)
        {
            _rangeCircle.GetComponent<SpriteRenderer>().color = new Color(80 / 255f, 240 / 255f, 100 / 255f, 25 / 255f);
            _isShowingRange = true;
        }
        else
        {
            _rangeCircle.GetComponent<SpriteRenderer>().color = new Color(80 / 255f, 240 / 255f, 100 / 255f, 0f);
            _isShowingRange = false;
        }
    }
}
