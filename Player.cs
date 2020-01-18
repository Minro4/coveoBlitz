using static Game;
public class Player
{
    public enum Move
    {
        TURN_LEFT,
        TURN_RIGHT,
        FORWARD
    }

    public enum Direction
    {
        LEFT, RIGHT, DOWN, UP
    }

    public int id;
    public string name;
    public double score;
    public bool active;
    public bool killed;
    public Direction direction;

    public Position position;
    public Position spawnPosition;
    public Position[] tail;


    public Position getFowardPositition()
    {
        switch (direction)
        {
            case Direction.LEFT:
                {
                    return new Position(position.x - 1, position.y);
                    break;
                }
            case Direction.RIGHT:
                {
                    return new Position(position.x + 1, position.y);
                    break;
                }
            case Direction.UP:
                {
                    return new Position(position.x, position.y - 1);
                    break;
                }
            default: //(Down)
                {
                    return new Position(position.x, position.y + 1);
                    break;
                }
        }
    }
    public Position getLeftPositition()
    {
        switch (direction)
        {
            case Direction.LEFT:
                {
                    return new Position(position.x, position.y + 1);
                    break;
                }
            case Direction.RIGHT:
                {
                    return new Position(position.x, position.y - 1);
                    break;
                }
            case Direction.UP:
                {
                    return new Position(position.x - 1, position.y);
                    break;
                }
            default: //(Down)
                {
                    return new Position(position.x + 1, position.y);
                    break;
                }
        }
    }

    public Position getRightPositition()
    {
        switch (direction)
        {
            case Direction.LEFT:
                {
                    return new Position(position.x, position.y - 1);
                    break;
                }
            case Direction.RIGHT:
                {
                    return new Position(position.x, position.y + 1);
                    break;
                }
            case Direction.UP:
                {
                    return new Position(position.x + 1, position.y);
                    break;
                }
            default: //(Down)
                {
                    return new Position(position.x - 1, position.y);
                    break;
                }
        }
    }

    public bool isTail(Position position)
    {
        foreach(Position pos in tail)
        {
            if (pos.sameAs(position))
                return true;
        }
        return false;
    }
}