using Cysharp.Threading.Tasks;

public class GameManager : BaseMonoManager<GameManager>
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}