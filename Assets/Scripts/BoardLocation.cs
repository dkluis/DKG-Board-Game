using System;
using UnityEngine;

public class BoardLocation : IDisposable
{
    public readonly string Name;
    public readonly string Tag;
    public Vector2 Location;

    public BoardLocation(string name, string tag, Vector3 position)
    {
        Name = name;
        Tag = tag;
        Location = position;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}