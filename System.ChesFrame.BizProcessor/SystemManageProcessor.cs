using System;
using System.ChesFrame.Entity.SystemManage;
using System.ChesFrame.IDataAccess;
using System.ChesFrame.Utility.ObjectFactory;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ChesFrame.BizProcessor
{
    public class SystemManageProcessor
    {
        #region 菜单

        private ISystemManageDal systemManageDal = ObjectFactory.Create<ISystemManageDal>();

        public List<MenuEntity> MenuGetAll()
        {
            //1.所有menu Item
            var menuEntityList = systemManageDal.MenuGetAll();

            //2.构建属性结构
            menuEntityList.Sort((a, b) =>string.Compare(a.RelationID, b.RelationID));
            List<MenuEntity> list = new List<MenuEntity>();
            var perLevelKeyLenth = 3;

            Func<List<MenuEntity>, string, MenuEntity> FindTargetEntity = null;
            FindTargetEntity = (entityList, relationID) =>
            {
                if (entityList != null && entityList.Count > 0)
                {
                    foreach (var l in entityList)
                    {
                        if (l.RelationID == relationID)
                        {
                            return l;
                        }
                        else
                        {
                            var g = FindTargetEntity(l.SubMenu, relationID);
                            if (g != null)
                            {
                                return g;
                            }
                            else { continue; }
                        }
                    }
                }
                return null;
            };

            foreach (var sub in menuEntityList) 
            {
                if (sub.RelationID.Length == perLevelKeyLenth) //第一级菜单
                {
                    list.Add(sub);
                    continue;
                }

                var single = FindTargetEntity(list,
                    sub.RelationID.Substring(0, sub.RelationID.Length - perLevelKeyLenth));//list.SingleOrDefault((a) => a.RelationID == sub.RelationID.Substring(0, sub.RelationID.Length - perLevelKeyLenth));
                if (single != null)
                {
                    if (single.SubMenu == null) { single.SubMenu = new List<MenuEntity>(); }
                    single.SubMenu.Add(sub);
                    continue;
                }
            }

            Func<int?,int> IntNullToMax = (o) =>o.HasValue?o.Value:int.MaxValue ;
            //3.对树形菜单结构更具优先级排序
            Action<List<MenuEntity>> CircleSort = null;
            CircleSort = (t) => {
                t.Sort((j, i) => IntNullToMax(j.Priority) - IntNullToMax(i.Priority));
                foreach (var m in t)
                {
                    if (m.SubMenu != null && m.SubMenu.Count > 0)
                    {
                        CircleSort(m.SubMenu);
                    }
                }
            };
            CircleSort(list);

            return list;
        }

        #endregion
    }
}
