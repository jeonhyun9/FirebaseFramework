public class TestUnitModel<T> : IBaseUnitModel where T : IBaseData
{
    public readonly T Data;

    public TestUnitModel(T data)
    {
        Data = data;
    }
}
