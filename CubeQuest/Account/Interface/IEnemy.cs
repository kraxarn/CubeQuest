using CubeQuest.Handler;
using fastJSON;
using Java.Lang;

namespace CubeQuest.Account.Interface
{
    public interface IEnemy : IItem
    {
	    ImageHandler.ImageName Image { get; }
    }

    public static class EnemyAsJavaString
    {
	    public static String ToJavaString(this IEnemy enemy) => 
		    new String(JSON.ToJSON(enemy));

	    public static IEnemy CreateEnemyFromJson(this Object json) => 
		    JSON.ToObject(json.ToString()) as IEnemy;
    }
}