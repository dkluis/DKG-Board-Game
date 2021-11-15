using UnityEngine;

public class BoardActions : MonoBehaviour
{
    public static void Init(GameObject gO, Vector2 position)
    {
        var newGp = Instantiate(gO.transform, position, Quaternion.identity);
        newGp.name = $"Range Point ({position.x},{position.y})";
        newGp.tag = "RangePoint";
        newGp.transform.position = position;
    }

    public static void Remove(GameObject gO)
    {
        Destroy(gO);
    }
}