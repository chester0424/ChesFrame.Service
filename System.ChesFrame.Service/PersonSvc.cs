using System;
using System.ChesFrame.Entity.QueryConditon;
using System.ChesFrame.IService;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Service
{
    [ServiceBehavior(IncludeExceptionDetailInFaults=true)]
    public class PersonSvc:IPersonSvc
    {
        private BizProcessor.PersonProcessor personProcessor = new BizProcessor.PersonProcessor();
        public System.ChesFrame.Entity.PersonEntity GetBySysNo(int sysNo)
        {
            return personProcessor.GetPersonBySysNo(sysNo);
        }

        public List<System.ChesFrame.Entity.PersonEntity> GetAll()
        {
            return personProcessor.GetAll();
        }

        public List<System.ChesFrame.Entity.PersonEntity> GetByCondition()
        {
            var result =personProcessor.GetAll();
            return result;
        }

        public System.ChesFrame.Entity.PersonEntity Add(System.ChesFrame.Entity.PersonEntity person)
        {
            throw new NotImplementedException();
        }

        public void Update(System.ChesFrame.Entity.PersonEntity person)
        {
            throw new NotImplementedException();
        }

        public void Delete(System.ChesFrame.Entity.PersonEntity person)
        {
            throw new NotImplementedException();
        }

        public int GetCount()
        {
            return 6;
        }

        public QueryResponse<System.ChesFrame.Entity.PersonEntity> QueryPerson(PersonQuery query)
        {
            var result = new  QueryResponse<System.ChesFrame.Entity.PersonEntity>(){
                PageInfo = query.PageInfo,
                DataList = personProcessor.QueryPerson(query)
            };
            return result;
        }
    }
}
