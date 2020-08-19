using System.Collections.Generic;

namespace JackSParrot.Utils
{
    public static class SharedServices
    {
        static Dictionary<System.Type, object> _services = new Dictionary<System.Type, object>();

        public static void UnregisterAll()
        {
            foreach(var kvp in _services)
            {
                (kvp.Value as System.IDisposable).Dispose();
            }
            _services.Clear();
        }
        
        public static bool RegisterService<T>() where T : class, System.IDisposable, new()
        {
            return RegisterService(new T());
        }

        public static bool RegisterService<T>(T service) where T : class, System.IDisposable
        {
            if (GetService<T>() == null)
            {
                _services.Add(typeof(T), service);
                return true;
            }
            return false;
        }

        public static bool UnRegisterService<T>() where T : class, System.IDisposable
        {
            var service = GetService<T>();
            if (service != null)
            {
                service.Dispose();
                _services.Remove(typeof(T));
                return true;
            }
            return false;
        }

        public static T GetService<T>() where T : class, System.IDisposable
        {
            foreach(var kvp in _services)
            {
                var val = kvp.Value as T;
                if(val != null)
                {
                    return val;
                }
            }
            return null;
        }
    }
}

