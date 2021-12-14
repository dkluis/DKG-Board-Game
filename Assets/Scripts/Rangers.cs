using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
// ReSharper disable RedundantArgumentDefaultValue

public class Rangers
{
    private List<BoardLocation> _rangerPoints = new List<BoardLocation>();
    private int _range;
    private Vector2 _activeTokenPosition;
    private readonly BoardCoordinates _boardPoints;
    private readonly BadGuys _badGuys;

    public Rangers(BoardCoordinates boardPoints, BadGuys badGuys)
    {
        _boardPoints = boardPoints;
        _badGuys = badGuys;
    }

    public bool CheckRangerPoint(Vector2 position)
    {
        var boardLocation = _rangerPoints.Find(brdLoc => brdLoc.Location == position);
        return boardLocation is { };
    }

    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public void Refill(GameObject activeToken, int originalX = 9999, int originalY = 9999)
    {
        _range = activeToken.GetComponent<TokenController>().range;
        _activeTokenPosition = activeToken.transform.position;
        ResetAllRangeIndicators();
        _rangerPoints = new List<BoardLocation>();
        Debug.Log($"Refill RangePoints from location {_activeTokenPosition.x}, {_activeTokenPosition.y} with Range of {_range}");

        int x0;
        int y0;
        if (originalX != 9999 && originalY != 9999)
        {
            x0 = originalX;
            y0 = originalY;
        }
        else
        {

            x0 = (int) _activeTokenPosition.x;
            y0 = (int) _activeTokenPosition.y;
        }

        // Build the X-Axis going right with Y going up
        var xModPlus = 0;
        for (var yLoop1 = y0; yLoop1 <= _range + y0; yLoop1++)
        {
            if (_badGuys.CheckIfColliderPoint(new Vector2(x0, yLoop1))) continue;
            if (!CheckRangerPoint(new Vector2(x0, yLoop1 - 1)) && yLoop1 != y0) continue;
            BuildRangePoint(new Vector2(x0, yLoop1), 0);
            for (var xLoop1 = x0; xLoop1 <= _range + x0 - xModPlus; xLoop1++)
            {
                if (CheckRangerPoint(new Vector2(xLoop1, yLoop1))) continue;
                if (_activeTokenPosition.x == xLoop1) continue;
                if (!CheckRangerPoint(new Vector2(xLoop1 - 1, yLoop1))) continue; 
                if (_badGuys.CheckIfColliderPoint(new Vector2(xLoop1, yLoop1))) continue;
                BuildRangePoint(new Vector2(xLoop1, yLoop1), 0);
            }
            xModPlus++;
        }
        // Build the X-Axis going right with Y going down
        xModPlus = 1;
        for (var yLoop2 = y0 - 1; yLoop2 >= (_range * -1) + y0; yLoop2--)
        {
            if (_badGuys.CheckIfColliderPoint(new Vector2(x0, yLoop2))) continue;
            if (!CheckRangerPoint(new Vector2(x0, yLoop2 + 1))) continue; 
            BuildRangePoint(new Vector2(x0, yLoop2), 1);
            for (var xLoop2 = x0; xLoop2 <= _range + x0 - xModPlus; xLoop2++)
            {
                if (CheckRangerPoint(new Vector2(xLoop2, yLoop2))) continue;
                if (_activeTokenPosition.x == xLoop2) continue;
                if (!CheckRangerPoint(new Vector2(xLoop2 - 1, yLoop2))) continue;
                if (_badGuys.CheckIfColliderPoint(new Vector2(xLoop2, yLoop2))) continue;
                BuildRangePoint( new Vector2(xLoop2, yLoop2), 1);
            }
            xModPlus++;
        }

        // Build the X-Axis going left with Y going up
        var xModMinus = 0;
        for (var yLoop3 = y0; yLoop3 <= _range + y0; yLoop3++)
        {
            if (_badGuys.CheckIfColliderPoint(new Vector2(x0, yLoop3))) continue;
            if (!CheckRangerPoint(new Vector2(x0, yLoop3 - 1)) && yLoop3 != y0) continue;
            if (!CheckRangerPoint(new Vector2(x0, yLoop3))) BuildRangePoint(new Vector2(x0, yLoop3), 2);
            for (var xLoop3 = x0; xLoop3 >= (_range * -1) + x0 - xModMinus; xLoop3--)
            {
                if (CheckRangerPoint(new Vector2(xLoop3, yLoop3))) continue;
                if (_activeTokenPosition.x == xLoop3) continue;
                if (!CheckRangerPoint(new Vector2(xLoop3 + 1, yLoop3))) continue; 
                if (_badGuys.CheckIfColliderPoint(new Vector2(xLoop3, yLoop3))) continue;
                BuildRangePoint( new Vector2(xLoop3, yLoop3), 2);
            }
            xModMinus--;
        }
        
        // Build the X-Axis going right with Y going down
        xModMinus = -1;
        for (var yLoop4 = y0; yLoop4 >= (_range * -1) + y0; yLoop4--)
        {
            if (y0 == yLoop4) continue;
            if (_badGuys.CheckIfColliderPoint(new Vector2(x0, yLoop4))) continue;
            if (!CheckRangerPoint(new Vector2(x0, yLoop4 + 1))) continue;
            for (var xLoop4 = x0; xLoop4 >= (_range * -1) + x0 - xModMinus; xLoop4--)
            {
                if (CheckRangerPoint(new Vector2(xLoop4, yLoop4))) continue;
                if (!CheckRangerPoint(new Vector2(xLoop4 + 1, yLoop4))) continue;
                if (_badGuys.CheckIfColliderPoint(new Vector2(xLoop4, yLoop4))) continue;
                BuildRangePoint(new Vector2(xLoop4, yLoop4), 3);
            }
            xModMinus--;
        }
        
        // Evaluate from the Y-Axis going up with X axis going right
        var yModPlus = 0;
        for (var xLoop5 = x0; xLoop5 <= _range + x0; xLoop5++)
        {
            if (!CheckRangerPoint(new Vector2(xLoop5, y0))) break;
            for (var yLoop5 = y0; yLoop5 <= _range + y0 - yModPlus; yLoop5++)
            {
                if (CheckRangerPoint(new Vector2(xLoop5, yLoop5))) continue;
                if (_badGuys.CheckIfColliderPoint(new Vector2(xLoop5, yLoop5))) break;
                BuildRangePoint(new Vector2(xLoop5, yLoop5), 4);
            }
            yModPlus++;
        }
        
        //Evaluate from the Y-Axis going up with the X axis going left
        yModPlus = 0;
        for (var xLoop6 = x0; xLoop6 >= (_range * -1) + x0; xLoop6--)
        {
            if (!CheckRangerPoint(new Vector2(xLoop6, y0))) break;
            for (var yLoop6 = y0; yLoop6 <= _range + y0 - yModPlus; yLoop6++)
            {
                if (CheckRangerPoint(new Vector2(xLoop6, yLoop6))) continue;
                if (_badGuys.CheckIfColliderPoint(new Vector2(xLoop6, yLoop6))) break;
                BuildRangePoint(new Vector2(xLoop6, yLoop6), 5);
            }
            yModPlus++;
        }
        
        //Evaluate from the Y-Axis going down with the X-Axis going right
        var yModMinus = 0;
        for (var xLoop7 = x0; xLoop7 >= (_range * -1) + x0; xLoop7--)
        {
            if (!CheckRangerPoint(new Vector2(xLoop7, y0))) break;
            for (var yLoop7 = y0; yLoop7 >= (_range * -1) + y0 - yModMinus; yLoop7--)
            {
                if (CheckRangerPoint(new Vector2(xLoop7, yLoop7))) continue;
                if (_badGuys.CheckIfColliderPoint(new Vector2(xLoop7, yLoop7))) break;
                BuildRangePoint(new Vector2(xLoop7, yLoop7), 6);
            }
            yModMinus--;
        }
        
        //Evaluate from the Y-Axis going down with the X-Axis going left
        yModMinus = 0;
        for (var xLoop6 = x0; xLoop6 <= _range + x0; xLoop6++)
        {
            if (!CheckRangerPoint(new Vector2(xLoop6, y0))) break;
            for (var yLoop6 = y0; yLoop6 >= (_range * -1) + y0 - yModMinus; yLoop6--)
            {
                if (CheckRangerPoint(new Vector2(xLoop6, yLoop6))) continue;
                if (_badGuys.CheckIfColliderPoint(new Vector2(xLoop6, yLoop6))) break;
                BuildRangePoint(new Vector2(xLoop6, yLoop6), 7);
            }
            yModMinus--;
        }
    }

