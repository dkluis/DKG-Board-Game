using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Colliders
{
    private List<BoardLocation> _colliderPoints = new List<BoardLocation>();

    public Colliders()
    {
        ReFill();
    }

    public void ReFill(bool randomize = false)
    {
        _colliderPoints = new List<BoardLocation>();
        var allGameObjectLocations = GameObject.FindGameObjectsWithTag("CircleCollider");
        var random = new Random();
        foreach (var gO in allGameObjectLocations)
        {
            if (randomize)
            {
                var newPos = new Vector2(random.Next(-9, 9), random.Next(-5, 5));
                gO.transform.position = newPos;
            }
            using var boardLocation = new BoardLocation(gO.name, gO.tag, gO.transform.position);
            _colliderPoints.Add(boardLocation);
        }
    }

    public bool CheckIfColliderPoint(Vector2 position)
    {
        var boardLocation = _colliderPoints.Find(brdLoc => brdLoc.Location == position);
        return boardLocation is { };
    }

    public void Shuffle()
    {
        ReFill(true);
    }
}