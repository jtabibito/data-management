<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginForm.aspx.cs" Inherits="DataManager.ServiceManager.LoginForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="NameLabel" Text="账号: " runat="server"></asp:Label>
            <asp:TextBox ID="NameTextBox" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="PwdLabel" Text="密码: " runat="server"></asp:Label>
            <asp:TextBox ID="PwdTextBox" runat="server"></asp:TextBox>
            <br />
            <asp:DropDownList ID="DropDownList" runat="server">
                <asp:ListItem Value="user">普通用户</asp:ListItem>
                <asp:ListItem Value="manager">管理员</asp:ListItem>
                <asp:ListItem Value="root_manager">root管理员</asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="SubmitButton" Text="确认" runat="server" OnClick="SubmitButton_OnClick"/>
        </div>
    </form>
</body>
</html>
