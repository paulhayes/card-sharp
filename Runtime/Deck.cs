using System;
using System.Collections;
using System.Collections.Generic;

namespace Entropedia.CardSharp {

public class Deck<T> : IEnumerable<T>, System.ICloneable
{
  protected List<T> m_deck;
  public event Action deckEmptyEvent;

  public bool IsEmpty
  {
    get { return m_deck.Count == 0; }
  }

  public int Length
  {
    get
    {
      return m_deck.Count;
    }
  }

  public T Top {
    get {
      if(IsEmpty){
        return default;
      }
      return m_deck[m_deck.Count-1];
    }
  }

  public T Bottom {
    get {
      if(IsEmpty){
        return default;
      }
      return m_deck[0];
    }
  }

  public Deck(List<T> deck)
  {
    this.m_deck = deck;
  }

  public void Remove(T obj)
  {
    m_deck.Remove(obj);
  }

  public Deck()
  {
    this.m_deck = new List<T>();
  }


  public Deck<T> Shuffle()
  {
    return Shuffle(new Random());
  }
  public Deck<T> Shuffle(int seed)
  {
    return Shuffle(new Random(seed));
  }

  public Deck<T> Shuffle(Random rand)
  {
    for (int i = 0; i < m_deck.Count; i++)
    {
      var tmp = m_deck[i];
      int rIndex = rand.Next(i, m_deck.Count);
      m_deck[i] = m_deck[rIndex];
      m_deck[rIndex] = tmp;
    }
    return this;
  }

  public Deck<T> Seperate(System.Predicate<T> p)
  {
    var filteredResults = m_deck.FindAll(p);
    var seperateDeck = new Deck<T>(filteredResults);
    for (int i = 0; i < m_deck.Count; i++)
    {
      if (p(m_deck[i]))
      {
        seperateDeck.AddToTop(m_deck[i]);
        m_deck.RemoveAt(i);
      }
    }
    return seperateDeck;
  }


  public List<Deck<T>> Split(int stacks)
  {
    var splitDecks = new List<Deck<T>>();
    var deckSize = m_deck.Count;
    for (int i = 0; i < stacks; i++)
    {
      //m_deck.Range();
      int len = (deckSize + (stacks - i - 1)) / stacks;
      splitDecks.Add(new Deck<T>(m_deck.GetRange(0, len)));
      m_deck.RemoveRange(0, len);
    }
    return splitDecks;
  }

  public List<Deck<T>> Deal(int numHands, int maxCardsPerHand=int.MaxValue)
  {
    var hands = new List<Deck<T>>();
    while(hands.Count<numHands){
      hands.Add(new Deck<T>());
    }

    Deal(hands,maxCardsPerHand);
    return hands;
  }

  public Deck<T> Deal(List<Deck<T>> hands, int maxCardsPerHand=int.MaxValue)
  {
    int i=0;
    int numHands = hands.Count;
    int maxCards = maxCardsPerHand*hands.Count;
    while(!IsEmpty && i<maxCards)
    {
      hands[i%numHands].AddToTop(TakeFromTop());
      i++;
    }
    return this;
  }

  public List<T> ToList()
  {
    return m_deck;
  }

    public IEnumerator GetEnumerator()
    {
      return m_deck.GetEnumerator();
    }

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return m_deck.GetEnumerator();
  }

  public Deck<T> Resize(int size)
  {
    m_deck = m_deck.GetRange(0, size);

    return this;
  }

  public Deck<T> InsertRandomly(T obj, int startIndex, int length = -1)
  {
    return InsertRandomly(new Random(), obj, startIndex, length);
  }

  public Deck<T> InsertRandomly(int seed, T obj, int startIndex, int length = -1)
  {
    return InsertRandomly(new Random(seed), obj, startIndex, length);
  }


  public Deck<T> InsertRandomly(System.Random rand, T obj, int startIndex, int length)
  {
    if (length == -1)
    {
      length = m_deck.Count;
    }
    m_deck.Insert(rand.Next(startIndex, startIndex + length), obj);
    return this;
  }

