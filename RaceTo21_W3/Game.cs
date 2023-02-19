using System;
using System.Collections.Generic;
using System.Linq;

namespace RaceTo21
{
    public class Game
    {
        int numberOfPlayers; // number of players in current game
        List<Player> players = new List<Player>(); // list of objects containing player data
        CardTable cardTable; // object in charge of displaying game information
        Deck deck = new Deck(); // deck of cards
        int currentPlayer = 0; // current player on list
        public Task nextTask; // keeps track of game state
        private bool cheating = false; // lets you cheat for testing purposes if true
        Player Roundwinner = null;

        public Game(CardTable c)
        {
            cardTable = c;
            deck.Shuffle();
            //deck.ShowAllCards();
            nextTask = Task.GetNumberOfPlayers;
        }

        /* Adds a player to the current game
         * Called by DoNextTask() method
         */
        public void AddPlayer(string n)
        {
            players.Add(new Player(n));
        }

        /* Figures out what task to do next in game
         * as represented by field nextTask
         * Calls methods required to complete task
         * then sets nextTask.
         */
        public void DoNextTask()
        {
            Console.WriteLine("================================"); // this line should be elsewhere right?
            if (nextTask == Task.GetNumberOfPlayers)
            {
                numberOfPlayers = cardTable.GetNumberOfPlayers();
                nextTask = Task.GetNames;
            }
            else if (nextTask == Task.GetNames)
            {
                for (var count = 1; count <= numberOfPlayers; count++)
                {
                    var name = cardTable.GetPlayerName(count);
                    AddPlayer(name); // NOTE: player list will start from 0 index even though we use 1 for our count here to make the player numbering more human-friendly
                }
                nextTask = Task.IntroducePlayers;
            }
            else if (nextTask == Task.IntroducePlayers)
            {
                cardTable.ShowPlayers(players);
                nextTask = Task.PlayerTurn;
            }
            else if (nextTask == Task.PlayerTurn)
            {
                cardTable.ShowHands(players);
                Player player = players[currentPlayer];
                if (player.status == PlayerStatus.active)
                {
                    if (cardTable.OfferACard(player))
                    {
                        int numberOfCards = cardTable.GetNumberOfCardstoDraw();

                        for(int i=0;i<numberOfCards;i++)
                        {
                            Card card = deck.DealTopCard();
                            player.cards.Add(card);
                            player.score = ScoreHand(player);
                            if (player.score > 21)
                            {
                                player.status = PlayerStatus.bust;
                            }
                        }
                        if (player.score == 21)
                        {
                            player.status = PlayerStatus.win;
                            cardTable.ShowHand(player);
                            cardTable.AnnounceWinner(player);
                            Roundwinner = player;
                            //Console.WriteLine("Condition1");
                            //nextTask = Task.RoundEnd;
                        }
                    }
                    else
                    {
                        player.status = PlayerStatus.stay;
                    }
                }
                cardTable.ShowHand(player);
                nextTask = Task.CheckForEnd;
            }
            else if (nextTask == Task.CheckForEnd)
            {
                if (!CheckActivePlayers())
                {
                    if(CheckStayPlayers() == numberOfPlayers)
                    {
                        int notTakeCards = 0;
                        int stayScore = 0;
                        foreach(Player player in players)
                        {
                            if (player.score == 0)
                            {
                                notTakeCards++;
                            } else if (player.score > stayScore)
                            {
                                stayScore = player.score;
                                Roundwinner = player;
                            }
                        }
                        if (notTakeCards == numberOfPlayers)
                        {
                            cardTable.AnnounceWinner(null);
                            foreach(Player player in players)
                            {
                                player.status = PlayerStatus.active;
                                nextTask = Task.PlayerTurn;
                            }
                        }
                        else
                        {
                            cardTable.AnnounceWinner(Roundwinner);
                            nextTask = Task.RoundEnd;
                        }
                        
                        
                        
                    }
                    else
                    {
                        Player winner = DoFinalScoring();
                        cardTable.AnnounceWinner(winner);                  
                        Roundwinner = winner;
                        nextTask = Task.RoundEnd;
                    }
                }
                else if (CheckBustPlayers() == numberOfPlayers - 1)
                {
                    foreach(Player player in players)
                    {
                        if(player.status == PlayerStatus.active)
                        {
                            cardTable.ShowHands(players);
                            cardTable.AnnounceWinner(player);
                            Roundwinner = player;
                            nextTask = Task.RoundEnd;
                        }
                    }
                }
                else if (Roundwinner != null)
                {
                    nextTask = Task.RoundEnd;
                }
                else
                {
                    currentPlayer++;
                    if (currentPlayer > players.Count - 1)
                    {
                        currentPlayer = 0; // back to the first player...
                    }
                    nextTask = Task.PlayerTurn;
                }
            }
            /* New task state to do the next round.*/
            else if (nextTask == Task.RoundEnd)
            {
                bool pointMax = false;
                foreach (Player player in players)
                {
                    if (player== Roundwinner)
                    {
                        player.points += player.score;
                    }
                    if (player.status == PlayerStatus.bust)
                    {
                        player.points -= player.score - 21;
                    }
                    if (player.points >= 100)
                    {
                        pointMax = true;
                        
                    }
                }
                cardTable.ShowPoints(players);
                if (pointMax)
                {
                    Console.WriteLine("Someone reaches the max point! Game End!");
                    nextTask = Task.GameOver;
                }
                else
                {
                    Player tempwinner = null;
                    int indextmp = -1;
                    List<int> playerToMove = new List<int>();
                    for (int i = 0; i < players.Count; i++)
                    {
                        bool ask = cardTable.AskContinue(players[i]);
                        if (!ask)
                        {
                            playerToMove.Add(i);
                            numberOfPlayers--;
                        }
                        else
                        {
                            players[i].status = PlayerStatus.active;
                            players[i].cards = new List<Card>();
                            players[i].score = 0;
                            if (players[i] == Roundwinner)
                            {
                                tempwinner = Roundwinner;
                                indextmp = i;
                            }
                        }
                    }
                    if (numberOfPlayers == 0)
                    {
                        nextTask = Task.GameOver;
                    }
                    else if (numberOfPlayers == 1)
                    {
                        Console.WriteLine(players[0].name + " wins!");
                        nextTask = Task.GameOver;
                    }
                    else
                    {
                        if (playerToMove != null)
                        {
                            foreach (int playerIndex in playerToMove)
                            {
                                players.Remove(players[playerIndex]);
                            }
                        }
                        deck = new Deck();
                        deck.Shuffle();
                        currentPlayer = 0;
                        Console.WriteLine("Shuffling players...");
                        if (tempwinner != null)
                        {
                            Player tmpter = players[players.Count - 1];
                            players[players.Count - 1] = tempwinner;
                            players[indextmp] = tmpter;
                        }
                        Random rng = new Random();
                        for (int i = 0; i < players.Count - 1; i++)
                        {
                            Player tmp = players[i];
                            int swapindex = rng.Next(players.Count - 1);
                            players[i] = players[swapindex];
                            players[swapindex] = tmp;
                        }
                        Console.WriteLine("Done.");
                        Console.WriteLine("================================");
                        cardTable.ShowPlayers(players);
                        nextTask = Task.PlayerTurn;
                    }
                    Roundwinner = null;
                }   
               
            }
            else // we shouldn't get here...
            {
                Console.WriteLine("I'm sorry, I don't know what to do now!");
                nextTask = Task.GameOver;
            }
        }

