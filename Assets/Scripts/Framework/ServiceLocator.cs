using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Framework.Core
{
    /// <summary>
    /// 服务定位器，提供注册、获取和注销服务的方法
    /// 服务全局入口，允许不同系统之间解耦和共享服务实例
    /// </summary>
    public class ServiceLocator
    {
        private static readonly Dictionary<Type, object> services = new();
        public static void Register<T>(T service)
        {
            var type = typeof(T);
            services[type] = service;

        }
        public static T Get<T>()
        {
            var type = typeof(T);
            if (services.TryGetValue(type, out var service))
            {
                return (T)service;
            }
            throw new Exception($"Service of type {type} not found");
        }
        public static bool TryGet<T>(out T service)
        {
            var type = typeof(T);
            if (services.TryGetValue(type, out var obj))
            {
                service = (T)obj;
                return true;
            }
            service = default;
            return false;
        }
        public static void Unregister<T>()
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                services.Remove(type);
            }
        }
        public static void Clear()
        {
            services.Clear();
        }
    }
}