  public Deck<T> AddToTop(T obj)
  {
    m_deck.Add(obj);
    return this;
  }

  public Deck<T> AddToBottom(T obj)
  {
    m_deck.Insert(0, obj);
    return this;
  }

  public Deck<T> AddToTop(Deck<T> deck)
  {
    m_deck.AddRange(deck.ToList());
    deck.Resize(0);
    return this;
  }

  public Deck<T> AddToBottom(Deck<T> deck)
  {
    m_deck.InsertRange(0, deck.ToList());
    deck.Resize(0);
    return this;
  }


  public void ForEach(System.Action<T> action)
  {
    m_deck.ForEach(action);
  }

  public T RemoveAt(int index)
  {
    if (m_deck.Count == 0)
    {
      return default(T);
    }

    var c = m_deck[index];
    m_deck.RemoveAt(index);
    if (m_deck.Count == 0)
    {
      if (deckEmptyEvent != null)
        deckEmptyEvent.Invoke();
    }
    return c;
  }

  public T this[int index]
  {
    get
    {
      if(index<0){
        index=m_deck.Count+index;
      }
      return m_deck[index];
    }
    set
    {
      if(index<0){
        index=m_deck.Count+index;
      }
      m_deck[index] = value;
    }
  }

  public T TakeFromTop()
  {
    int index = m_deck.Count - 1;
    return RemoveAt(index);
  }

  public T TakeFromBottom()
  {
    int index = 0;
    return RemoveAt(index);
  }

  // public object Clone()
  // {
  //   return new Deck<T>(new List(m_deck));
  // }

  public Deck<T> Cut(int num)
  {
    var deck = m_deck.GetRange(m_deck.Count-num, num);
    m_deck.RemoveRange(m_deck.Count-num, num);
    return new Deck<T>(deck);
  }

  public Deck<T> CutRandomly()
  {
    return CutRandomly(new Random());
  }

  public Deck<T> CutRandomly(int seed)
  {
    return CutRandomly(new Random(seed));
  }


  public Deck<T> CutRandomly(Random random)
  {
    return Cut(random.Next(0, m_deck.Count));
  }


  public object Clone()
  {
    return Duplicate();
  }

  public Deck<T> Duplicate()
  {
    return new Deck<T>(new List<T>(m_deck));
  }

  public Deck<T> Reverse()
  {
    m_deck.Reverse();
    return this;
  }


}

public static class DeckExt
{
  public static Deck<T> Join<T>(this List<Deck<T>> target)
  {
    var bottom = new Deck<T>();
    
    while(target.Count!=0){
      bottom.AddToTop( target[0] );
      target.RemoveAt(0);
    }
    return bottom;
  }

  public static List<Deck<T>> AddToTop<T>(this List<Deck<T>> target, Deck<T> deckToAdd)
  {
    var toAdd = deckToAdd.Split(target.Count);
    int len = target.Count;
    for(int i=0;i<len;i++){
      target[i].AddToTop(toAdd[i]);
    }
    return target;
  }
  
  public static List<Deck<T>> AddToBottom<T>(this List<Deck<T>> target, Deck<T> deckToAdd)
  {
    var toAdd = deckToAdd.Split(target.Count);
    int len = target.Count;
    for(int i=0;i<len;i++){
      target[i].AddToBottom(toAdd[i]);
    }
    return target;
  }

  public static Deck<T> TakeFromTop<T>(this List<Deck<T>> target)
  {
    var deckToReturn = new Deck<T>();
    int len = target.Count;
    for(int i=0;i<len;i++){
      deckToReturn.AddToTop( target[i].TakeFromTop() );
    }
    return deckToReturn;
  }

  public static Deck<T> TakeFromBottom<T>(this List<Deck<T>> target)
  {
    var deckToReturn = new Deck<T>();
    int len = target.Count;
    for(int i=0;i<len;i++){
      deckToReturn.AddToTop( target[i].TakeFromBottom() );
    }
    return deckToReturn;
  }
  
  

}

}