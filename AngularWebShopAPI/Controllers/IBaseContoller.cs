using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Api.Controllers
{
   public interface IBaseContoller<T>
    {
        Expression <Func<T,object>> wsPost { get; }
    }
}
