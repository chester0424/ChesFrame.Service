using System;
using System.ChesFrame.DataAccess;
using System.ChesFrame.Entity;
using System.ChesFrame.Entity.QueryConditon;
using System.ChesFrame.IDataAccess;
using System.ChesFrame.Utility.ObjectFactory;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.BizProcessor
{
    public class PersonProcessor
    {
        private IPersonDal personDal = ObjectFactory.Create<IPersonDal>();
        public PersonEntity GetPersonBySysNo(int sysNo)
        {
            return personDal.GetBySysNo(sysNo);
        }

        public List<PersonEntity> GetAll()
        {
            return personDal.GetAll();
        }

        public List<System.ChesFrame.Entity.PersonEntity> QueryPerson(PersonQuery query)
        {
            return personDal.QueryPerson(query);
        }
    }
}
