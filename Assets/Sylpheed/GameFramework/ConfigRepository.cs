using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

namespace Sylpheed.GameFramework
{
    public class ConfigRepository : MonoBehaviour
    {
        private static ConfigRepository _instance;
        
        [SerializeField] private Object[] _configs;
        
        public IReadOnlyList<Object> Configs => _configs;

        private void Awake()
        {
            Assert.IsNull(_instance, "There can only be one ConfigRepository");
            _instance = this;
        }

        public static T GetConfig<T>()
            where T : Object
        {
            var settings = _instance._configs.FirstOrDefault(s => s is T) as T;
            Assert.IsNotNull(settings, typeof(T).ToString() + " does not exist");

            return settings;
        }
    }
}
