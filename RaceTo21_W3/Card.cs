using System;
using System.Collections.Generic;


namespace RaceTo21
{
    public class Card
    {
        public string id;
        public string cardName;



        public Card(string shortCardName, string longCardName)
        {
            id = shortCardName;
            cardName = longCardName;
        }
    }
}