using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DafByte.Utils.VisualCentroid
{
	public class PriorityQueue<T> : IEnumerable<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PriorityQueue&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="items">
		/// The items to initialize the priority queue with.
		/// </param>
		/// <param name="comparer"></param>
		public PriorityQueue(IEnumerable<T> items, Comparer<T> comparer)
			: this(comparer)
		{
			Enqueue(items);
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="PriorityQueue&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="comparer"></param>
		public PriorityQueue(Comparer<T> comparer)
		{
			if (null == comparer)
				throw new ArgumentNullException(@"Argument `comparer` must contain a value.");

			_items = new LinkedList<T>();
			_comparer = comparer;
		}


		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Gets the enumerator for the priority queue.
		/// </summary>
		/// <returns>
		/// The enumerator for the priority queue.
		/// </returns>
		public IEnumerator<T> GetEnumerator()
		{
			return _items.GetEnumerator();
		}


		/// <summary> Adds an item to the priority queue. </summary>
		/// <param name="item"> The item to add. </param>
		public void Enqueue(T item)
		{
			Insert(item);
		}
		/// <summary>
		///     Adds the items to the priority queue.  This method checks if the enumerable is null 
		///     and only iterates of the items once.
		/// </summary>
		/// <param name="itemsToAdd"> An IEnumerable&lt;T&gt; of items to add to the priority queue. </param>
		public void Enqueue(IEnumerable<T> itemsToAdd)
		{
			if (itemsToAdd == null)
				throw new ArgumentNullException(@"Argument `itemsToAdd` must contain a value.");

			foreach (var item in itemsToAdd)
				Insert(item);
		}

		/// <summary> Pops an item from the front of the queue. </summary>
		/// <exception cref="InvalidOperationException">
		/// Thrown when no items exist in the priority queue.
		/// </exception>
		/// <returns> An item from the front of the queue. </returns>
		public T Dequeue()
		{
			if (IsEmpty)
				throw new InvalidOperationException("No elements exist in the queue");

			var item = _items.First;
			_items.RemoveFirst();
			return item.Value;
		}
		/// <summary> Pops the specified number of items from the front of the queue. </summary>
		/// <exception cref="ArgumentException">
		/// Thrown when the number of items to pop exceeds the number of items in the priority queue.
		/// </exception>
		/// <param name="numberToPop"> Number of items to pop from the front of the queue. </param>
		/// <returns> The items from the front of the queue. </returns>
		public IEnumerable<T> Dequeue(int numberToPop)
		{
			if (numberToPop > _items.Count)
				throw new ArgumentException(@"The `numberToPop` exceeds the number of elements in the queue");

			var poppedItems = new List<T>();
			while (poppedItems.Count < numberToPop)
				poppedItems.Add(Dequeue());

			return poppedItems;
		}

		/// <summary>
		/// Clears all the items from the priority queue.
		/// </summary>
		public void Clear()
		{
			_items.Clear();
		}


		/// <summary>
		/// Inserts the given item into the queue.
		/// </summary>
		/// <param name="item">
		/// The item to insert into the queue.
		/// </param>
		private void Insert(T item)
		{
			if (IsEmpty)
			{
				_items.AddFirst(item);
				return;
			}

			var tail = _items.Last.Value;
			var comparedToTail = _comparer.Compare(item, tail);

			if (comparedToTail <= 0) // Less or equal to the than the current minimum
			{
				_items.AddLast(item);
				return;
			}

			// Logical else: Queue contains at least one item with lower priority.
			for (var node = _items.First; null != node; node = node.Next)
			{
				// ReSharper disable once InvertIf
				if (_comparer.Compare(item, node.Value) > 0)
				{
					_items.AddBefore(node, item);
					return;
				}
			}
		}


		/// <summary>
		/// Peeks at the item at the front queue without removing the item.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// Thrown when no items exist in the priority queue.
		/// </exception>
		/// <returns>
		/// The item at the front of the queue.
		/// </returns>
		public T Front
		{
			get
			{
				if (IsEmpty)
					throw new InvalidOperationException("No elements exist in the queue");

				return _items.First.Value;
			}
		}

		/// <summary>
		/// Gets the number of items that are in the priority queue. 
		/// </summary>
		/// <value> 
		/// The number of items in the priority queue.
		/// </value>
		public int Count
		{
			get
			{
				return _items.Count;
			}
		}

		/// <summary>
		/// Gets whether the priority queue is empty. 
		/// </summary>
		/// <returns>
		/// true if the priority queue empty, false otherwise.
		/// </returns>
		public bool IsEmpty
		{
			get
			{
				return !_items.Any();
			}
		}


		private readonly LinkedList<T> _items;
		private readonly Comparer<T> _comparer;
	}
}
