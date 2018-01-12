﻿using LearningPlatform.Domain.Common;
using System.Web;

namespace LearningPlatform.Domain.Services.Common
{
    public class RequestObjectProvider<T> : IRequestObjectProvider<T> where T : class
    {
        public T Get()
        {
            return HttpContext.Current.Items[this] as T;
        }

        public void Set(T obj)
        {
            HttpContext.Current.Items[this] = obj;
        }
    }
}
