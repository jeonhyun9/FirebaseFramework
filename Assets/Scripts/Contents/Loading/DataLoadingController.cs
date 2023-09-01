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

    protected override ViewType GetViewType()
    {
        return ViewType.DataLoadingView;
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

    private void OnFinishFireBaseDataLoader()
    {
        if (FireBaseDataLoader == null)
        {
            Logger.Null("FireBaseDataLoader");
            return;
        }
        
        try
        {
            //FireBaseDataLoader.Dispose();
        
            //���� �����ִ� ��찡 ����..
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
