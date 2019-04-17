namespace ReflectionTest
{
    public interface IObjectReflection
    {
        object GetProperty(object @object, string propertyName);
        void SetProperty(object @object, string propertyName, object value);
        void ConvertSetProperty(object @object, string propertyName, object value);
    }
    public interface IObjectReflection<TObject>
    { }
}