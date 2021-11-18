using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class Rangers
{
    private List<BoardLocation> _rangerPoints = new List<BoardLocation>();
    private int _range;
    private Vector2 _playerPosition;
    private readonly BoardCoordinates _boardPoints;
    private readonly Colliders _colliders;

    public Rangers(GameObject player, BoardCoordinates boardPoints, Colliders colliders)
    {
        _boardPoints = boardPoints;
        _colliders = colliders;
    }

    public bool CheckRangerPoint(Vector2 position)
    {
        var boardLocation = _rangerPoints.Find(brdLoc => brdLoc.Location == position);
        return boardLocation is { };
    }

    public void Refill(GameObject player)
    {
        _range = player.GetComponent<PlayerController>().range;
        _playerPosition = player.transform.position;
        ResetAllRangeIndicators();
        _rangerPoints = new List<BoardLocation>();
        Debug.Log($"Refill RangePoints from location {_playerPosition.x}, {_playerPosition.y} with Range of {_range}");
        
        // Build the Y-axis for X
        var x0 = (int) _playerPosition.x;
        var y0 = (int) _playerPosition.y;
        for (var y = y0; y <= _range + y0; y++)
        {
            var totalDistance = Math.Abs((Math.Abs(x0) + Math.Abs(y)) - (Math.Abs(x0) + Math.Abs(y0)));
            BuildRangePoint(new Vector2(x0, y), totalDistance, new Vector2(x0, y0));
        }
        for (var y = y0; y >= ((y0 - _range )); y--)
        {
            var totalDistance = Math.Abs((Math.Abs(x0) + Math.Abs(y)) - (Math.Abs(x0) + Math.Abs(y0)));
            BuildRangePoint(new Vector2(x0, y), totalDistance, new Vector2(x0, y0));
        }
        // Build the X-axis for Y
        for (var x = x0; x <= _range + x0; x++)
        {
            var totalDistance = Math.Abs((Math.Abs(x) + Math.Abs(y0)) - (Math.Abs(x0) + Math.Abs(y0)));
            BuildRangePoint(new Vector2(x, y0), totalDistance, new Vector2(x0, y0));
        }
        for (var x = x0; x >= (x0 - _range); x--)
        {
            var totalDistance = Math.Abs((Math.Abs(x) + Math.Abs(y0)) - (Math.Abs(x0) + Math.Abs(y0)));
            BuildRangePoint(new Vector2(x, y0), totalDistance, new Vector2(x0, y0));
        }
        

        // Now we can use RangePointChecker
        // Build the Y-axis -1 to -2 for y = -1 to -2
        for (var x2 = -1; x2 > _range * -1; x2--)
        {
            if (!CheckRangerPoint(new Vector2(x2, 0))) return;
            for (var y2 = -1; y2 > _range * -1; y2--)
            {
                var totalDistance = Math.Abs(x2) + Math.Abs(y2);
                BuildRangePoint(new Vector2(x2, y2), totalDistance, new Vector2(x2, 0));
            }
        }
        
        // Now we can use RangePointChecker
        // Build the Y-axis 1 to 2 for y = -1 to -2
        for (var x2 = 1; x2 < _range; x2++)
        {
            if (!CheckRangerPoint(new Vector2(x2, 0))) return;
            for (var y2 = -1; y2 > _range * -1; y2--)
            {
                var totalDistance = Math.Abs(x2) + Math.Abs(y2);
                BuildRangePoint(new Vector2(x2, y2), totalDistance, new Vector2(x2, 0));
            }
        }
    }

    private void BuildRangePoint(Vector2 toPosition, int distance, Vector2 fromPosition)
    {
        if (distance > _range) return;
        var rangeIndType = distance switch
        {
            0 => "GridPoint Home",
            1 => "GridPoint Green",
            2 => "GridPoint Blue",
            3 => "GridPoint Red",
            _ => "GridPoint Home"
        };
        var boardLocation = new BoardLocation($"GridPoint ({toPosition.x},{toPosition.y})", "GridPoint", toPosition);
        if (!_boardPoints.IsValidBoardPoint(toPosition)) return;
        if (!IsValidRouteAvailable(toPosition, fromPosition)) return;
        _rangerPoints.Add(boardLocation);
        InitRangeIndicator(rangeIndType, toPosition);
    }

    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    private bool IsValidRouteAvailable(Vector2 toPosition, Vector2 fromPosition)
    {
        var playerX = fromPosition.x;
        var playerY = fromPosition.y;
        var toPosX = toPosition.x;
        var toPosY = toPosition.y;

        if (playerX == toPosX && playerY != toPosY)
        {
            if (toPosY > playerY)
            {
                for (var i = playerY; i < toPosY; i++)
                {
                    if (_colliders.CheckIfColliderPoint(new Vector2(toPosX, i))) return false;
                }
            }
            else
            {
                if (toPosY >= playerY) return true;
                for (var i = playerY; i > toPosY; i--)
                {
                    if (_colliders.CheckIfColliderPoint(new Vector2(toPosX, i))) return false;
                }
            }

            return true;
        }
        
        if (playerY == toPosY && playerX != toPosX)
        {
            if (toPosX > playerX)
            {
                for (var i = playerX; i < toPosX; i++)
                {
                    if (_colliders.CheckIfColliderPoint(new Vector2(i, toPosY))) return false;
                }
            }
            else
            {
                if (toPosX >= playerX) return true;
                for (var i = playerX; i > toPosX; i--)
                {
                    if (_colliders.CheckIfColliderPoint(new Vector2(i, toPosY))) return false;
                }
            }

            return true;
        }

        return true;
    }

    private static void InitRangeIndicator(string rpIndType, Vector2 position)
    {
        var requestedGo = GameObject.Find(rpIndType);
        BoardActions.Init(requestedGo, position);
    }

    private static void ResetAllRangeIndicators()
    {
        var allRangePoints = GameObject.FindGameObjectsWithTag("RangePoint");
        foreach (var rangePoint in allRangePoints)
        {
            BoardActions.Remove(rangePoint);
        }
    }
}