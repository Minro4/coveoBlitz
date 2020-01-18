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
        Game.Position[] safes;

        int dangerMovesCount = 0;

        bool hasPath = false;
        int currentPathIndex;
        Position[] currentPath;
        Grid pfGrid;

        int dangerCAc = 4;
        int expendCount = 3;
        Game.Position lastPos;

        int dangerMeter = 1;

        bool modeReturn = false;
        bool modeExpand = false;
        Game.Position expendTarget;
        public Bot()
        {
            first_move = true;
            dangerMovesCount = 0;
            // initialize some variables you will need throughout the game here
        }

        public Player.Move nextMove(GameMessage gameMessage)
        {
            // Here is where the magic happens, for now the moves are random. I bet you can do better ;)
            
            Random random = new Random();
            game = gameMessage.game;
            gameMessage.getPlayerMapById.TryGetValue(gameMessage.game.playerId, out me);

            initAStar();
           // Player.Move[] legalMoves = getLegalMovesForCurrentTick(gameMessage);
            if (me.killed){
                first_move = true;
                hasPath = false;
                dangerMovesCount = 0;
                modeExpand =false;
                modeReturn = false;
            }

            Game.Position target;
            coins = findCoins(gameMessage);
            safes = findSafes(gameMessage);
            if (game.getTileTypeAt(me.position) != TileType.CONQUERED){
               
                if (isInDanger(gameMessage,dangerMeter)){
                    modeReturn = true;
                    modeExpand = false;
                }
           

                if (modeReturn){
                    target = returnToSafe();
                }
                else {
                    if (dangerMovesCount >= dangerCAc)
                        {
                        dangerMovesCount = 0;

                        modeExpand = true;
                        expendTarget = getExpand();
                        
                    }
                if (modeExpand){
                    if (me.position.sameAs(new Game.Position(expendTarget.x,expendTarget.y))){
                        modeExpand = false;
                        modeReturn = true;
                        target = returnToSafe();
                    }
                    else
                        target = expendTarget;

                }else{
                    modeReturn = false;
                    dangerMovesCount++;
                    target = getClosestCoin();
                }}
            }
            else{
                dangerMovesCount = 0;
                    modeReturn = false;
                    dangerMovesCount++;
                    target = getClosestCoin();
                }
            

            //if (!hasPath)
            initPath(target);
            Console.WriteLine("modeReturn:" + modeReturn.ToString() + " modeExpand:" + modeExpand.ToString());
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

        private Game.Position returnToSafe(){
            return getClosestSafe();
        }

        private Game.Position getExpand(){
            switch(me.direction){
                case Player.Direction.UP:{
                    Game.Position rightPos = new Game.Position(me.position.x + expendCount, me.position.y);
                    Position[] pathToExpend = pfGrid.GetPath(toAStarPos(me.position),toAStarPos(rightPos),MovementPatterns.LateralOnly);
                    if (pathToExpend.Length != 0 &&pathToExpend.Length -1 <= expendCount){
                        return rightPos;
                    }
                    Game.Position leftPos = new Game.Position(me.position.x + expendCount, me.position.y);
                    Position[] pathToExpend2 = pfGrid.GetPath(toAStarPos(me.position),toAStarPos(rightPos),MovementPatterns.LateralOnly);
                    if (pathToExpend.Length != 0 &&pathToExpend2.Length -1 <= expendCount){
                        return leftPos;
                    }
                    modeExpand =false;
                    modeReturn = true;
                    return getClosestSafe();
                }

                case Player.Direction.DOWN:{
                    Game.Position rightPos = new Game.Position(me.position.x + expendCount, me.position.y);
                    Position[] pathToExpend = pfGrid.GetPath(toAStarPos(me.position),toAStarPos(rightPos),MovementPatterns.LateralOnly);
                    if (pathToExpend.Length != 0 &&pathToExpend.Length -1 <= expendCount){
                        return rightPos;
                    }
                    Game.Position leftPos = new Game.Position(me.position.x + expendCount, me.position.y);
                    Position[] pathToExpend2 = pfGrid.GetPath(toAStarPos(me.position),toAStarPos(rightPos),MovementPatterns.LateralOnly);
                    if (pathToExpend.Length != 0 &&pathToExpend2.Length -1 <= expendCount){
                        return leftPos;
                    }
                     modeExpand =false;
                    modeReturn = true;
                    return getClosestSafe();
                }


                case Player.Direction.LEFT:
                {
                    Game.Position rightPos = new Game.Position(me.position.x , me.position.y+expendCount);
                    Position[] pathToExpend = pfGrid.GetPath(toAStarPos(me.position),toAStarPos(rightPos),MovementPatterns.LateralOnly);
                    if (pathToExpend.Length != 0 && pathToExpend.Length -1 <= expendCount){
                        return rightPos;
                    }
                    Game.Position leftPos = new Game.Position(me.position.x, me.position.y-expendCount);
                    Position[] pathToExpend2 = pfGrid.GetPath(toAStarPos(me.position),toAStarPos(rightPos),MovementPatterns.LateralOnly);
                    if (pathToExpend.Length != 0 &&pathToExpend2.Length -1 <= expendCount){
                        return leftPos;
                    }
                     modeExpand =false;
                    modeReturn = true;
                    return getClosestSafe();
                }
                
                  default:
                  {
                    Game.Position rightPos = new Game.Position(me.position.x , me.position.y+expendCount);
                    Position[] pathToExpend = pfGrid.GetPath(toAStarPos(me.position),toAStarPos(rightPos),MovementPatterns.LateralOnly);
                    if (pathToExpend.Length != 0 &&pathToExpend.Length -1 <= expendCount){
                        return rightPos;
                    }
                    Game.Position leftPos = new Game.Position(me.position.x, me.position.y-expendCount);
                    Position[] pathToExpend2 = pfGrid.GetPath(toAStarPos(me.position),toAStarPos(rightPos),MovementPatterns.LateralOnly);
                    if (pathToExpend.Length != 0 &&pathToExpend2.Length -1 <= expendCount){
                        return leftPos;
                    }
                     modeExpand =false;
                    modeReturn = true;
                    return getClosestSafe();
                }
            }
        }



        public void initAStar(){
            int mapX = game.map.Length;
            int mapY = game.map[0].Length;

            pfGrid = new Grid(mapX,mapY,1);
            for(int i =0; i< mapX; i++){
                for(int j=0; j<mapY;j++){
                    Game.Position pos = new Game.Position(i,j);
                    TileType tile = game.getTileTypeAt(pos);
                    if(me.isTail(pos) && !me.spawnPosition.sameAs(pos))
                        pfGrid.BlockCell(new Position(i,j));
                    if (Game.isBlock(tile)){
                        pfGrid.BlockCell(new Position(i,j));
                    }
                }
            }
            currentPathIndex = 1;
        }
        
        private bool isInDanger(GameMessage gameMessage,int danger_meter)
        {
            int steps_to_safety = getStepToSafety(getClosestSafe());
            int steps_to_die = 99999;
            foreach(Player player in gameMessage.players)
            {
                if (player.id == me.id)
                {
                    continue;
                }
                Game.Position  enenemy_pos = player.position;
                Game.Position endangered_tail = getClosestTail(enenemy_pos);
                int current_enemy_kill_potential = pfGrid.GetPath(toAStarPos(endangered_tail),toAStarPos(enenemy_pos)).Length;
                if (steps_to_die > current_enemy_kill_potential)
                {
                    steps_to_die = current_enemy_kill_potential;
                }
            }
            if (steps_to_die  + danger_meter < steps_to_safety)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private Game.Position getClosestTail(Game.Position enenemy_pos)
        {
            if (me.tail.Length == 0)
            {
                return me.position;
            }
            Game.Position min  = me.tail[0];
            int mindist = 999;
            foreach (Game.Position tail in me.tail)
            {
                if (tail.distance(enenemy_pos) < mindist)
                {
                    min = tail;
                }
            }
            return min;
        }




        public Player.Move playPath(GameMessage message){
            if (currentPath.Length == 0)
            {
                Random random = new Random();
                Player.Move[] legalMoves = getLegalMovesForCurrentTick(message);
                try {
                    return legalMoves[random.Next(legalMoves.Length)]; 
                }
                catch{
                    return Player.Move.FORWARD;
                }
            }
            Position nextPosAS = currentPath[currentPathIndex];
            Console.WriteLine(currentPath.ToString());
            Console.WriteLine(me.direction.ToString());
            Console.WriteLine(nextPosAS.ToString());
            Game.Position nextPos = new Game.Position(nextPosAS.X,nextPosAS.Y);
            currentPathIndex++;
            if (currentPathIndex >= currentPath.Length){
                hasPath = false;
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
           // initAStar();
            hasPath = true;
            currentPath= pfGrid.GetPath(toAStarPos(me.position),toAStarPos(blitzPos),MovementPatterns.LateralOnly);
            currentPathIndex = 1;
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
            Console.WriteLine(min.ToString());
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
            try
            {
                //Console.WriteLine(positions[0].ToString());
            }
            catch
            {
                Console.WriteLine("no more coins");
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
        private Game.Position[] findSafes(GameMessage gameMessage)
        {
            List<Game.Position> positions = new List<Game.Position>();

            int s = gameMessage.game.getMapSize();
            for (int i = 0; i < s;i++)
            {
                for (int j = 0; j < s;j++)
                {
                    Game.Position p = new Game.Position(i,j);
                    if (game.getTileTypeAt(p) == TileType.CONQUERED||game.getTileTypeAt(p) == TileType.CONQUERED_PLANET)
                    {
                        if (game.getTileOwnerId(p) == me.id)
                        {
                            positions.Add(p);
                        }
                    }

                }
            }
            
            return positions.ToArray();
        }
        private Game.Position getClosestSafe(){
            if (safes.Length == 0)
                return me.spawnPosition;

            Game.Position min = safes[0];
            int minDist = min.distance(me.position);
            for(int i=1; i<safes.Length;i++){
                Game.Position pos = safes[i];

                if (pos.distance(me.position) < minDist){
                    min = pos;
                    minDist = pos.distance(me.position);
                }
            }
            Console.WriteLine(min.ToString());
            return min;
        }
        private int getStepToSafety(Game.Position safetyPos){
        
            Position[] pathToSafety = pfGrid.GetPath(toAStarPos(me.position),toAStarPos(safetyPos),MovementPatterns.LateralOnly);
            return pathToSafety.Length;
        }


    }
}