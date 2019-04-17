using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionTest
{
    public class TestPerformance
    {
        public async Task<long> TestReflection(int mode, int numberOfObjects)
        {
            List<MyClass> myClassList = Enumerable.Repeat(new MyClass(), numberOfObjects).ToList();
            IObjectReflection cache = null;

            if (mode == 1)
                cache = ObjectReflectionService.GetInstace(typeof(MyClass), typeof(DefaultObjectReflectionProvider<>));
            else if (mode == 2)
                cache = ObjectReflectionService.GetInstace(typeof(MyClass), typeof(DelegateObjectReflectionProvider<>));

            object aux = 0;

            var stopwatch = new Stopwatch();

            foreach (var obj in myClassList)
            {
                switch (mode)
                {
                    case 1:
                    case 2:
                        stopwatch.Start();
                        aux = cache.GetProperty(obj, "Test0");
                        cache.ConvertSetProperty(obj, "Test0", 123);
                        aux = cache.GetProperty(obj, "Test1");
                        cache.ConvertSetProperty(obj, "Test1", "123");
                        aux = cache.GetProperty(obj, "Test2");
                        cache.ConvertSetProperty(obj, "Test2", "123");
                        aux = cache.GetProperty(obj, "Test3");
                        cache.ConvertSetProperty(obj, "Test3", "2019-04-16");
                        aux = cache.GetProperty(obj, "Test4");
                        cache.ConvertSetProperty(obj, "Test4", "9EA9C2BA-CF59-46EB-BE4D-E6002DFBCD3B");
                        stopwatch.Stop();

                        break;

                    default:
                        stopwatch.Start();
                        aux = obj.Test0;
                        obj.Test0 = (string)ConvertTo(123, typeof(string));
                        aux = obj.Test1;
                        obj.Test1 = (int)ConvertTo("123", typeof(int));
                        aux = obj.Test2;
                        obj.Test2 = (decimal)ConvertTo("123", typeof(decimal));
                        aux = obj.Test3;
                        obj.Test3 = (DateTime)ConvertTo("2019-04-16", typeof(DateTime));
                        aux = obj.Test4;
                        obj.Test4 = (Guid)ConvertTo("9EA9C2BA-CF59-46EB-BE4D-E6002DFBCD3B", typeof(Guid));
                        stopwatch.Stop();

                        break;
                }
            }

            stopwatch.Stop();
            return stopwatch.ElapsedTicks;
        }

        public void CacheReflection()
        {
            ObjectReflectionService.GetInstace(typeof(MyClass), typeof(DelegateObjectReflectionProvider<>));
            ObjectReflectionService.GetInstace(typeof(MyClass), typeof(DefaultObjectReflectionProvider<>));
        }

        static object ConvertTo(object @object, Type convertType)
        {
            if (convertType == typeof(string))
                return Convert.ToString(@object);
            else if (convertType == typeof(int))
                return Convert.ToInt32(@object);
            else if (convertType == typeof(decimal))
                return Convert.ToDecimal(@object);
            else if (convertType == typeof(DateTime))
                return Convert.ToDateTime(@object);
            else if (convertType == typeof(Guid))
                return Guid.Parse(@object.ToString());

            return null;
        }
    }

    class MyClass
    {
        public string Test0 { get; set; }
        public int Test1 { get; set; }
        public decimal Test2 { get; set; }
        public DateTime Test3 { get; set; }
        public Guid Test4 { get; set; }
        public Guid Test5 { get; set; }
    }
}
