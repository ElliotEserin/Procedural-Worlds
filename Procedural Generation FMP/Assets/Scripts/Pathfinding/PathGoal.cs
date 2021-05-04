using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGoal : MonoBehaviour
{
    public enum Direction
    {
        North,
        East,
        South,
        West
    }

    public Direction facingDirection;
    public int goalCount = 0;

    public bool ValidPath(PathGoal other)
    {
        if (other.facingDirection == facingDirection)
            return false;

        Vector3 direction = (other.transform.position - transform.position).normalized;
        
        if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if(direction.x > 0)
            {
                if (other.facingDirection == Direction.North)
                    return false;
                if(facingDirection == Direction.South)
                    return false;
            }
            else
            {
                if (other.facingDirection == Direction.South)
                    return false;
                if (facingDirection == Direction.North)
                    return false;
            }
        }
        else
        {
            if (direction.y > 0)
            {
                if (other.facingDirection == Direction.East)
                    return false;
                if (facingDirection == Direction.West)
                    return false;
            }
            else
            {
                if (other.facingDirection == Direction.West)
                    return false;
                if (facingDirection == Direction.East)
                    return false;
            }
        }

        return true;
    }
}
