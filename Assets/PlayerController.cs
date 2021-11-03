using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private GameObject _player;
    private GameObject _rangeSquare;
    private GameObject _rangeCircle;
    private Rigidbody2D _rb;
    private float _originalX;
    private float _originalY;
    private bool _isMoving;
    private bool _isShowingRange;
    private bool _turnInProgress;

    private Vector2 _point;

    public float speed = 1;
    public float range = 2;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rangeSquare = GameObject.Find("Range");
        _rangeCircle = GameObject.Find("RangeRenderer");
        _player = GameObject.Find("Bird Token Grey");
        _turnInProgress = false;
        OnNewTurn();
        RangeViewToggle(true);
        _isMoving = false;
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
        var position = _player.transform.position;
        _originalX = position.x;
        _originalY = position.y;
        _rangeSquare.transform.position = new Vector2(_originalX, _originalY);
        _rangeCircle.transform.position = new Vector2(_originalX, _originalY);
    }

    private void OnDragAndMove()
    {
        if (_isMoving) return;
        var mousePos = Mouse.current.position.ReadValue();
        if (Camera.main is null) return;
        var mouseCalcPos = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));
        if (!CheckMove(mouseCalcPos)) return;
        _point = mouseCalcPos;
        _isMoving = true;
    }

    private void OnCollisionStay2D()
    {
        StopMoving();
    }

    private bool CheckMove(Vector3 mouseCalcPos)
    {
        var deltaX = Math.Abs(_originalX - mouseCalcPos.x);
        var deltaY = Math.Abs(_originalY - mouseCalcPos.y);
        //deltaX = (float)Math.Round(deltaX, 0);
        //deltaY = (float)Math.Round(deltaY, 0);
        if (deltaX > range || deltaY > range) return false;
        return !(deltaX < range * -1f) && !(deltaY < range * -1);
    }

    private void FixedUpdate()
    {
        var ranger = GameObject.Find("Range");
        ranger.transform.localScale = new Vector3(range * 2, range * 2, 1f);
    }

    
    private void Update()
    {
        if (!_isMoving) return;
        var position = _player.transform.position;
        position = Vector3.MoveTowards(position, _point, Time.deltaTime * speed);
        _player.transform.position = position;
        if (Math.Abs(position.x - _point.x) < 0.00001f && Math.Abs(position.y - _point.y) < 0.00001f) _isMoving = false;
    }

    private void StopMoving()
    {
        _isMoving = false;
    }

    private void RangeViewToggle(bool turnOn)
    {
        if (turnOn)
        {
            _rangeSquare.GetComponent<SpriteRenderer>().color = new Color(80 / 255f, 240 / 255f, 100 / 255f, 25 / 255f);
            _isShowingRange = true;
        }
        else
        {
            _rangeSquare.GetComponent<SpriteRenderer>().color = new Color(80 / 255f, 240 / 255f, 100 / 255f, 0f);
            _isShowingRange = false;
        }
    }
}
