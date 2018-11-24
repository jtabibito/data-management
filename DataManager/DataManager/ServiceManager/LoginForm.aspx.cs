using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using MySql.Data.MySqlClient;

namespace DataManager.ServiceManager {
    public partial class LoginForm : System.Web.UI.Page {
        private string conn_str = WebConfigurationManager.ConnectionStrings["yiju_databaseConnectionString"].ToString();
        protected void Page_Load(object sender, EventArgs e) {
            if (!IsPostBack) {
                
            }
        }

        /// <summary>
        /// 读取数据库前的回调方法
        /// </summary>
        /// <param name="udata"></param>
        private void CreateSelectUserCmd(ref MySqlCmd.MySqlCmd.MySqlContext udata) {
            udata.context = "SELECT * FROM `" + this.DropDownList.SelectedValue + "` WHERE `账号`=?account and `密码`=?pwd LIMIT 1";
            udata.comm = new MySqlCommand(udata.context, udata.conn);
            udata.comm.Parameters.AddWithValue("account", this.NameTextBox.Text.Trim());
            udata.comm.Parameters.AddWithValue("pwd", this.PwdTextBox.Text.Trim());
        }

        /// <summary>
        /// 读取数据库后的回调方法
        /// </summary>
        /// <param name="udata"></param>
        private void AfterSelect(MySqlCmd.MySqlCmd.MySqlContext udata) {
            if (udata.res == 1) {
                Session["user_context"] = udata.context;

                HttpCookie cookie = new HttpCookie("user");
                cookie["log_act+?/"] = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(this.NameTextBox.Text.Trim(), "MD5").ToLower();
                cookie["log_pwd+?/"] = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(this.PwdTextBox.Text.Trim(), "MD5").ToLower();
                cookie.Expires.AddDays(7.0f);
                Response.AppendCookie(cookie);
                
                Response.Redirect("./OrderForm.aspx?compentence=+" + this.DropDownList.Text.Trim());
            }
            else {
                ClientScript.RegisterStartupScript(Page.GetType(), "NonData", "<script type=\"text/javascript\">alert(\"没有此账户!\")</script>");
                return;
            }
        }

        /// <summary>
        /// 登录按钮事件,使用MySqlContext结构保存数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SubmitButton_OnClick(object sender, EventArgs e) {
            if (this.NameTextBox.Text == "" || this.PwdTextBox.Text == "") {
                ClientScript.RegisterStartupScript(this.Page.GetType(), "InfoSubmit", "<script type=\"text/javascript\">alert(\"账号或密码不能为空!\");</script>");
                return;
            }
            MySqlConnection conn = MySqlCmd.MySqlCmd.Connection(conn_str);
            MySqlCmd.MySqlCmd.MySqlContext udata = new MySqlCmd.MySqlCmd.MySqlContext();
            udata.conn = conn;
            udata.create_cmd = CreateSelectUserCmd;
            udata.res = -1;
            udata.status = MySqlCmd.MySqlRequest.SEARCH;
            udata.callback = AfterSelect;

            MySqlCmd.MySqlCmd.LoginCommand(ref udata);
        }
    }
}