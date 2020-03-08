using System.Collections.Generic;

namespace JackSParrot.Utils
{
    public class SharedServices
    {
        static SharedServices _instance;
        static SharedServices Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new SharedServices();
                }
                return _instance;
            }
        }
        public static bool RegisterService<T>() where T : class, new()
        {
            return RegisterService(new T());
        }

        public static bool RegisterService<T>(T service) where T : class
        {
            if (GetService<T>() == null)
            {
                Instance._services.Add(typeof(T), service);
                return true;
            }
            return false;
        }

        public static bool UnRegisterService<T>() where T : class
        {
            if (GetService<T>() != null)
            {
                Instance._services.Remove(typeof(T));
                return true;
            }
            return false;
        }

        public static T GetService<T>() where T : class
        {
            foreach(var kvp in Instance._services)
            {
                var val = kvp.Value as T;
                if(val != null)
                {
                    return val;
                }
            }
            return null;
        }

        Dictionary<System.Type, object> _services = new Dictionary<System.Type, object>();
        private SharedServices()
        {

        }
    }
}

