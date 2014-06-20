using System;
using System.ChesFrame.Entity;
using System.ChesFrame.Entity.QueryConditon;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.IService
{
    [ServiceContract]
    public interface IPersonSvc
    {
        [OperationContract]
        PersonEntity GetBySysNo(int sysNo);

        [OperationContract]
        List<PersonEntity> GetAll();

        [OperationContract]
        List<PersonEntity> GetByCondition();

        [OperationContract]
        PersonEntity Add(PersonEntity person);

        [OperationContract]
        void Update(PersonEntity person);

        [OperationContract]
        void Delete(PersonEntity person);
        [OperationContract]
        int GetCount();
        [OperationContract]
        QueryResponse<PersonEntity> QueryPerson(PersonQuery query);
    }
}
