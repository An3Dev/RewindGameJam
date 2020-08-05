using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointInTime
{
    public Vector2 position;
    public int direction;

    public PointInTime (Vector2 _position, int _direction)
    {
        position = _position;
        direction = _direction;
    }
}
