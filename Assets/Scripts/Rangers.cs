using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

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
        Refill(player);
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

        Debug.Log($"Base Location for Player in Grid {_playerPosition.x}, {_playerPosition.y} with Range of {_range}");
        for (var x = _range * -1; x <= _range; x++)
        {
            for (var y = _range * -1; y <= _range; y++)
            {
                var totalDistance = Math.Abs(x) + Math.Abs(y);
                if (totalDistance > _range) continue;
                var newPos = new Vector2(x + _playerPosition.x, y + _playerPosition.y);
                var boardLocation = new BoardLocation($"GridPoint ({newPos.x},{newPos.y})", "GridPoint", newPos);
                var rangeIndType = totalDistance switch
                {
                    0 => "GridPoint Home",
                    1 => "GridPoint Green",
                    2 => "GridPoint Blue",
                    3 => "GridPoint Red",
                    _ => "GridPoint Home"
                };
                if (!_boardPoints.IsValidBoardPoint(newPos)) continue;
                //if (!IsValidRouteAvailable(newPos)) continue;
                _rangerPoints.Add(boardLocation);
                InitRangeIndicator(rangeIndType, newPos);
            }
        }
    }
    
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    private bool IsValidRouteAvailable(Vector2 toPosition)
    {
        
        /*
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
        return true;
    }

    private static void InitRangeIndicator(string rpIndType, Vector2 position)
    {
        var requestedGo = GameObject.Find(rpIndType);
        BoardActions.Init(requestedGo, position);
    }

    private static void ResetAllRangeIndicators()
    {
        var allGridPoints = GameObject.FindGameObjectsWithTag("RangePoint");
        foreach (var gridPoint in allGridPoints)
        {
            BoardActions.Remove(gridPoint);
        }
    }
}