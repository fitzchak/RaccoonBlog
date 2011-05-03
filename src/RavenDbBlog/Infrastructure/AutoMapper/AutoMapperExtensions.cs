using System;
using System.Collections;
using System.Collections.Generic;
using AutoMapper;

namespace RavenDbBlog.Infrastructure.AutoMapper
{
    public static class AutoMapperExtensions
    {
        public static List<TResult> MapTo<TResult>(this IEnumerable self)
        {
            if (self == null)
                throw new ArgumentNullException();

            return (List<TResult>) Mapper.Map(self, self.GetType(), typeof (List<TResult>));
        }

        public static TResult MapTo<TResult>(this object self)
        {
            if (self == null)
                throw new ArgumentNullException();

            return (TResult)Mapper.Map(self, self.GetType(), typeof(TResult));
        }

		public static TResult MapPropertiestoInstance<TResult>(this object self, TResult value)
		{
            if (self == null)
                throw new ArgumentNullException();

			return (TResult)Mapper.Map(self, value, self.GetType(), typeof(TResult));
		}

        public static TResult DynamicMapTo<TResult>(this object self)
        {
            if (self == null)
                throw new ArgumentNullException();

            return (TResult)Mapper.DynamicMap(self, self.GetType(), typeof(TResult));
        }
    }
}