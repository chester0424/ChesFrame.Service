using System;
using System.ChesFrame.Entity;
using System.ChesFrame.Entity.QueryConditon;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.IDataAccess
{
    public interface IPersonDal
    {
        PersonEntity GetBySysNo(int sysNo);

        List<PersonEntity> GetAll();

        List<PersonEntity> GetByCondition(PersonQuery query);

        PersonEntity Add(PersonEntity person);

        void Update(PersonEntity person);

        void Delete(PersonEntity person);

        List<System.ChesFrame.Entity.PersonEntity> QueryPerson(PersonQuery query);
    }
}