    private void BuildRangePoint(Vector2 toPosition, int distance = 4)
    {
        var rangeIndType = distance switch
        {
            0 => "GridPoint Home",
            1 => "GridPoint Green",
            2 => "GridPoint Blue",
            3 => "GridPoint Red",
            4 => "GridPoint AltHome",
            5 => "GridPoint AltBlue",
            6 => "GridPoint AltRed",
            7 => "GridPoint AltGreen",
            _ => "GridPoint AltRed"
        };
        var boardLocation = new BoardLocation($"GridPoint ({toPosition.x},{toPosition.y})", "GridPoint", toPosition);
        if (!_boardPoints.IsValidBoardPoint(toPosition)) return;
        _rangerPoints.Add(boardLocation);
        InitRangeIndicator(rangeIndType, toPosition);
    }

    public static int CalculateStepBetweenCoordinates(Vector2 fromPosition, Vector2 toPosition)
    {
        var xDelta = 0;
        if (fromPosition.x >= 0 && toPosition.y >= 0)
        {
            xDelta = (int) Math.Abs(fromPosition.x - toPosition.x);
        }
        else if (toPosition.x < 0)
        {
            xDelta = (int) Math.Abs(fromPosition.x + toPosition.x);
        }

        var yDelta = 0;
        if (fromPosition.y >= 0 && toPosition.x >= 0)
        {
            yDelta = (int) Math.Abs(fromPosition.y - toPosition.y);
        } else if (toPosition.y >= 0)
        {
            yDelta = (int) Math.Abs(fromPosition.y - toPosition.y);
        }
        else if (toPosition.y < 0)
        {
            yDelta = (int) Math.Abs(fromPosition.y + toPosition.y);
        }

        var distance = xDelta + yDelta;
        return distance;
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