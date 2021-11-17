using UnityEngine;

public class LineRendering : MonoBehaviour
{
    public float thetaScale = 0.01f;
    public float radius = 2f;
    private int _size;
    private LineRenderer _lineDrawer;
    private float _theta;
    private int _idx;

    private void Start()
    {
        _idx++;
        _lineDrawer = gameObject.AddComponent<LineRenderer>();
        _lineDrawer.startWidth = 0.025f;
        _lineDrawer.endWidth = 0.025f;
        _lineDrawer.startColor = Color.red;
        _lineDrawer.endColor = Color.red;

        _theta = 0f;
        _size = (int) ((1f / thetaScale) + 1f);
        _lineDrawer.positionCount = _size;
        for (var i = 0; i < _size; i++)
        {
            _theta += (2.0f * Mathf.PI * thetaScale);
            var x = radius * Mathf.Cos(_theta);
            var y = radius * Mathf.Sin(_theta);
            _lineDrawer.SetPosition(i, new Vector3(x, y, 2));
        }
    }
}