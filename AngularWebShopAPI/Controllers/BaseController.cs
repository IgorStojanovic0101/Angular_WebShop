using Api.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace Api.Controllers
{
    public class BaseController<T> : ApiController, IBaseContoller<T>
    {
        public Expression<Func<T, object>> wsPost { get; private set; }

        public R Call<R>(Expression<Func<Products, object>> p)
        {
          //  p.Compile().Method.
          //  var param = Expression.Parameter(typeof(T), "x");

          //  p.Compile().DynamicInvoke(param);
            //   Expression.Call(null,((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(T) }));  
            //  R result = compole();
        //    MethodCallExpression expression = p as MethodCallExpression;
            object result = Expression.Lambda(p).Compile().DynamicInvoke();
            var methodName = ((MethodCallExpression)p.Body).Method.Name;
            var methodInfo = typeof(T).GetMethod(methodName);
            var instance = Expression.Parameter(typeof(T), "instance");
            var call = Expression.Call(instance, methodInfo);
            var lambda = Expression.Lambda<Func<T, R>>(call, instance).Compile();
           // return lambda(instance);

            return (R)result;
        }
    }
}