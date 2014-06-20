using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.Utility.DataAccess
{
    public class SqlBuilder
    {
        private DbCommand dbCommand = null;

        private List<SqlCondition> conditions = new List<SqlCondition>();

        public SqlBuilder(DbCommand dbCommand)
        {
            this.dbCommand = dbCommand;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageInfo"></param>
        /// <returns></returns>
        public SqlBuilder AddPageInfo(Pagination pageInfo)
        {
            var commandText = dbCommand.CommandText;
            //脚本中可以使用“#....#”代表条件，也可以直接使用参数（形如：@PageSize）
            commandText = commandText.Replace("#PageSize#", SqlConst.PageSize);
            commandText = commandText.Replace("#RowFrom#", SqlConst.RowFrom);
            commandText = commandText.Replace("#RowTo#", SqlConst.RowTo);
            commandText = commandText.Replace("#OrderBy#", SqlConst.OrderBy);
            commandText = commandText.Replace("#TotalCount#", SqlConst.TotalCount);

            dbCommand.CommandText = commandText;

            var pageSize = dbCommand.CreateParameter();
            pageSize.DbType = Data.DbType.Int32;
            pageSize.ParameterName = SqlConst.PageSize;
            pageSize.Size = 4;
            pageSize.Value = pageInfo.PageSze;
            dbCommand.Parameters.Add(pageSize);

            var rowFrom = dbCommand.CreateParameter();
            rowFrom.DbType = Data.DbType.Int32;
            rowFrom.ParameterName = SqlConst.RowFrom;
            rowFrom.Size = 4;
            rowFrom.Value = pageInfo.PageIndex * pageInfo.PageSze;
            dbCommand.Parameters.Add(rowFrom);

            var rowTo = dbCommand.CreateParameter();
            rowTo.DbType = Data.DbType.Int32;
            rowTo.ParameterName = SqlConst.RowTo;
            rowTo.Size = 4;
            rowTo.Value = (pageInfo.PageIndex+1) * pageInfo.PageSze;
            dbCommand.Parameters.Add(rowTo);

            var orderBy = dbCommand.CreateParameter();
            orderBy.DbType = Data.DbType.String;
            orderBy.ParameterName = SqlConst.OrderBy;
            orderBy.Size = 50;
            orderBy.Value = pageInfo.OrderBy;
            dbCommand.Parameters.Add(orderBy);

            var totalCount = dbCommand.CreateParameter();
            totalCount.DbType = Data.DbType.Int32;
            totalCount.ParameterName = SqlConst.TotalCount;
            totalCount.Direction = ParameterDirection.Output;
            dbCommand.Parameters.Add(totalCount);

            return this;
        }

        /// <summary>
        /// 添加条件
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public SqlBuilder AddCondition(SqlCondition condition)
        {
            conditions.Add(condition);
            return this;
        }

        /// <summary>
        /// 添加条件
        /// </summary>
        /// <param name="conditionConnectionType"></param>
        /// <param name="sql"></param>
        /// <param name="operationType"></param>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlBuilder AddCondition(ConditionConnectionType conditionConnectionType, string sql,
            OperationType operationType, string parameterName, DbType dbType, int? size, object value)
        {
            if (value == null) { return this; }//如果为空则不加条件
            SqlCondition sqlCondition = new SqlCondition()
            {
                ConditionConnectionType = conditionConnectionType,
                Sql = sql,
                OperationType = operationType,
                ParameterName = parameterName,
                Size = size,
                Value = value
            };
            conditions.Add(sqlCondition);
            return this;
        }

        public SqlBuilder AddCondition(string sql,
            OperationType operationType, string parameterName, DbType dbType, int size, object value)
        {
            return AddCondition(ConditionConnectionType.And, sql, operationType, parameterName, dbType, size, value);
        }

        public SqlBuilder AddCondition(ConditionConnectionType conditionConnectionType, string sql,
           OperationType operationType, string parameterName, DbType dbType, object value)
        {
            return AddCondition(ConditionConnectionType.And, sql, operationType, parameterName, dbType,null, value);
        }

        public SqlBuilder AddCondition( string sql,OperationType operationType, string parameterName, DbType dbType, object value)
        {
            return AddCondition(ConditionConnectionType.And, sql, operationType, parameterName, dbType, value);
        }

        /// <summary>
        /// 根据添加的条件，生成脚本（脚本和参数）
        /// </summary>
        public SqlBuilder MakeCondition4DbCommand()
        {
            List<string> conditionStrList = new List<string>();
            List<DbParameter> dbParameters = new List<DbParameter>();
            if (conditions.Count > 0)
            {
                foreach (var sub in conditions)
                {
                    //1.Sql模板，2.参数DbParameter

                    switch (sub.OperationType)
                    {
                        case OperationType.Between:
                            var value1 = (dynamic)sub.Value;
                            DbParameter p1 = dbCommand.CreateParameter();
                            DbParameter p2 = dbCommand.CreateParameter();
                            SetDbParameter(p1, sub.DbType, sub.Size, sub.ParameterName + "1", sub.Value);
                            SetDbParameter(p1, sub.DbType, sub.Size, sub.ParameterName + "2", sub.Value);

                            var sqlStrModelBetween = string.Format(sub.OperationType.GetTypeModel(), sub.Sql, sub.ParameterName + "1", sub.ParameterName + "2");

                            conditionStrList.Add(string.Format(" {0} {1} ", sub.ConditionConnectionType.Tostr(), sqlStrModelBetween));
                            dbParameters.Add(p1);
                            dbParameters.Add(p2);
                            break;
                        case OperationType.In:
                            var value = string.Join<object>(",", (IEnumerable<Object>)sub.Value);
                            var sqlStrModelIn = string.Format(sub.OperationType.GetTypeModel(), sub.Sql, string.Format("({0})", value));
                            conditionStrList.Add(string.Format(" {0} {1} ", sub.ConditionConnectionType.Tostr(), sqlStrModelIn));
                            break;
                        case OperationType.Like:
                            var sqlStrModelLike = string.Format(sub.OperationType.GetTypeModel(), sub.Sql, sub.ParameterName);
                            DbParameter pLike = dbCommand.CreateParameter();
                            SetDbParameter(pLike, sub.DbType, sub.Size, sub.ParameterName, string.Format("%{0}%", sub.Value));
                            conditionStrList.Add(string.Format(" {0} {1} ", sub.ConditionConnectionType.Tostr(), sqlStrModelLike));
                            dbParameters.Add(pLike);
                            break;
                        case OperationType.LeftLike:
                            var sqlStrModelLeftLike = string.Format(sub.OperationType.GetTypeModel(), sub.Sql, sub.ParameterName);
                            DbParameter pLeftLike = dbCommand.CreateParameter();
                            SetDbParameter(pLeftLike, sub.DbType, sub.Size, sub.ParameterName, string.Format("{0}%", sub.Value));
                            conditionStrList.Add(string.Format(" {0} {1} ", sub.ConditionConnectionType.Tostr(), sqlStrModelLeftLike));
                            dbParameters.Add(pLeftLike);
                            break;
                        case OperationType.RightLike:
                            var sqlStrModelRightLike = string.Format(sub.OperationType.GetTypeModel(), sub.Sql, sub.ParameterName);
                            DbParameter pRightLike = dbCommand.CreateParameter();
                            SetDbParameter(pRightLike, sub.DbType, sub.Size, sub.ParameterName, string.Format("%{0}", sub.Value));
                            conditionStrList.Add(string.Format(" {0} {1} ", sub.ConditionConnectionType.Tostr(), sqlStrModelRightLike));
                            dbParameters.Add(pRightLike);
                            break;

                        default:
                            DbParameter p = dbCommand.CreateParameter();
                            SetDbParameter(p, sub.DbType, sub.Size, sub.ParameterName, sub.Value);
                            var sqlStrModel = string.Format(sub.OperationType.GetTypeModel(), sub.Sql, sub.ParameterName);
                            conditionStrList.Add(string.Format(" {0} {1} ", sub.ConditionConnectionType.Tostr(), sqlStrModel));
                            dbParameters.Add(p);
                            break;
                    }
                }

                var sqlStr = string.Join("", conditionStrList);
                if (sqlStr.IndexOf(ConditionConnectionType.And.Tostr()) > 0)
                {
                    sqlStr = sqlStr.Substring(sqlStr.IndexOf(ConditionConnectionType.And.Tostr()) + ConditionConnectionType.And.Tostr().Length);
                }
                if (sqlStr.IndexOf(ConditionConnectionType.Or.Tostr()) > 0)
                {
                    sqlStr = sqlStr.Substring(sqlStr.IndexOf(ConditionConnectionType.Or.Tostr()) + ConditionConnectionType.Or.Tostr().Length);
                }
                dbCommand.CommandText = dbCommand.CommandText.Replace("#Where#", string.Format("WHERE {0}", sqlStr));
                dbCommand.Parameters.AddRange(dbParameters.ToArray());

                //清除缓存
                conditions.Clear();
                conditionStrList.Clear();
            }
            else 
            {
                dbCommand.CommandText = dbCommand.CommandText.Replace("#Where#", "");
            }

            return this;
        }

        /// <summary>
        /// 为配置的参数赋值
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlBuilder SetDbParameterValue(string parameterName, object value)
        {
            dbCommand.Parameters[parameterName].Value = value;
            return this;
        }

        /// <summary>
        /// 为指定的DbParameter赋值
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlBuilder SetDbParameter(DbParameter parameter, DbType dbType,
            int? size, string parameterName, object value)
        {
            parameter.DbType = dbType;
            if (size.HasValue) { parameter.Size = size.Value; }
            parameter.ParameterName = parameterName;
            parameter.Value = value;

            return this;
        }

        /// <summary>
        /// 为参数赋值，如果参数列表中没有该参数，则增加
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlBuilder SetDbParameter(string parameterName, DbType? dbType,
            int? size,  object value)
        {
            DbParameter targetParameter = null;
            if (dbCommand.Parameters != null)
            {
                if(dbCommand.Parameters.Contains(parameterName))
                {
                     targetParameter = dbCommand.Parameters[parameterName];
                }
            }
            if (targetParameter == null)
            {
                targetParameter = dbCommand.CreateParameter();

                if (dbType.HasValue) { targetParameter.DbType = dbType.Value; }
                if (size.HasValue) { targetParameter.Size = size.Value; }

                dbCommand.Parameters.Add(targetParameter);
            }

            targetParameter.ParameterName = parameterName;
            targetParameter.Value = value;

            return this;
        }

        /// <summary>
        /// 替换脚本中的指定字符串
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public SqlBuilder SqlReplace(string oldValue, string newValue)
        {
            dbCommand.CommandText = dbCommand.CommandText.Replace(oldValue, newValue);

            return this;
        }
    }

    public enum OperationType
    {
        More,
        MoreAndEqual,
        Less,
        LessAndEqual,
        Equal,
        In,
        Between,
        LeftLike,
        RightLike,
        Like
    }

    /// <summary>
    /// 方法扩展
    /// </summary>
    public static class OperationTypeExtend
    {
        /// <summary>
        /// 对枚举变量方法扩展
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public static string GetTypeModel(this OperationType operationType)
        {
            switch (operationType)
            {
                case OperationType.More:
                    return "{0} > {1}";
                case OperationType.MoreAndEqual:
                    return "{0} >= {1}";
                case OperationType.Less:
                    return "{0} < {1}";
                case OperationType.LessAndEqual:
                    return "{0} <= {0}";
                case OperationType.In:
                    return "{0} In {1}";
                case OperationType.Between:
                    return "{0} between {1} and  {2}";
                case OperationType.Like:
                    return "{0} like {1}";
                case OperationType.LeftLike:
                    return "{0} like {1}";
                case OperationType.RightLike:
                    return "{0} like {1}";
                case OperationType.Equal:
                    return "{0} = {1}";
                default: return "";
            }
        }

        /// <summary>
        /// 条件连接类型方法扩展
        /// </summary>
        /// <param name="conditionConnectionType"></param>
        /// <returns></returns>
        public static string Tostr(this ConditionConnectionType conditionConnectionType)
        {
            switch (conditionConnectionType)
            {
                case ConditionConnectionType.And:
                    return "and";
                case ConditionConnectionType.Or:
                    return "or";
                default: return "";
            }
        }
    }

    /// <summary>
    /// 条件连接符
    /// </summary>
    public enum ConditionConnectionType
    {
        And,
        Or
    }

    public struct SqlCondition
    {
        public ConditionConnectionType ConditionConnectionType { get; set; }
        public string Sql { get; set; }
        public OperationType OperationType { get; set; }
        public string ParameterName { get; set; }
        public DbType DbType { get; set; }
        public int? Size { get; set; }
        public object Value { get; set; }
    }
}
