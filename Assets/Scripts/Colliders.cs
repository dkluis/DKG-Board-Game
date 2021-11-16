using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

public class Colliders
{
    private List<BoardLocation> _colliderPoints = new List<BoardLocation>();

    public Colliders()
    {
        ReFill();
    }

    public void ReFill()
    {
        _colliderPoints = new List<BoardLocation>();
        var allGameObjectLocations = GameObject.FindGameObjectsWithTag("CircleCollider");
        foreach (var gO in allGameObjectLocations)
        {
            using var boardLocation = new BoardLocation(gO.name, gO.tag, gO.transform.position);
            _colliderPoints.Add(boardLocation);
        }
    }

    public bool CheckIfColliderPoint(Vector2 position)
    {
        var boardLocation = _colliderPoints.Find(brdLoc => brdLoc.Location == position);
        return boardLocation is { };
    }
}