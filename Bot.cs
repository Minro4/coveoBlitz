using System;
using System.Collections.Generic;
using RoyT.AStar;


namespace Blitz2020
{
    public class Bot
    {
        public static string NAME = "MyBot C#";
        public static Player.Move[] POSSIBLE_MOVES = (Player.Move[])Enum.GetValues(typeof(Player.Move));
        Player me;
    
        Game game;
        bool first_move;
        Game.Position[] coins;

        bool hasPath = false;
        int currentPathIndex;
        Position[] currentPath;
        Grid pfGrid;


        Game.Position lastPos;
        public Bot()
        {
            first_move = true;
            // initialize some variables you will need throughout the game here
        }

        public Player.Move nextMove(GameMessage gameMessage)
        {
            // Here is where the magic happens, for now the moves are random. I bet you can do better ;)
            
            Random random = new Random();
            game = gameMessage.game;
            gameMessage.getPlayerMapById.TryGetValue(gameMessage.game.playerId, out me);
           // Player.Move[] legalMoves = getLegalMovesForCurrentTick(gameMessage);
            if (me.killed){
                first_move = true;
                hasPath = false;
            }


            if (first_move)
            {
                coins = findCoins(gameMessage);
                first_move = false;
            }

            if (!hasPath)
                initPath(getClosestCoin());

            return playPath(gameMessage);

            // You can print out a pretty version of the map but be aware that 
            // printing out long strings can impact your bot performance (30 ms in average).
            // Console.WriteLine(gameMessage.game.prettyMap);
          
          /*  if (legalMoves.Length == 0)
            {
                return Player.Move.FORWARD;
            }
            return legalMoves[random.Next(legalMoves.Length)];*/
        }

        public void initAStar(){
            int mapX = game.map.Length;
            int mapY = game.map[0].Length;

            pfGrid = new Grid(mapX,mapY,1);
            for(int i =0; i< mapX; i++){
                for(int j=0; j<mapY;j++){
                    Game.Position pos = new Game.Position(i,j);
                    TileType tile = game.getTileTypeAt(pos);
                    if(me.isTail(pos))
                        pfGrid.BlockCell(new Position(i,j));
                    if (Game.isBlock(tile)){
                        pfGrid.BlockCell(new Position(i,j));
                    }
                }
            }
            currentPathIndex = 1;
        }



        public Player.Move playPath(GameMessage message){
            Position nextPosAS = currentPath[currentPathIndex];
            Game.Position nextPos = new Game.Position(nextPosAS.X,nextPosAS.Y);
            currentPathIndex++;
            if (currentPathIndex >= currentPath.Length){
                hasPath = false;
                coins = findCoins(message);
            }
            if (!me.canGoTo(nextPos)){
                bool isSuicideLeft = checkSuicideLeft();
                bool isSuicideRight = checkSuicideRight();
                if (isSuicideLeft && !isSuicideRight){
                    return Player.Move.TURN_RIGHT;
                }
                if (!isSuicideLeft && isSuicideRight){
                    return Player.Move.TURN_LEFT;
                }
                return Player.Move.FORWARD;
            }
            return positionToMove(nextPos);
        }

        public Position[] initPath(Game.Position blitzPos){
            initAStar();
            hasPath = true;
            currentPath= pfGrid.GetPath(toAStarPos(me.position),toAStarPos(blitzPos));
            return currentPath;
        }

        private Game.Position getClosestCoin(){
            if (coins.Length == 0)
                return me.spawnPosition;

            Game.Position min = coins[0];
            int minDist = min.distance(me.position);
            for(int i=1; i<coins.Length;i++){
                Game.Position pos = coins[i];

                if (pos.distance(me.position) < minDist){
                    min = pos;
                    minDist = pos.distance(me.position);
                }
            }
            return min;
        }

        private Position toAStarPos(Game.Position pos){
            return new Position(pos.x,pos.y);
        }

        public Player.Move[] getLegalMovesForCurrentTick(GameMessage gameMessage)
        {                 
            return checkWalls(POSSIBLE_MOVES);
        }


        private Player.Move[] checkWalls(Player.Move[] fromMoves)
        {
            List<Player.Move> possMoves = new List<Player.Move>();
            possMoves.AddRange(fromMoves);
            foreach (Player.Move move in fromMoves)
            {
                switch (move)
                {
                    case Player.Move.FORWARD:
                        {
                            if (checkSuicideFoward())
                                possMoves.Remove(move);
                            break;
                        }
                    case Player.Move.TURN_LEFT:
                        {
                            if (checkSuicideLeft())
                                possMoves.Remove(move);
                            break;
                        }
                    default: //(Right)
                        {
                            if (checkSuicideRight())
                                possMoves.Remove(move);
                            break;
                        }
                }
            }

            return possMoves.ToArray();
        }
        private Game.Position[] findCoins(GameMessage gameMessage)
        {
            List<Game.Position> positions = new List<Game.Position>();

            int s = gameMessage.game.getMapSize();
            for (int i = 0; i < s;i++)
            {
                for (int j = 0; j < s;j++)
                {
                    Game.Position p = new Game.Position(i,j);
                    if (game.getTileTypeAt(p) == TileType.BLITZIUM)
                    {
                        positions.Add(p);
                    }

                }
            }
            return positions.ToArray();
        }
        private Player.Move positionToMove(Game.Position position)
        {
            switch (me.direction)
            {
                case Player.Direction.UP:
                {
                    if(position.x > me.position.x)
                    {
                        return Player.Move.TURN_RIGHT;
                    }
                    else if (position.x < me.position.x)
                    {
                        return Player.Move.TURN_LEFT;
                    }
                    else
                    {
                        return Player.Move.FORWARD;
                    }
                    break;
                }
                case Player.Direction.DOWN:
                {
                    if(position.x < me.position.x)
                    {
                        return Player.Move.TURN_RIGHT;
                    }
                    else if (position.x > me.position.x)
                    {
                        return Player.Move.TURN_LEFT;
                    }
                    else
                    {
                        return Player.Move.FORWARD;
                    }
                    break;
                }
                case Player.Direction.RIGHT:
                {
                    if(position.y > me.position.y)
                    {
                        return Player.Move.TURN_RIGHT;
                    }
                    else if (position.y < me.position.y)
                    {
                        return Player.Move.TURN_LEFT;
                    }
                    else
                    {
                        return Player.Move.FORWARD;
                    }
                    break;
                }
                case Player.Direction.LEFT:
                {
                    if(position.y < me.position.y)
                    {
                        return Player.Move.TURN_RIGHT;
                    }
                    else if (position.y > me.position.y)
                    {
                        return Player.Move.TURN_LEFT;
                    }
                    else
                    {
                        return Player.Move.FORWARD;
                    }
                    break;
                }
                default:
                {
                    return Player.Move.FORWARD;
                }
            }
        }
        private bool checkSuicideFoward()
        {
           return checkSuicide(me,game,me.getFowardPositition());
        }
        private bool checkSuicideLeft()
        {
            return checkSuicide(me,game,me.getLeftPositition());
        }
        private bool checkSuicideRight()
        {
            return checkSuicide(me,game,me.getRightPositition());
        }
        private bool checkSuicide(Player me, Game game, Game.Position position)
        {
            //Position fPos = me.getFowardPositition();
            TileType tile = game.getTileTypeAt(position);
            if (tile == TileType.ASTEROIDS || tile == TileType.BLACK_HOLE)
            {
                return true;
            }

            if (me.isTail(position))
            {
                return true;
            }

            return false;

        }


    }
}