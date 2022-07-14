using UnityEngine;

//this will store our path towards a location the AI wants to move towards
public class Path
{
    private Vector3[] _pathNodes;
    public int _currentPathIndex = 0;

    public Path(Vector3[] _pathNodes)
    {
        this._pathNodes = _pathNodes;
    }

    public Vector3[] GetPathNodes()
    {
        return _pathNodes;
    }

    public Vector3 GetNextNode()
    {
        if (_currentPathIndex < _pathNodes.Length)
        {
            return _pathNodes[_currentPathIndex];
        }

        return Vector3.negativeInfinity; //we reached the end / there is no path to follow
    }

    public bool ReachedEndNode()
    {
        return (_currentPathIndex == _pathNodes.Length); //returns true if we have reached the end of our path
    }
}