using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
public class PlayerController : MonoBehaviour
{
    private GameObject _token1;
    private GameObject _token2;
    private GameObject _activeToken;
    private int _activeTokenInt;
    private int _numOfTurns;
    private UnityEngine.UI.Text _token1Text;
    private UnityEngine.UI.Text _token2Text;
    private UnityEngine.UI.Text _statusText;
    private string _status;
    private float _originalX;
    private float _originalY;
    private bool _tokenHasPlayed;
    private bool _isMoving;
    private bool _turnInProgress;

    private Vector2 _point;
    public int speed;
    public int range;
    private int _currentRange;

    private Colliders _colliders;
    private BoardCoordinates _board;
    private Rangers _rangeLocations;

    private void Start()
    {
        _token1 = GameObject.Find("Bird Token Grey");
        _token2 = GameObject.Find("Bird Token Grey 1");
        _token1Text = GameObject.Find("Token1").GetComponent<UnityEngine.UI.Text>();
        _token2Text = GameObject.Find("Token2").GetComponent<UnityEngine.UI.Text>();
        _statusText = GameObject.Find("Status").GetComponent<UnityEngine.UI.Text>();
        _activeToken = _token1;
        _activeTokenInt = 1;
        _numOfTurns = 0;
        _colliders = new Colliders();
        _board = new BoardCoordinates();
        _rangeLocations = new Rangers(_board, _colliders);
        _turnInProgress = false;
        _currentRange = 2;
        _isMoving = false;
        _tokenHasPlayed = false;
        OnNewTurn();
    }

    [UsedImplicitly]
    private void OnRange()
    {
        //_colliders.Shuffle(_activeToken.transform.position);
        //_rangeLocations.Refill(_activeToken);
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
        // ToDo add code for finalizing turn
        _turnInProgress = false;
        _tokenHasPlayed = false;
        UpdateStatus("Turn Over");
    }

    [UsedImplicitly]
    private void OnMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void UpdateStatus(string status = "")
    {
        if (_activeTokenInt == 1)
        {
            _token1Text.text = "Active";
            _token2Text.text = "InActive";
        }
        else
        {
            _token1Text.text = "InActive";
            _token2Text.text = "Active";
        }

        _statusText.text = $"Turn: {_numOfTurns} || G: {_token1Text.text} || O: {_token2Text.text} || Status: {status}";
    }

    private void OnNewTurn()
    {
        if (_turnInProgress) return;
        _turnInProgress = true;
        _numOfTurns++;
        UpdateStatus();

        var position = _activeToken.transform.position;
        _originalX = position.x;
        _originalY = position.y;
        if (_currentRange != range)
        {
            if (range > 3 || range < 1) range = 1;
            _currentRange = range;
        }

        _tokenHasPlayed = false;
        _colliders.Shuffle(_activeToken.transform.position);
        _rangeLocations.Refill(_activeToken);
        //_colliders.ReFill(_activeToken.transform.position);
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
        var curPos = _activeToken.transform.position;
        var roundX = (float) Math.Round(_originalX + mouseCalcPos.x, 0);
        var roundY = (float) Math.Round(_originalY + mouseCalcPos.y, 0);
        var x = roundX - _originalX;
        var y = roundY - _originalY;
        var locToCheck = new Vector2(x, y);

        if (CheckIfOtherTokenIsClicked(locToCheck) && !_tokenHasPlayed)
        {
            _activeToken = _activeToken.name == _token1.name ? _token2 : _token1;
            _activeTokenInt = _activeTokenInt == 1 ? 2 : 1;
            _rangeLocations.Refill(_activeToken);
            _tokenHasPlayed = true;
            UpdateStatus("Switched Token");
        }

        var gp = _rangeLocations.CheckRangerPoint(locToCheck);
        var cc = _colliders.CheckIfColliderPoint(locToCheck);
        var bp = _board.IsValidBoardPoint(locToCheck);

        if (!gp || !bp)
        {
            if (cc)
            {
                UpdateStatus($"You Hit a BadGuy at {locToCheck}");
                return;
            }
            UpdateStatus($"Invalid Move!! to {locToCheck}");
            return;
        }
        
        _point.x = x;
        _point.y = y;
        UpdateStatus($"Moved to Coordinate ({_point.x},{_point.y})");
        _isMoving = true;
    }

    private void Update()
    {
        if (_isMoving) _activeToken.transform.position = _point;
        _isMoving = false;
        /*
        var position = _activeToken.transform.position;
        position = Vector3.MoveTowards(position, _point, Time.deltaTime * speed);
        _activeToken.transform.position = position;
        if (Math.Abs(position.x - _point.x) < 0.00001 && Math.Abs(position.y - _point.y) < 0.00001) _isMoving = false;
        */
    }

    private void StopMoving()
    {
        _isMoving = false;
    }

    private bool CheckIfOtherTokenIsClicked(Vector2 clickedLocation)
    {
        var gOList = GameObject.FindGameObjectsWithTag("Player");
        foreach (var gO in gOList)
        {
            if ((Vector2) gO.transform.position != clickedLocation) continue;
            if (gO.name != _activeToken.name)
                return true;
        }

        return false;
    }
}