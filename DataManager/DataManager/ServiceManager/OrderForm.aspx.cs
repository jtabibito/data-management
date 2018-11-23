using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services.Configuration;
using System.Data;
using System.Web.Configuration;
using MySql.Data.MySqlClient;

namespace DataManager.ServiceManager
{
    public partial class OrderForm : System.Web.UI.Page
    {
        private string conn_str = WebConfigurationManager.ConnectionStrings["yiju_databaseConnectionString"].ToString();
        private MySqlConnection conn;

        protected void Page_Load(object sender, EventArgs e) {
            if (!IsPostBack) {
                FormViewBindData();
                ((Button)(this.GridView1.FooterRow.Cells[0].Controls[3])).Visible = false;
            }

            this.GridView1.RowEditing += new GridViewEditEventHandler(GridView1_RowEditing);
            this.GridView1.RowDeleting += new GridViewDeleteEventHandler(GridView1_RowDeleting);
            this.GridView1.RowUpdating += new GridViewUpdateEventHandler(GridView1_RowUpdating);
            this.GridView1.RowCancelingEdit += new GridViewCancelEditEventHandler(GridView1_RowCancelingEdit);

            RegisterTBControls();
        }
        
        // 注册文本框控件,用户添加新数据时向注册
        // 的文本框中填入数据
        private void RegisterTBControls() {
            for (int i = 1; i < this.GridView1.HeaderRow.Cells.Count; i++) {
                TableCell tabc = new TableCell();
                TextBox tb = new TextBox();
                tb.Attributes.Add("runat", "server");
                tabc.Controls.Add(tb);
                this.GridView1.FooterRow.Cells.RemoveAt(i);
                this.GridView1.FooterRow.Cells.AddAt(i, tabc);
            }
        }

        /// <summary>
        /// 创建数据库操作字符串
        /// </summary>
        /// <param name="udata">自定义数据</param>
        private void CreateCmd(ref MySqlCmd.MySqlCmd.MySqlContext udata) {
            string table_name = "ORDER_FORM";
            switch (udata.status) {
                case MySqlCmd.MySqlRequest.INSERT: {
                    string add_str = "INSERT INTO `" + table_name + "` (`";
                    for (int i = 1; i < this.GridView1.HeaderRow.Cells.Count; i++) {
                        if (i < this.GridView1.Rows[0].Cells.Count - 1) {
                            add_str += this.GridView1.HeaderRow.Cells[i].Text.Trim() + "`,`";
                        }
                        else {
                            add_str += this.GridView1.HeaderRow.Cells[i].Text.Trim() + "`) VALUES (\"";
                        }
                    }

                    for (int i = 1; i < this.GridView1.HeaderRow.Cells.Count; i++) {
                        if (i < this.GridView1.Rows[0].Cells.Count - 1) {
                            add_str += ((TextBox)(this.GridView1.FooterRow.Cells[i].Controls[0])).Text.Trim() + "\",\"";
                        }
                        else {
                            add_str += ((TextBox)(this.GridView1.FooterRow.Cells[i].Controls[0])).Text.Trim() + "\");";
                        }
                    }
                    udata.context = add_str;
                    break;
                }
                case MySqlCmd.MySqlRequest.DELETE: {
                    string delete_str = "DELETE FROM `" + table_name + "` WHERE `" + this.GridView1.HeaderRow.Cells[1].Text.Trim()
                                + "`=\"" + this.GridView1.Rows[udata.res].Cells[1].Text.Trim() + "\";";
                    udata.context = delete_str;
                    break;
                }
                case MySqlCmd.MySqlRequest.UPDATE: {
                    string update_str = "UPDATE `" + table_name + "` SET `";
                    for (int i = 1; i < this.GridView1.Rows[0].Cells.Count; i++) {
                        if (i < this.GridView1.Rows[0].Cells.Count - 1) {
                            update_str += this.GridView1.HeaderRow.Cells[i].Text.Trim() + "`=\""
                                + ((TextBox)(this.GridView1.Rows[this.GridView1.EditIndex].Cells[i].Controls[0])).Text + "\",`";
                        }
                        else {
                            update_str += this.GridView1.HeaderRow.Cells[i].Text.Trim() + "`=\""
                                + ((TextBox)(this.GridView1.Rows[this.GridView1.EditIndex].Cells[i].Controls[0])).Text + "\"";
                        }
                    }
                    update_str += " WHERE `" + this.GridView1.HeaderRow.Cells[1].Text.Trim() + "`=\""
                        + ((TextBox)(this.GridView1.Rows[this.GridView1.EditIndex].Cells[1].Controls[0])).Text + "\";";
                    udata.context = update_str;
                    break;
                }
            }
        }

