using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.IO;
using System.Linq;

namespace TMS_MobileSampleModelGeneration
{
    /// <summary>
    /// 自动生成实体类
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                   .AddJsonFile("appsettings.json")
                                                   .Build();
            var conn = config.GetSection("Connections:DefaultConnect").Value;
            string path = string.Empty;
            var relativePath = config.GetSection("Settings:RelativePath").Value;
            //自动找最外层并 找到更外层 方便附加到其他项目中
            if (!string.IsNullOrEmpty(relativePath))
            {
                var basePath = new DirectoryInfo(Directory.GetCurrentDirectory());
                while ((basePath.FullName.Contains(@"\Debug") || basePath.FullName.Contains(@"\bin"))&&!string.IsNullOrEmpty(basePath.FullName))
                {
                    basePath=basePath.Parent;
                }
                path = Path.Combine(basePath.Parent.FullName, relativePath);
            }
            var fullPath= config.GetSection("Settings:FullPath").Value;
            if (!string.IsNullOrEmpty(fullPath))
                path = fullPath;
            InitModel(conn,config.GetSection("Settings:NameSpace").Value, path, config.GetSection("Settings:GenerateTables").Value);
        }
        public static void InitModel(string conn,string namespaceStr, string path,string genaratetables)
        {
            try
            {
                Console.WriteLine("开始创建");
                var tableNames = genaratetables.Split(',').ToList();
                for (int i = 0; i < tableNames.Count; i++)
                {
                    tableNames[i] = tableNames[i].ToLower();
                }
                var suger = GetInstance(conn).DbFirst.SettingClassTemplate(old =>
                {
                    return old.Replace("{Namespace}", namespaceStr);//.Replace("class {ClassName}", "class {ClassName} :BaseEntity");//改变命名空间
                });
                if (tableNames.Count >= 0)
                {
                    suger.Where(it => tableNames.Contains(it.ToLower())).IsCreateDefaultValue();
                }
                else
                {
                    suger.IsCreateDefaultValue();
                }
                //过滤BaseEntity中存在的字段
                //var pros = typeof(BaseEntity).GetProperties();
                //var list = new List<SqlSugar.IgnoreColumn>();
                var tables = suger.ToClassStringList().Keys;
                //foreach (var item in pros)
                //{
                //    foreach (var table in tables)
                //    {
                //        list.Add(new SqlSugar.IgnoreColumn() { EntityName = table, PropertyName = item.Name });
                //    }
                //}
                //suger.Context.IgnoreColumns.AddRange(list);
                suger.CreateClassFile(path);
                Console.WriteLine("创建完成");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
        public static SqlSugarClient GetInstance(string conn)
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = conn,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                IsShardSameThread = true //设为true相同线程是同一个SqlSugarClient
            });
            db.Ado.IsEnableLogEvent = true;
            db.Ado.LogEventStarting = (sql, pars) =>
            {

            };
            return db;
        }
    }
}
