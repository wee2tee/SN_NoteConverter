using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SN_NoteConverter.Model
{
    public class DBX
    {
        public static sn_netEntities DataSet(string servername, string dbname, string uid, string pwd)
        {
            return new sn_netEntities("metadata=res://*/Model.SN_NetModel.csdl|res://*/Model.SN_NetModel.ssdl|res://*/Model.SN_NetModel.msl;provider=MySql.Data.MySqlClient;provider connection string=\"Data Source=" + servername + ";Port=3306;user id=" + uid + ";password=" + pwd + ";Persist Security Info=True;Initial Catalog=" + dbname + ";charset=utf8\"");
        }
    }
}
