using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Random = System.Random;

public class BadGuys
{
    private List<BoardLocation> _colliderPoints = new List<BoardLocation>();

    public BadGuys()
    {
        var pos = new Vector2(0f, 0f);
        ReFill(pos);
    }

    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public void ReFill(Vector2 playerPos, bool randomize = false)
    {
        var sceneInfo = GameObject.Find("SceneInfo");
        var maxes = sceneInfo.GetComponent<SceneInfo>();
        _colliderPoints = new List<BoardLocation>();
        var allGameObjectLocations = GameObject.FindGameObjectsWithTag("BadGuys");
        var random = new Random();
        foreach (var gO in allGameObjectLocations)
        {
            var good = false;

            if (randomize)
            {
                while (!good)
                {
                    var newPos = new Vector2(random.Next(maxes.boardMaxX * -1, maxes.boardMaxX),
                        random.Next(maxes.boardMaxY * -1, maxes.boardMaxY));
                    gO.transform.position = newPos;
                    if (playerPos.x == newPos.x && playerPos.y == newPos.y) continue;
                    if (!CheckIfColliderPoint(newPos) && !CheckIfTokenExists(newPos)) good = true;
                }
            }
            
            using var boardLocation = new BoardLocation(gO.name, gO.tag, gO.transform.position);
            _colliderPoints.Add(boardLocation);
        }
    }
    
    private bool CheckIfTokenExists(Vector2 locationCheck)
    {
        var gOList = GameObject.FindGameObjectsWithTag("Token");
        foreach (var gO in gOList)
        {
            if ((Vector2) gO.transform.position == locationCheck) return true;
        }

        return false;
    }

    public bool CheckIfColliderPoint(Vector2 position)
    {
        var boardLocation = _colliderPoints.Find(brdLoc => brdLoc.Location == position);
        return boardLocation is { };
    }

    public void Shuffle(Vector2 playerPos)
    {
        ReFill(playerPos, true);
    }
}