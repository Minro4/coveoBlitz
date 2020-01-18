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

        public Bot()
        {
            
            // initialize some variables you will need throughout the game here
        }

        public Player.Move nextMove(GameMessage gameMessage)
        {
            // Here is where the magic happens, for now the moves are random. I bet you can do better ;)
            
            Random random = new Random();
            game = gameMessage.game;
            gameMessage.getPlayerMapById.TryGetValue(gameMessage.game.playerId, out me);
            Player.Move[] legalMoves = getLegalMovesForCurrentTick(gameMessage);
            // You can print out a pretty version of the map but be aware that 
            // printing out long strings can impact your bot performance (30 ms in average).
            // Console.WriteLine(gameMessage.game.prettyMap);
            if (legalMoves.Length == 0)
            {
                return Player.Move.FORWARD;
            }
            return legalMoves[random.Next(legalMoves.Length)];
        }

      /*  public initAStar(){
            int mapX = game.map.
        }*/

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