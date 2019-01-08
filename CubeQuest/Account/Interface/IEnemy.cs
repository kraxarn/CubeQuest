using CubeQuest.Handler;

namespace CubeQuest.Account.Interface
{
	public interface IEnemy : IItem
    {
	    ImageHandler.ImageName Image { get; }

        int Level { get; set; }
    }
}