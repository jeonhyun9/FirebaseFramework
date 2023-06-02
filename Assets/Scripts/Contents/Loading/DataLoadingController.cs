using UnityEngine;

public class DataLoadingController : BaseController<DataLoadingView,BaseDataLoader>
{
    public enum LoadDataType
    {
        LocalPath,
        FireBase,
    }

    public bool IsSuccess => Model?.CurrentState == BaseDataLoader.State.Success;

    private LocalDataLoader LocalDataLoader => Model.GetLoader<LocalDataLoader>();

    private FireBaseDataLoader FireBaseDataLoader => Model.GetLoader<FireBaseDataLoader>();

    protected override string GetViewPrefabName()
    {
        return nameof(DataLoadingController).Replace("Controller", "View");
    }

    protected override void Enter()
    {
        if (LocalDataLoader != null)
        {
            LocalDataLoader.SetOnSuccessLoadData(OnSuccessDataLoader);
            LocalDataLoader.LoadData().Forget();
        }
        else if (FireBaseDataLoader != null)
        {
            FireBaseDataLoader.SetOnFinishLoadData(OnFinishFireBaseDataLoader);
            FireBaseDataLoader.SetOnSuccessLoadData(OnSuccessDataLoader);
            FireBaseDataLoader.LoadData().Forget();
        }
    }

    public LocalDataLoader CreateLocalDataLoader()
    {
        LocalDataLoader localDataLoader = new();
        return localDataLoader;
    }

    public FireBaseDataLoader CreateFireBaseDataLoader()
    {
        FireBaseDataLoader fireBaseDataLoader = new();
        return fireBaseDataLoader;
    }

    private void OnFinishFireBaseDataLoader()
    {
        if (FireBaseDataLoader == null)
        {
            Logger.Null("FireBaseDataLoader");
            return;
        }
        
        try
        {
            if (FireBaseDataLoader.Storage.App != null)
                FireBaseDataLoader.Storage.App.Dispose();
        
            Firebase.FirebaseApp.DefaultInstance.Dispose();
        
            //씬에 남아있는 경우가 있음..
            GameObject go = GameObject.Find("Firebase Services");
        
            if (go != null)
            {
                Logger.Log("Firebase Services destroyed.");
                Object.Destroy(go);
            }
        }
        catch (System.Exception e)
        {
            Logger.Error(e.ToString());
        }
    }

    private void OnSuccessDataLoader()
    {
        DataManager.Instance.AddDataContainerByDataDic(Model.DicJsonByFileName);
    }


}
