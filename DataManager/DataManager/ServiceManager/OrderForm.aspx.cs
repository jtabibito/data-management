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
        protected void Page_Load(object sender, EventArgs e) {
            if (!IsPostBack) {
                FormViewBindData();
            }
            this.GridView1.RowEditing += new GridViewEditEventHandler(GridView1_RowEditing);
            this.GridView1.RowDeleting += new GridViewDeleteEventHandler(GridView1_RowDeleting);
            this.GridView1.RowUpdating += new GridViewUpdateEventHandler(GridView1_RowUpdating);
            this.GridView1.RowCancelingEdit += new GridViewCancelEditEventHandler(GridView1_RowCancelingEdit);
        }
        
        private void FormViewBindData() {
            string connStr = WebConfigurationManager.ConnectionStrings["yiju_databaseConnectionString"].ToString();
            MySqlConnection conn = new MySqlConnection(connStr);

            string selectiongStr = "SELECT * FROM ORDER_FORM";
            MySqlDataAdapter mysqlAdapter = new MySqlDataAdapter(selectiongStr, conn);

            DataSet ds = new DataSet();
            mysqlAdapter.Fill(ds, "order_form");

            this.GridView1.DataSource = ds.Tables["order_form"].DefaultView;
            this.GridView1.DataBind();
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e) {
            this.GridView1.EditIndex = e.NewEditIndex;
            FormViewBindData();
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e) {
            FormViewBindData();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e) {
            this.GridView1.EditIndex = -1;
            FormViewBindData();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e) {
            this.GridView1.EditIndex = -1;
            FormViewBindData();
        }
    }
}