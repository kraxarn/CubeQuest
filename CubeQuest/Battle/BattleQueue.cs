using System;
using System.Collections.Generic;
using System.Linq;

namespace CubeQuest.Battle
{
	public class BattleQueue
	{
		public delegate void OnQueueEndEvent();

		/// <summary>
		/// When the queue is done executing
		/// </summary>
		public event OnQueueEndEvent OnQueueEnd;

		/// <summary>
		/// Queue of all items
		/// </summary>
		private readonly Queue<IQueueAction> queue;

		/// <summary>
		/// If we're executing the current queue
		/// </summary>
		private bool executing;

		/// <summary>
		/// Create a new battle queue with an empty queue
		/// </summary>
		public BattleQueue() => 
			queue = new Queue<IQueueAction>();

		/// <summary>
		/// Create a new battle queue and insert some items into the queue
		/// </summary>
		/// <param name="items"></param>
		public BattleQueue(IEnumerable<IQueueAction> items) => 
			queue = new Queue<IQueueAction>(items);

		/// <summary>
		/// Add one item to the queue
		/// </summary>
		public void Add(IQueueAction item) => 
			queue.Enqueue(item);

		/// <summary>
		/// Add multiple items to the queue
		/// </summary>
		public void Add(List<IQueueAction> items) => 
			items.ForEach(i => queue.Enqueue(i));
        

		/// <summary>
		/// Start executing the queue
		/// </summary>
		/// <returns>If the queue was started</returns>
		public bool Execute()
		{
			// Return false if already executing queue
			if (executing)
				return false;

			if (!queue.Any())
				return false;

			// Start from the beginning
			executing = true;

			// Start queue
			var action = queue.Dequeue();
			action.OnEnd += OnEnd;
			action.Execute();

			return true;
		}

		private void OnEnd(object sender, EventArgs e)
		{
			if (queue.TryDequeue(out var action))
			{
				// Next item in the queue
				action.OnEnd += OnEnd;
				action.Execute();
			}
			else
			{
				// Done with the queue
				executing = false;
				OnQueueEnd?.Invoke();
			}
		}
	}
}