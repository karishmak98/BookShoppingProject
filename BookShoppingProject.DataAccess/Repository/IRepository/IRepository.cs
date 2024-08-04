using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingProject.DataAccess.Repository.IRepository
{
   public interface IRepository<T> where T:class
    {
        T Get(int id);            //for find
        IEnumerable<T> GetAll(                               //for display. we can use List also.
            Expression<Func<T,bool>>filter=null,               //Query expresion
            Func<IQueryable<T>,IOrderedQueryable<T>> orderBy=null,        //for sorting, inc or dec order 
            string includeProperties=null                        //for multiple tables
            );

        T FirstOrDefault(
            Expression<Func<T,bool>>filter=null,
            string includeProperties=null       // since multible table. Category,CoverType
                                              //orderBy or sorting is not written bcz single data comes.
            );

        void Add(T entity);      //save code
        void Remove(T entity);   //delete code to remove type
        void Remove(int id);     //to remove single record
        void RemoveRange(IEnumerable<T> entity);    //to remove collection of records
          
        //Here we didn't write update code bcz of bug in this.
        //The above methods are generic method(generic repository is used).
    }
}
