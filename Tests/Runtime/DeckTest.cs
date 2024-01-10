using Entropedia.CardSharp;
using NUnit.Framework;
using System;

namespace Entropedia.CardSharp.Tests
{
    public class DeckTest
    {
        [Test]
        public void TestCardDraw()
        {
            var deck = new Deck<Card>();
            deck.AddToTop(new Card(){ suit=Card.SuitEnum.Spades, value=Card.ValueEnum.Ace });
            var card = deck.TakeFromTop();
            Assert.IsTrue(deck.IsEmpty);
            Assert.AreEqual(deck.Length,0);
            Assert.AreEqual(card.value,Card.ValueEnum.Ace);
            Assert.AreEqual(card.suit,Card.SuitEnum.Spades);           
        }

        public Deck<Card> BuildDeck()
        {
            var deck = new Deck<Card>();
            foreach(var suit in System.Enum.GetValues(typeof(Card.SuitEnum))){
                foreach(var value in System.Enum.GetValues(typeof(Card.ValueEnum))){
                    var card  = new Card{ suit=(Card.SuitEnum)suit, value=(Card.ValueEnum)value};
                    deck.AddToTop(card);
                }
            }
            return deck;
        }

        [Test]
        public void TestCuttingJoining()
        {
            var deck = BuildDeck();
            var secondDeck = BuildDeck();
            
            Assert.AreEqual( deck.Length, 52 );
            var deck1 = deck.Cut(5);
            Assert.AreEqual( deck.Length+deck1.Length, 52 );
            var deck2 = deck.Cut(5);
            Assert.AreEqual( deck.Length+deck1.Length+deck2.Length, 52 );
            
            deck.AddToTop(deck2).AddToTop(deck1);
            Assert.AreEqual( deck.Length, 52 );
            Assert.AreEqual( deck1.Length, 0 );
            Assert.AreEqual( deck2.Length, 0 );
            
            for(var i=0;i<deck.Length;i++){
                Assert.AreEqual(secondDeck[i],deck[i]);
            }

            deck1 = deck.CutRandomly();
            deck2 = deck.CutRandomly();
            deck.AddToTop(deck2).AddToTop(deck1);

            for(var i=0;i<deck.Length;i++){
                Assert.AreEqual(secondDeck[i],deck[i]);
            }

            
        }

        [Test]
        public void TestRemovingCards()
        {
            var deck = BuildDeck();
            var secondDeck = BuildDeck();

            for(int i=0;i<deck.Length;i++){
                if(i!=0){
                    Assert.AreNotEqual(deck.Top,secondDeck.Top);
                }
                var card = deck.TakeFromTop();
                deck.AddToBottom( card );
            }
            
            for(var i=0;i<deck.Length;i++){
                Assert.AreEqual(secondDeck[i],deck[i]);
            }
        }

        [Test]
        public void TestDeckSplitting()
        {
            var deck = BuildDeck();
            var secondDeck = BuildDeck();

            deck = deck.Split(4).Join();
            for(var i=0;i<deck.Length;i++){
                Assert.AreEqual(secondDeck[i],deck[i]);
            }

            var stacks = deck.Split(4);
            var orderedByValue = new Deck<Card>();
            while(!stacks[0].IsEmpty){
                stacks.ForEach(stack=>orderedByValue.AddToTop( stack.TakeFromTop() ));
                var sameValues = orderedByValue.Cut(4);
                var value=sameValues.Top.value;
                foreach(Card card in sameValues){
                    Assert.AreEqual(value,card.value);
                }
                orderedByValue.AddToTop(sameValues);
            }
        }

        [Test]
        public void TestDealing()
        {
            var deck = BuildDeck();
            var secondDeck = BuildDeck();

            var hands = deck.Deal(4,2);
            Assert.AreEqual( 4,hands.Count );
            foreach(var hand in hands){
                Assert.AreEqual( 2,hand.Length);
            }
            Assert.AreEqual( secondDeck.Length-(4*2), deck.Length);

        }

    }

    public class Card: IEquatable<Card>
    {
        public enum SuitEnum {
            Diamonds,
            Hearts,
            Clubs,
            Spades
        }

        public enum ValueEnum 
        {
            Ace=1,
            C2=2,
            C3=3,
            C4=4,
            C5=5,
            C6=6,
            C7=7,
            C8=8,
            C9=9,
            C10=10,
            Jack=11,
            Queen=12,
            King=13
        }

        public SuitEnum suit;
        public ValueEnum value;

        public bool Equals(Card other)
        {
            return value==other.value && suit==other.suit;
        }

        public override string ToString()
        {
            return $"{value} of {suit}";            
        }
    }

}
