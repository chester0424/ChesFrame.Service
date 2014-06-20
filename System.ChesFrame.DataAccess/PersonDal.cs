using System;
using System.ChesFrame.Entity;
using System.ChesFrame.Entity.QueryConditon;
using System.ChesFrame.IDataAccess;
using System.ChesFrame.Utility.DataAccess;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.DataAccess
{
    public class PersonDal : IPersonDal
    {
        public PersonEntity GetBySysNo(int sysNo)
        {
            DbCommand dbcommand = DbCommandManager.GetDbCommand("Person_GetBySysNo");
            new SqlBuilder(dbcommand).SetDbParameterValue("@SysNo", sysNo);
            var person = dbcommand.ExecuteEntity<PersonEntity>();
            return person;
        }

        public List<PersonEntity> GetAll()
        {
            DbCommand dbcommand = DbCommandManager.GetDbCommand("Person_GetAll");
            var persons = dbcommand.ExecuteEntityList<PersonEntity>();
            return persons;
        }

        public List<PersonEntity> GetByCondition(PersonQuery query)
        {
            DbCommand dbcommand = DbCommandManager.GetDbCommand("Person_GetAll");

            return null;
        }

        public PersonEntity Add(PersonEntity person)
        {
            DbCommand dbcommand = DbCommandManager.GetDbCommand("Person_Add");
            int sysNo= dbcommand.ExecuteScalar<int>();
            person.SysNo = sysNo;
            return person;
        }

        public void Update(PersonEntity person)
        {
            DbCommand dbcommand = DbCommandManager.GetDbCommand("Person_Update");
            int sysNo = dbcommand.ExecuteNonQuery();
        }

        public void Delete(PersonEntity person)
        {
            DbCommand dbcommand = DbCommandManager.GetDbCommand("Person_Delete");
            int sysNo = dbcommand.ExecuteNonQuery();
        }


        public List<PersonEntity> QueryPerson(Entity.QueryConditon.PersonQuery query)
        {
            DbCommand dbcommand = DbCommandManager.GetDbCommand("Person_QueryPerson");
            //添加条件
            new SqlBuilder(dbcommand).AddPageInfo(query.PageInfo)
                .AddCondition("Name", OperationType.Equal, "@Name", Data.DbType.String, query.Name)
                .AddCondition("Age", OperationType.Equal, "@Age", Data.DbType.Int32, query.Age)
                .AddCondition("Phone", OperationType.Equal, "@Phone", Data.DbType.Int32, query.Phone)
                .MakeCondition4DbCommand();
            var result =dbcommand.ExecuteEntityList<PersonEntity>();
            //总记录数
            query.PageInfo.Count = dbcommand.GetTotalCount();
            return result;
        }
    }
}
