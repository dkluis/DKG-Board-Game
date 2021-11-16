using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

public class Rangers
{
    private List<BoardLocation> _rangerPoints = new List<BoardLocation>();
    private readonly GameObject _player;
    private readonly BoardCoordinates _boardPoints;

    public Rangers(GameObject player, BoardCoordinates boardPoints)
    {
        _player = player;
        _boardPoints = boardPoints;
        Refill();
    }
    
    public bool CheckRangerPoint(Vector2 position)
    {
        var boardLocation = _rangerPoints.Find(brdLoc => brdLoc.Location == position);
        return boardLocation is { };
    }

    public void Refill()
    {
        ResetGridPoints();
        _rangerPoints = new List<BoardLocation>();
        var currentPos = _player.transform.position;
        var range = _player.GetComponent<PlayerController>().range;
        Debug.Log($"Base Location for Player in Grid {currentPos.x}, {currentPos.y} with Range of {range}");
        for (var x = range * -1; x <= range; x++)
        {
            for (var y = range * -1; y <= range; y++)
            {
                var totalDistance = Math.Abs(x) + Math.Abs(y);
                if (totalDistance > range) continue;
                var newPos = new Vector2(x + currentPos.x, y + currentPos.y);
                var boardLocation = new BoardLocation($"GridPoint ({newPos.x},{newPos.y})", "GridPoint", newPos);
                var gridPointType = totalDistance switch
                {
                    0 => "GridPoint Home",
                    1 => "GridPoint Green",
                    2 => "GridPoint Blue",
                    3 => "GridPoint Red",
                    _ => "GridPoint Home"
                };
                if (!_boardPoints.IsValidBoardPoint(newPos)) continue;
                _rangerPoints.Add(boardLocation);
                InitGridPoint(gridPointType, newPos);
            }
        }
    }
    
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    private void  CullGrid()
    {
        /*
        var playerPos = GameObject.Find("Bird Token Grey").transform.position;
        var playerPosX = playerPos.x;
        var playerPosY = playerPos.y;
        foreach (var rangePoint in RangerPoints)
        {
            var rangePosX = rangePoint.Location.x;
            var rangePosY = rangePoint.Location.y;
            var checkPosX = 0;
            if (playerPosX == rangePosX)
                if (rangePosY < 0)
                    if (ColliderPoints.CheckGameObjects("CircleCollider", new Vector2(rangePosX, playerPosY - 1)))
                    {
                        var rangePointBlocked = new BoardLocation($"GridPoint ({rangePosX},{rangePosY})", "GridPoint", new Vector2(rangePosX, rangePosY));
                        RangerPoints.Find(rangePointBlocked);
                    }
            Debug.Log(rangePoint.Location);  
        }
        */
    }

    private static void InitGridPoint(string gpType, Vector2 position)
    {
        var requestedGo = GameObject.Find(gpType);
        BoardActions.Init(requestedGo, position);
    }

    private static void ResetGridPoints()
    {
        var allGridPoints = GameObject.FindGameObjectsWithTag("RangePoint");
        foreach (var gridPoint in allGridPoints)
        {
            BoardActions.Remove(gridPoint);
        }
    }
}