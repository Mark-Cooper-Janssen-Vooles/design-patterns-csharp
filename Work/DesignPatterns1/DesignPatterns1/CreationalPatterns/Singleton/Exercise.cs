using System;

namespace DesignPatterns1.CreationalPatterns.Singleton
{
    public class SingletonTester
    {
        public static bool IsSingleton(Func<object> func)
        {
            var one = func();
            var two = func();
            if (one == two)
                return true;
            
            return false;
        }
    }
}