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
                }
            case Direction.RIGHT:
                {
                    return new Position(position.x + 1, position.y);
                }
            case Direction.UP:
                {
                    return new Position(position.x, position.y - 1);
                }
            default: //(Down)
                {
                    return new Position(position.x, position.y + 1);
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
                }
            case Direction.RIGHT:
                {
                    return new Position(position.x, position.y - 1);
                }
            case Direction.UP:
                {
                    return new Position(position.x - 1, position.y);
                }
            default: //(Down)
                {
                    return new Position(position.x + 1, position.y);
                }
        }
    }


    public bool canGoTo(Game.Position pos){
        if (direction == Direction.UP && pos.y-1 == position.y)
            return false;
        if (direction == Direction.DOWN && pos.y+1 == position.y)
            return false;
        if (direction == Direction.LEFT && pos.x == position.x +1)
            return false;
        if (direction == Direction.RIGHT && pos.x == position.x-1)
            return false;
        return true;

    }

    /*public bool posToMove(Game.Position pos){
        if (direction == Direction.UP && pos.y == position.y-1)
            return false;
        if (direction == Direction.DOWN && pos.y == position.y+1)
            return false;
        if (direction == Direction.LEFT && pos.x == position.x -1)
            return false;
        if (direction == Direction.RIGHT && pos.x == position.x+1)
            return false;
        return true;

    }
*/
    public bool isTail(Game.Position pos){
        foreach(Game.Position tailPos in tail){
            if (pos.sameAs(tailPos)){
                return true;
            }
        }
        return false;
    }


    public Position getRightPositition()
    {
        switch (direction)
        {
            case Direction.LEFT:
                {
                    return new Position(position.x, position.y - 1);
                }
            case Direction.RIGHT:
                {
                    return new Position(position.x, position.y + 1);
                }
            case Direction.UP:
                {
                    return new Position(position.x + 1, position.y);
                }
            default: //(Down)
                {
                    return new Position(position.x - 1, position.y);
                }
        }
    }

}