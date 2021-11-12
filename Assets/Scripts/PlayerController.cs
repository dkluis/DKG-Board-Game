using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private GameObject _player;
    private GameObject _rangeSquare;
    private GameObject _ranger;
    private GameObject _ranger2;
    private GameObject _ranger3;

    //private Rigidbody2D _rb;
    private float _originalX;
    private float _originalY;
    private bool _isMoving;
    private bool _isShowingRange;
    private bool _turnInProgress;

    private Vector2 _point;

    public int speed = 5;
    public int range = 2;
    private int _currentRange;

    private void Start()
    {
        //_rb = GetComponent<Rigidbody2D>();
        _rangeSquare = GameObject.Find("Range");
        _ranger = GameObject.Find("Grid-Range");
        _ranger2 = GameObject.Find("Grid-Range-2");
        _ranger3 = GameObject.Find("Grid-Range-3");
        
        _player = GameObject.Find("Bird Token Grey");
        _turnInProgress = false;
        OnNewTurn();
        RangeViewToggle(true);
        _currentRange = 1;
        _ranger2.SetActive(false);
        _ranger3.SetActive(false);
        _isMoving = false;
    }

    [UsedImplicitly]
    private void OnRange()
    {
        RangeViewToggle(!_isShowingRange);
    }

    [UsedImplicitly]
    private void OnStopMoving()
    {
        if (_isMoving) StopMoving();
    }

    [UsedImplicitly]
    private void OnTurnOff()
    {
        if (_isMoving) StopMoving();
        _turnInProgress = false;
    }

    [UsedImplicitly]
    private void OnMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    private void OnNewTurn()
    {
        if (_isMoving) return;
        if (_turnInProgress) return;
        _turnInProgress = true;
        var position = _player.transform.position;
        print($"Changing Original Pos from ({_originalX},{_originalY}) to ({position.x},{position.y})");
        _originalX = position.x;
        _originalY = position.y;
        _rangeSquare.transform.position = new Vector2(_originalX, _originalY);
        _ranger.transform.position = new Vector2(_originalX, _originalY);
    }

    [UsedImplicitly]
    private void OnDragAndMove()
    {
        if (_isMoving) return;
        var mousePos = Mouse.current.position.ReadValue();
        if (Camera.main is null) return;
        var mouseCalcPos = Camera.main.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));
        CheckMove(mouseCalcPos);
    }

    private void OnCollisionStay2D()
    {
        StopMoving();
        //ToDo correct the final position
    }

    private void CheckMove(Vector3 mouseCalcPos)
    {
        /*
        var deltaX = Math.Abs(_originalX - mouseCalcPos.x);
        var deltaY = Math.Abs(_originalY - mouseCalcPos.y);
        if (deltaX > range || deltaY > range) return;
        if (deltaX < range * -1f || deltaY < range * -1f) return;
        */
        //Valid Move - Setup the new Coordinates to MoveTo
        var signedDeltaX = _originalX + mouseCalcPos.x;
        var signedDeltaY = _originalY + mouseCalcPos.y;
        var roundX = (float) Math.Round(signedDeltaX, 0);
        var roundY = (float) Math.Round(signedDeltaY, 0);
        //print($"Rounding ({deltaX}, {deltaY}) - ({roundX - _originalX}, {roundY - _originalY})");
        _point.x = roundX - _originalX;
        _point.y = roundY - _originalY;
        // Ranger Calculations
        if ((Math.Abs(_point.x) + Math.Abs(_point.y)) - (Math.Abs(_originalX) + Math.Abs(_originalY)) > range)
        {
            print($"Not allowed to move here, breaking the grid rules");
            return;
        }
        _isMoving = true;
    }

    private void FixedUpdate()
    {
        var ranger = GameObject.Find("Range");
        ranger.transform.localScale = new Vector3(range * 2, range * 2, 1f);
    }


    private void Update()
    {
        if (_currentRange != range)
        {
            if (range > 3 || range < 1) range = 1;
            RangeViewToggle(true);
            _currentRange = range;
        }
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
        _ranger.SetActive(false);
        _ranger2.SetActive(false);
        _ranger3.SetActive(false);
        _isShowingRange = false;
        if (turnOn)
        {
            switch (range)
            {
                case 2:
                    _ranger2.SetActive(true);
                    break;
                case 3:
                    _ranger2.SetActive(true);
                    _ranger3.SetActive(true);
                    break;
            }
            _ranger.SetActive(true);
            //_ranger.GetComponent<SpriteRenderer>().color = new Color(80 / 255f, 240 / 255f, 100 / 255f, 25 / 255f);
            _isShowingRange = true;
        }
        else
        {
            //_ranger.GetComponent<SpriteRenderer>().color = new Color(80 / 255f, 240 / 255f, 100 / 255f, 0f);
        }
    }
}