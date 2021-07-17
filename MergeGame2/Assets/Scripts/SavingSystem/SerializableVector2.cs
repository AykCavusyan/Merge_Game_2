using System;
using UnityEngine;

[Serializable]
public class SerializableVector2  
{
    float x;
    float y;

  public SerializableVector2(Vector2 vector2IN)
    {
        x = vector2IN.x;
        y = vector2IN.y;
    } 

    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }


}
