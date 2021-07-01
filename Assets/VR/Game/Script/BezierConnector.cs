
using UnityEngine;

public class BezierConnector : MonoBehaviour
{
    [SerializeField]
    private Transform _origin;
    
    [SerializeField]
    private Transform _end;
    
    [SerializeField]
    private Transform[] _midPoints;
    
    [SerializeField]
    private float _clampAngleTo;
    
    [SerializeField]
    private bool _orientTransforms;
    
    [SerializeField]
    private bool _alternateOrientation;
    
    [SerializeField]
    private bool _offsetOrientation;

    private void Update()
    {
        var from = _end.position - _origin.position;
        var forward = _origin.forward;
        var num = Mathf.Clamp(Vector3.Angle(from, forward), 0f - _clampAngleTo, _clampAngleTo);
        var d = Mathf.Clamp(from.magnitude / (2f * Mathf.Cos(num * 0.0174532924f)), 0f,
            from.magnitude / (float) _midPoints.Length * 2f);
        var topPoint = _origin.position + _origin.forward * d;
        for (var i = 0; i < _midPoints.Length; i++)
        {
            var t = ((float) i + 1f) / ((float) _midPoints.Length + 1f);
            _midPoints[i].position = GetPointAt(t, topPoint);
        }
        if (_orientTransforms)
        {
            for (var j = 0; j < _midPoints.Length - 1; j++)
            {
                if (_alternateOrientation)
                {
                    if (_offsetOrientation)
                    {
                        _midPoints[j].LookAt(_midPoints[j + 1], (j % 2 != 0) ? _origin.up : _origin.right);
                    }
                    else
                    {
                        _midPoints[j].LookAt(_midPoints[j + 1], (j % 2 != 0) ? _origin.right : _origin.up);
                    }
                }
                else if (j > 0)
                {
                    _midPoints[j].LookAt(_midPoints[j + 1], _midPoints[j - 1].up);
                }
                else
                {
                    _midPoints[j].LookAt(_midPoints[j + 1], _origin.up);
                }
            }
            _midPoints[_midPoints.Length - 1].LookAt(_end.transform, _end.up);
        }
    }

    private Vector3 GetPointAt(float t, Vector3 topPoint)
    {
        return (1f - t) * (1f - t) * _origin.transform.position + 2f * (1f - t) * t * topPoint +
               t * t * _end.position;
    }
}



