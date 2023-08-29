
public class BaseStaticManager<T> where T : BaseStaticManager<T>, new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = new T();

            return instance;
        }
    }
}
