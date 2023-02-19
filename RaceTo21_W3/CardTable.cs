using System;
using System.Collections.Generic;
using System.Linq;

namespace RaceTo21
{
    public class CardTable
    {
        public CardTable()
        {
            Console.WriteLine("Setting Up Table...");
        }

        /* Shows the name of each player and introduces them by table position.
         * Is called by Game object.
         * Game object provides list of players.
         * Calls Introduce method on each player object.
         */
        public void ShowPlayers(List<Player> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].Introduce(i+1); // List is 0-indexed but user-friendly player positions would start with 1...
            }
        }

        /* Gets the user input for number of players.
         * Is called by Game object.
         * Returns number of players to Game object.
         */
        public int GetNumberOfPlayers()
        {
            Console.Write("How many players? ");
            string response = Console.ReadLine();
            int numberOfPlayers;
            while (int.TryParse(response, out numberOfPlayers) == false
                || numberOfPlayers < 2 || numberOfPlayers > 8)
            {
                Console.WriteLine("Invalid number of players.");
                Console.Write("How many players?");
                response = Console.ReadLine();
            }
            return numberOfPlayers;
        }

        /* Gets the name of a player
         * Is called by Game object
         * Game object provides player number
         * Returns name of a player to Game object
         */
        public string GetPlayerName(int playerNum)
        {
            Console.Write("What is the name of player# " + playerNum + "? ");
            string response = Console.ReadLine();
            while (response.Length < 1)
            {
                Console.WriteLine("Invalid name.");
                Console.Write("What is the name of player# " + playerNum + "? ");
                response = Console.ReadLine();
            }
            return response;
        }

        /* Ask player if draw a card or stay
         * Is called by Game object.
         * Returns bool value of players decision.
         */
        public bool OfferACard(Player player)
        {
            while (true)
            {
                Console.Write(player.name + ", do you want card(s)? (Y/N)");
                string response = Console.ReadLine();
                if (response.ToUpper().StartsWith("Y"))
                {
                    return true;
                }
                else if (response.ToUpper().StartsWith("N"))
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Please answer Y(es) or N(o)!");
                }
            }
        }

        /* Gets the player's card
         * Is called by ShowHands function.
         * No return value
         */
        public void ShowHand(Player player)
        {
            if (player.cards.Count > 0)
            {
                Console.Write(player.name + " has: ");
                foreach (Card card in player.cards.Take(player.cards.Count() - 1))
                {
                    Console.Write(card.cardName + ", ");
                }
                Console.Write(player.cards[player.cards.Count - 1].cardName);
                Console.Write(" =" + player.score + "/21 ");
                if (player.status != PlayerStatus.active)
                {
                    Console.Write("(" + player.status.ToString().ToUpper() + ")");
                }
                Console.WriteLine();
            }
        }

        /* Go through each player and get their cards
         * Is called by Game object.
         * No return value
         */
        public void ShowHands(List<Player> players)
        {
            foreach (Player player in players)
            {
                ShowHand(player);
            }
        }

        /* Show each player's points
         * Is called by Game object.
         * No return value
         */
        public void ShowPoints(List<Player> players)
        {
            foreach (Player player in players)
            {
                Console.WriteLine(player.name + "'s points: "+ player.points);
            }
        }

        /* Output the winner's name
         * Is called by Game object.
         * No return value
         */
        public void AnnounceWinner(Player player)
        {
            if (player != null)
            {
                Console.WriteLine(player.name + " wins!");
            }
            else
            {
                Console.WriteLine("Everyone busted!");
            }
            //Console.Write("Press <Enter> to exit... ");
            //while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }


        /* Gets the user input for whether to continue.
         * Is called by Game object.
         * Returns bool value of players decision.
         */
        public bool AskContinue(Player player)
        {
            while (true)
            {
                Console.Write(player.name + ", do you want to continue playing? (Y/N)");
                string response = Console.ReadLine();
                if (response.ToUpper().StartsWith("Y"))
                {
                    return true;
                }
                else if (response.ToUpper().StartsWith("N"))
                {
                    Console.WriteLine("said No");
                    return false;
                }
                else
                {
                    Console.WriteLine("Please answer Y(es) or N(o)!");
                }
            }
        }

        /* Gets the user input for number of cards to draw.
         * Is called by Game object.
         * Returns number of cards to Game object.
         */
        public int GetNumberOfCardstoDraw()
        {
            Console.Write("How many cards do you want? (1-3)");
            string response = Console.ReadLine();
            int number;
            while (int.TryParse(response, out number) == false
                || number < 1 || number > 3)
            {
                Console.WriteLine("Invalid number of cards.");
                Console.Write("How many cards do you want? (1-3)");
                response = Console.ReadLine();
            }
            return number;
        }


    }
}