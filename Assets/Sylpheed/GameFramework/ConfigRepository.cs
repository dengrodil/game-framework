using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

namespace Sylpheed.GameFramework
{
    public class ConfigRepository : MonoBehaviour
    {
        private static ConfigRepository instance;

        public Object[] Configs;

        private void Awake()
        {
            Assert.IsNull(instance, "There can only be one ConfigRepository");
            instance = this;
        }

        public static T GetConfig<T>()
            where T : Object
        {
            T settings = instance.Configs.Where(s => s is T).FirstOrDefault() as T;
            Assert.IsNotNull(settings, typeof(T).ToString() + " does not exist");

            return settings;
        }
    }
}
