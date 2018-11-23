using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace DataManager.MySqlCmd {
    public enum MySqlRequest {
        INSERT = 0x0001,
        DELETE = 0x0010,
        UPDATE = 0x0100,
        SEARCH = 0x1000,
    }

    public class MySqlCmd {
        public delegate void AdapterCallBack(MySqlDataAdapter adapter);
        public delegate void CommandCallBack(MySqlContext udata);
        public delegate void CreateCommand(ref MySqlContext udata);

        public struct MySqlContext {
            public MySqlConnection conn;               // 用户请求的数据库连接对象
            public MySqlCommand comm;                  // 用户请求的command命令对象
            public MySqlRequest status;                         // 数据库请求的命令号
            public string context;                     // 用户请求的command命令描述
            public int res;                            // 数据库执行操作返回值
            public CreateCommand create_cmd;           // 用户自定义数据库命令创建方法
            public CommandCallBack callback;           // 用户自定义方法
        }

        /// <summary>
        /// 打开数据库返回请求连接的数据库对象
        /// </summary>
        /// <param name="conn">数据库连接字符串</param>
        /// <returns>数据库连接对象</returns>
        public static MySqlConnection Connection(string conn_str) {
            MySqlConnection conn_object = new MySqlConnection(conn_str);
            return conn_object;
        }

        /// <summary>
        /// 创建数据库适配器对象,执行用户自定义方法
        /// </summary>
        /// <param name="cmd_str">数据库命令字符串</param>
        /// <param name="conn">连接的数据库对象</param>
        /// <param name="callback">用户自定义方法</param>
        public static void SetMySqlDataAdapter(string cmd_str, MySqlConnection conn, AdapterCallBack callback) {
            lock (conn) {
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd_str, conn);
                callback(adapter);
            }
        }

        /// <summary>
        /// 创建数据库命令对象，执行用户自定义方法
        /// </summary>
        /// <param name="cmd_str">数据库命令字符串</param>
        /// <param name="conn">连接的数据库对象</param>
        /// <param name="callback">用户自定义方法</param>
        public static void SetMySqlCommand(ref MySqlContext udata) {
            lock (udata.conn) {
                udata.create_cmd(ref udata);
                udata.comm = new MySqlCommand(udata.context, udata.conn);
                udata.conn.Open();
                udata.res = udata.comm.ExecuteNonQuery();
                udata.conn.Close();
                udata.callback(udata);
            }
        }
    }
}