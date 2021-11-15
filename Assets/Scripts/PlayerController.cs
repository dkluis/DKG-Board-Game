using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private GameObject _player;
    private float _originalX;
    private float _originalY;
    private bool _isMoving;
    private bool _isShowingRange;
    private bool _turnInProgress;

    private Vector2 _point;
    public int speed = 5;
    public int range;
    private int _currentRange;
    // const int maxX = 10;
    // const int maxY = 6;
    
    private readonly BoardLocations _boardLocations = new BoardLocations();
    private readonly BoardLocations _board = new BoardLocations();
    private readonly BoardLocations _rangeLocations = new BoardLocations();

    private void Start()
    {
        _player = GameObject.Find("Bird Token Grey");
        _turnInProgress = false;
        _currentRange = 2;
        _isMoving = false;
        _board.GetFullBoard();
        OnNewTurn();
        RangeViewToggle(true);
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
        if (_currentRange != range)
        {
            if (range > 3 || range < 1) range = 1;
            RangeViewToggle(true);
            _currentRange = range;
        }
        _rangeLocations.GetGrid();
        _boardLocations.GetBoardLocations();
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
        _isMoving = false;
        var roundX = (float) Math.Round(_originalX + mouseCalcPos.x, 0);
        var roundY = (float) Math.Round(_originalY + mouseCalcPos.y, 0);
        _point.x = roundX - _originalX;
        _point.y = roundY - _originalY;
        print($"Round {roundX}, {roundY} Point {_point.x}, {_point.y}");
        var locToCheck = new Vector2(_point.x, _point.y);
        var gp = _rangeLocations.CheckGrid(locToCheck);
        var cc = _boardLocations.CheckGameObjects("CircleCollider", locToCheck);
        var bp = _board.CheckBoard(locToCheck);
        print($"Location to Check {locToCheck.x}, {locToCheck.y}");
        print($"Grid Point: {gp}");
        print($"Circle Collider {cc}");
        print($"Board Point {bp}");
        if (!gp) return;
        if (cc) return;
        if (!bp) return;
        _isMoving = true;
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
        _isShowingRange = false;
        if (turnOn)
        {
            switch (range)
            {
                case 2:
                    break;
                case 3:
                    break;
            }
            //_ranger.GetComponent<SpriteRenderer>().color = new Color(80 / 255f, 240 / 255f, 100 / 255f, 25 / 255f);
            _isShowingRange = true;
        }
        else
        {
            //_ranger.GetComponent<SpriteRenderer>().color = new Color(80 / 255f, 240 / 255f, 100 / 255f, 0f);
        }
    }
}