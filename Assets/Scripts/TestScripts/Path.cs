using UnityEngine;

public class Path
{
    private Vector3[] _pathNodes;
    public int _currentPathIndex = 0;

    public Path(Vector3[] _pathNodes)
    {
        this._pathNodes = _pathNodes;
    }

    public Vector3[] _GetPathNodes()
    {
        return _pathNodes;
    }

    public Vector3 GetNextNode()
    {
        if (_currentPathIndex < _pathNodes.Length)
        {
            return _pathNodes[_currentPathIndex];
        }

        return Vector3.negativeInfinity; 
    }

    public bool ReachedEndNode()
    {
        return (_currentPathIndex == _pathNodes.Length); 
    }
}