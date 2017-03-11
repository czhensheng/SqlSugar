﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.Linq.Expressions;
using OrmTest.Models;
namespace OrmTest.UnitTest
{
    public class SelectQuery : ExpTestBase
    {
        private SelectQuery() { }
        public SelectQuery(int eachCount)
        {
            this.Count = eachCount;
        }
        internal void Init()
        {
            base.Begin();
            for (int i = 0; i < base.Count; i++)
            {
                Q2();
            }
            base.End("Method Test");
        }

        public void Q2()
        {
            using (var db = GetInstance())
            {
                db.Database.IsEnableLogEvent = true;
                db.Database.LogEventStarting = (sql,pars) =>
                {
                    Console.WriteLine(sql+" " + pars);
                };
                var list = db.Queryable<Student>()
                    .Where(st => st.Id > 0)
                    .Select(it => new ViewModelStudent { Name = it.Name }).ToList();

                var list2 = db.Queryable<Student>()
                  .Where(st => st.Id > 0)
                 .Select("id").ToList();

                var list3 = db.Queryable<Student, School,School>((st, sc,sc2) => new object[] {
                          JoinType.Left,st.SchoolId==sc.Id,
                          JoinType.Left,sc2.Id==sc.Id
                }).Where(st => st.Id > 0)
                .Select<School>((st) =>new School() { Id=st.Id}).ToList();

                var list4 = db.Queryable("Student", "st")
                 .AddJoinInfo("School", "sh", "sh.id=st.schoolid")
                 .Where("st.id>@id")
                 .AddParameters(new {id=1})
                 .Select("st.*").ToList();
            }
        }

        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new SystemTablesConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer });
            return db;
        }
    }
}