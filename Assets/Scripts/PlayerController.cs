using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private GameObject _player;
    private GameObject _player1;
    private GameObject _activePlayer;
    private float _originalX;
    private float _originalY;
    private bool _isMoving;
    private bool _turnInProgress;

    private Vector2 _point;
    public int speed = 5;
    public int range;
    private int _currentRange;

    private Colliders _colliders;
    private BoardCoordinates _board;
    private Rangers _rangeLocations;

    private void Start()
    {
        _player = GameObject.Find("Bird Token Grey");
        _player1 = GameObject.Find("Bird Token Grey 1");
        _activePlayer = _player;
        _colliders = new Colliders();
        _board = new BoardCoordinates();
        _rangeLocations = new Rangers(_board, _colliders);
        _turnInProgress = false;
        _currentRange = 2;
        _isMoving = false;
        OnNewTurn();
    }

    [UsedImplicitly]
    private void OnRange()
    {
        //RangeViewToggle(!_isShowingRange);
        _colliders.Shuffle(_activePlayer.transform.position);
        _rangeLocations.Refill(_activePlayer);
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
        var position = _activePlayer.transform.position;
        print($"Changing Original Pos from ({_originalX},{_originalY}) to ({position.x},{position.y})");
        _originalX = position.x;
        _originalY = position.y;
        if (_currentRange != range)
        {
            if (range > 3 || range < 1) range = 1;
            _currentRange = range;
        }

        _activePlayer = _activePlayer.name == _player.name ? _player1 : _player;
        _rangeLocations.Refill(_activePlayer);
        _colliders.ReFill(_activePlayer.transform.position);
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
    }

    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    private void CheckMove(Vector3 mouseCalcPos)
    {
        _isMoving = false;
        var roundX = (float) Math.Round(_originalX + mouseCalcPos.x, 0);
        var roundY = (float) Math.Round(_originalY + mouseCalcPos.y, 0);
        _point.x = roundX - _originalX;
        _point.y = roundY - _originalY;
        var locToCheck = new Vector2(_point.x, _point.y);
        Debug.Log($"Distance to {_point.x},{_point.y} is: {Rangers.CalculateStepBetweenCoordinates(new Vector2(_originalX, _originalY), locToCheck)}");
        var gp = _rangeLocations.CheckRangerPoint(locToCheck);
        var cc = _colliders.CheckIfColliderPoint(locToCheck);
        var bp = _board.IsValidBoardPoint(locToCheck);
        if (!gp || cc || !bp) return;
        _isMoving = true;
    }

    private void Update()
    {
        if (!_isMoving) return;
        _activePlayer.transform.position = _point;
        _isMoving = false;
    }

    private void StopMoving()
    {
        _isMoving = false;
    }
}