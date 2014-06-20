using System;
using System.ChesFrame.Entity;
using System.ChesFrame.IService;
using System.ChesFrame.Utility.WCF;
using System.ChesFrame.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ChesFrame.Web.Model;
using System.ChesFrame.Entity.QueryConditon;

namespace System.ChesFrame.Web.Facade
{
    public class PersonFacade
    {
        #region Person

        public static List<PersonModel> GetPersonByPage(PersonQueryModel condition)
        {
            IPersonSvc proxy = ServiceFactory<IPersonSvc>.Instace;
            var result = proxy.GetByCondition();
            List<PersonModel> target = new List<PersonModel>();
            foreach (var sub in result)
            {
                target.Add(sub.AssignValueTo<PersonModel>());
            }
            return target;
        }

        public static PersonModel GetPersonBySysNo(int sysNo)
        {
            IPersonSvc proxy = ServiceFactory<IPersonSvc>.Instace;
            var serviceModel = proxy.GetBySysNo(sysNo);
            return serviceModel.AssignValueTo<PersonModel>();
        }

        public static List<PersonModel> GetAllPerson()
        {
            IPersonSvc proxy = ServiceFactory<IPersonSvc>.Instace;
            var persons = proxy.GetAll();
            List<PersonModel> target = new List<PersonModel>();
            foreach (var sub in persons)
            {
                target.Add(sub.AssignValueTo<PersonModel>());
            }
            return target;
        }

        public static int GetCount()
        {
            IPersonSvc proxy = ServiceFactory<IPersonSvc>.Instace;
            var count = proxy.GetCount();
            return count;
        }

        public static List<PersonModel> QueryPerson(PersonQueryModel queryModel)
        {
            IPersonSvc proxy = ServiceFactory<IPersonSvc>.Instace;
            //对象属性赋值转换
            var personQuery = queryModel.AssignValueToByAutoMapper<PersonQueryModel,PersonQuery>();
            //如果获取到的字符串是""，则转换成null,("",null 代表不同的意义)
            personQuery.Name = personQuery.Name.EmptyToNull();
            personQuery.Phone = personQuery.Phone.EmptyToNull();
            
            var response = proxy.QueryPerson(personQuery);

            var result = response.DataList.AssignValueToByAutoMapper<PersonEntity, PersonModel>();
            queryModel.PageInfo.Count = response.PageInfo.Count;

            return result;
        }

        #endregion
    }
}