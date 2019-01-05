using System;
using System.Threading.Tasks;

namespace CubeQuest.Battle
{
	/// <summary>
	/// Generic <see cref="IQueueAction"/>
	/// </summary>
	public class QueueAction : IQueueAction
	{
		private readonly Action action;
		
		public event EventHandler OnEnd;

		private readonly bool autoEnd;
		
		/// <summary>
		/// Generic class for a <see cref="BattleQueue"/> item
		/// </summary>
		/// <param name="executeAction">Action to be executed</param>
		/// <param name="autoEnd">Start a new thread and call <see cref="End()"/> when finished</param>
		public QueueAction(Action executeAction, bool autoEnd = false)
		{
			action = executeAction;
			this.autoEnd = autoEnd;
		}

		public void Execute()
		{
			if (autoEnd)
			{
				Task.Run(() =>
				{
					action.Invoke();
					End();
				});
			}
			else
				action.Invoke();
		}

		public void End() => 
			OnEnd?.Invoke(this, null);
	}
}