using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingProject.DataAccess.Repository.IRepository
{
   public interface ISP_Call:IDisposable    
    {
        T Single<T>(string procedureName, DynamicParameters param = null);//for single value.
        T OneRecord<T>(string procedureName, DynamicParameters param = null);//for one record.
        void Execute(string procedureName, DynamicParameters param = null); //work for save,update,delete.
        IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null);//for display .
        Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters param = null);
                      //to get result from multiple query at one time we use tuple. 

    }
}