        /* Calculate the score in player's hand
         * Is called by Game object.
         * Returns int value of score
         */
        public int ScoreHand(Player player)
        {
            int score = 0;
            if (cheating == true && player.status == PlayerStatus.active)
            {
                string response = null;
                while (int.TryParse(response, out score) == false)
                {
                    Console.Write("OK, what should player " + player.name + "'s score be?");
                    response = Console.ReadLine();
                }
                return score;
            }
            else
            {
                foreach (Card card in player.cards)
                {
                    string faceValue = card.id;
                    switch (faceValue.First<char>())
                    {
                        case 'K':
                        case 'Q':
                        case 'J':
                            score = score + 10;
                            break;
                        case 'A':
                            score = score + 1;
                            break;
                        case '1':
                            score = score + 10;
                            break;
                        default:
                            score = score + int.Parse(faceValue.Substring(0,1));
                            break;
                    }
                }
            }
            return score;
        }

        /* Loop through players to check if their state is active
         * Is called by Game object.
         * Returns bool value.
         */
        public bool CheckActivePlayers()
        {
            foreach (var player in players)
            {
                if (player.status == PlayerStatus.active)
                {
                    return true; // at least one player is still going!
                }
            }
            return false; // everyone has stayed or busted, or someone won!
        }

        /* Loop through players to check the number of bust players
         * Is called by Game object.
         * Returns int value of bust players number.
         */
        public int CheckBustPlayers()
        {
            int bust = 0;
            foreach(var player in players)
            {
                if (player.status == PlayerStatus.bust)
                {
                    bust++;
                }
            }
            return bust;
        }

        /* Loop through players to check the number of stay players
         * Is called by Game object.
         * Returns int value of stay players number.
         */
        public int CheckStayPlayers()
        {
            int stay = 0;
            foreach (var player in players)
            {
                if (player.status == PlayerStatus.stay)
                {
                    stay++;
                }
            }
            return stay;
        }

        /* Loop through all players to check who got the highest score
         * Is called by Game object.
         * Returns player
         */
        public Player DoFinalScoring()
        {
            int highScore = 0;
            foreach (var player in players)
            {
                cardTable.ShowHand(player);
                if (player.status == PlayerStatus.win) // someone hit 21
                {
                    return player;
                }
                if (player.status == PlayerStatus.stay) // still could win...
                {
                    if (player.score > highScore)
                    {
                        highScore = player.score;
                    }
                }
                // if busted don't bother checking!
            }
            if (highScore > 0) // someone scored, anyway!
            {
                // find the FIRST player in list who meets win condition
                return players.Find(player => player.score == highScore);
            }
            return null; // everyone must have busted because nobody won!
        }
    }
}
