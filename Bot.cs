using System;

namespace Blitz2020
{
    public class Bot
    {
        public static string NAME = "MyBot C#";
        public static Player.Move[] POSSIBLE_MOVES = (Player.Move[])Enum.GetValues(typeof(Player.Move));

        public Bot()
        {
            // initialize some variables you will need throughout the game here
        }

        public Player.Move nextMove(GameMessage gameMessage)
        {
            // Here is where the magic happens, for now the moves are random. I bet you can do better ;)
            Player.Move[] legalMoves = getLegalMovesForCurrentTick(gameMessage);
            Random random = new Random();

            // You can print out a pretty version of the map but be aware that 
            // printing out long strings can impact your bot performance (30 ms in average).
            // Console.WriteLine(gameMessage.game.prettyMap);

            return legalMoves[random.Next(legalMoves.Length)];
        }

        public Player.Move[] getLegalMovesForCurrentTick(GameMessage gameMessage)
        {        
            


            Player me;
            gameMessage.getPlayerMapById.TryGetValue(gameMessage.game.playerId, out me);

            



            return checkWalls(POSSIBLE_MOVES);
        }

        private Player.Move[] checkWalls(Player.Move[] fromMoves)
        {
            ArrayList<Player.Move> possMoves = new ArrayList<Player.Move>();
            possMoves.addAll(fromMoves);
            foreach (Player.Move move in fromMoves)
            {
                switch (move)
                {
                    case Player.Move.FORWARD:
                        {
                            if (checkSuicideFoward())
                                possMoves.remove(move);
                            break;
                        }
                    case Player.Move.LEFT:
                        {
                            if (checkSuicideLeft())
                                possMoves.remove(move);
                            break;
                        }
                    default: //(Right)
                        {
                            if (checkSuicideRight())
                                possMoves.remove(move);
                            break;
                        }
                }
            }

            return possMoves.toArray();
        }

        private bool checkSuicideFoward()
        {
           return checkSuicide(me.getFowardPositition());
        }
        private bool checkSuicideLeft()
        {
            return checkSuicide(me.getLeftPositition());
        }
        private bool checkSuicideRight()
        {
            return checkSuicide(me.getRightPositition());
        }
        private bool checkSuicide(Player me, Game game, Position position)
        {
            //Position fPos = me.getFowardPositition();
            TyleType tile = game.getTileTypeAt(position);
            if (tile == TileType.ASTEROIDS || tile == TileType.BLACK_HOLE)
            {
                return true;
            }

            if (me.isTail(fPos))
            {
                return true;
            }

            return false;

        }


    }
}