        private void AfterSetAdapter(MySqlDataAdapter adapter) {
            DataSet ds = new DataSet();
            adapter.Fill(ds, "order_form");

            this.GridView1.DataSource = ds.Tables["order_form"].DefaultView;
            this.GridView1.DataBind();
        }

        // 更新前台显示数据
        private void FormViewBindData() {
            conn = MySqlCmd.MySqlCmd.Connection(conn_str);
            string selection_str = "SELECT * FROM ORDER_FORM";
            MySqlCmd.MySqlCmd.SetMySqlDataAdapter(selection_str, conn, AfterSetAdapter);
            ((Button)(this.GridView1.FooterRow.Cells[0].Controls[3])).Visible = false;
        }

        // 对当前行显示编辑状态
        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e) {
            this.GridView1.EditIndex = e.NewEditIndex;
            FormViewBindData();
        }

        // 删除当前行
        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e) {
            conn = MySqlCmd.MySqlCmd.Connection(conn_str);
            MySqlCmd.MySqlCmd.MySqlContext udata = new MySqlCmd.MySqlCmd.MySqlContext();
            udata.conn = conn;
            udata.status = MySqlCmd.MySqlRequest.DELETE;
            udata.create_cmd = CreateCmd;
            udata.callback = AfterExecuteCmd;
            udata.res = e.RowIndex;
            MySqlCmd.MySqlCmd.SetMySqlCommand(ref udata);

            FormViewBindData();
        }
        
        private void AfterExecuteCmd(MySqlCmd.MySqlCmd.MySqlContext udata) {
            Response.Write(udata.comm.CommandText + "</br>");
            Response.Write("受影响行数: " + udata.res + "</br>");
        }

        private void UpdateData(int row_index) {
            conn = MySqlCmd.MySqlCmd.Connection(conn_str);
            MySqlCmd.MySqlCmd.MySqlContext udata = new MySqlCmd.MySqlCmd.MySqlContext();
            udata.conn = conn;
            udata.status = MySqlCmd.MySqlRequest.UPDATE;            
            udata.res = -1;
            udata.create_cmd = CreateCmd;
            udata.callback = AfterExecuteCmd;
            MySqlCmd.MySqlCmd.SetMySqlCommand(ref udata);
        }

        // 更新数据
        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e) {
            UpdateData(this.GridView1.EditIndex);

            this.GridView1.EditIndex = -1;
            FormViewBindData();
        }

        // 取消当前行编辑
        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e) {
            this.GridView1.EditIndex = -1;
            FormViewBindData();
        }
        
        protected void CheckBox1_CheckedChanged(object sender, EventArgs e) {
            if (((CheckBox)(this.GridView1.FooterRow.Cells[0].Controls[1])).Checked) {
                ((Button)(this.GridView1.FooterRow.Cells[0].Controls[3])).Visible = true;
            }
            if (((CheckBox)(this.GridView1.FooterRow.Cells[0].Controls[1])).Checked == false) {
                ((Button)(this.GridView1.FooterRow.Cells[0].Controls[3])).Visible = false;
            }
        }

        private void AddUserData() {
            conn = MySqlCmd.MySqlCmd.Connection(conn_str);
            MySqlCmd.MySqlCmd.MySqlContext udata = new MySqlCmd.MySqlCmd.MySqlContext();            
            udata.conn = conn;
            udata.status = MySqlCmd.MySqlRequest.INSERT;
            udata.create_cmd = CreateCmd;
            udata.callback = AfterExecuteCmd;
            udata.res = -1;
            MySqlCmd.MySqlCmd.SetMySqlCommand(ref udata);
        }

        protected void InsertButton_Click(object sender, EventArgs e) {
            AddUserData();
            FormViewBindData();
        }
    }
}