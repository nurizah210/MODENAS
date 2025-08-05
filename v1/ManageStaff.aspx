<%@ Page Title="Manage Staff" Language="C#" MasterPageFile="~/v1/Main.Master" AutoEventWireup="true" CodeBehind="ManageStaff.aspx.cs" Inherits="vms.v1.ManageStaff" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
     .btn-sm {
    font-size: 0.875rem;
    padding: 0.475rem 1.45rem;
    background-color: #1A1A40;
    border-color: #1A1A40;
    color: white;
}

.btn-sm:hover {
    background-color: #8989ae;
    border-color: #8989ae;
    color: black;
}

.form-section-title {
    font-size: 1.2rem;
    font-weight: 600;
    margin-bottom: 1rem;
    color: #1A1A40;
}

.form-control {
    font-size: 0.8rem;
    padding: 0.4rem 0.6rem;
}

::placeholder {
    font-size: 0.75rem;
    color: #999;
}

.guard-table td, .guard-table th {
    font-size: 0.8rem;
    padding: 0.35rem 0.5rem;
    vertical-align: middle;
}

.delete-btn {
    background-color: #dc3545;
    color: white;
    border: none;
    padding: 4px 6px;
    border-radius: 4px;
    font-size: 0.75rem;
}

.delete-btn i {
    pointer-events: none;
}

    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet" />
<!-- STAFF MANAGEMENT -->
<div class="card mb-4 shadow-sm">
    <div class="card-body">
        <div class="form-section-title">Staff Info</div>

        <div class="row g-3 mb-3">
            <div class="col-md-4">
                <asp:TextBox ID="txtEmpNo" runat="server" CssClass="form-control" placeholder="Enter Employee No"></asp:TextBox>
            </div>
            <div class="col-md-4">
                <button id="btnSearchStaff" runat="server" onserverclick="btnSearchStaff_Click" class="btn btn-sm" title="Search">
                    <i class="fas fa-search"></i>
                </button>
            </div>
        </div>

        <div class="row g-3 mb-3">
            <div class="col-md-4">
                <asp:TextBox ID="txtEmpName" runat="server" CssClass="form-control" placeholder="Employee Name"></asp:TextBox>
            </div>
            <div class="col-md-2">
                <asp:TextBox ID="txtDept" runat="server" CssClass="form-control" placeholder="Dept"></asp:TextBox>
            </div>
            <div class="col-md-2">
                <asp:TextBox ID="txtSect" runat="server" CssClass="form-control" placeholder="Sect"></asp:TextBox>
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtPosition" runat="server" CssClass="form-control" placeholder="Position"></asp:TextBox>
            </div>
        </div>

        <!-- Working Time & HOD Email Side by Side -->
        <div class="row g-3 mb-3">
            <div class="col-md-4">
                <asp:TextBox ID="txtWorkingTime" runat="server" CssClass="form-control" placeholder="Working Time"></asp:TextBox>
            </div>
            <div class="col-md-4">
                <asp:TextBox ID="txtHodEmail" runat="server" CssClass="form-control" placeholder="Email (For HOD)"></asp:TextBox>
            </div>
                    <div class="col-md-2">
    <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control">
        <asp:ListItem Text="- Select Role -" Value="" />
        <asp:ListItem Text="USER" Value="USER" />
        <asp:ListItem Text="ADMIN" Value="ADMIN" />
        <asp:ListItem Text="SUPER ADMIN" Value="SUPER ADMIN" />
    </asp:DropDownList>
</div>
        </div>
           


        <div class="mt-3">
            <asp:Button ID="btnAddOrUpdateStaff" runat="server" CssClass="btn btn-sm me-2" Text="Add"
                OnClick="btnAddOrUpdateStaff_Click"
                OnClientClick="return confirm('Are you sure you want to add or update this staff?');" />

            <asp:Button ID="btnUpdateStaff" runat="server" CssClass="btn btn-sm me-2" Text="Update"
                OnClick="btnUpdateStaff_Click"
                OnClientClick="return confirm('Are you sure you want to update this staff?');" />

            <asp:Button ID="btnDeleteStaff" runat="server" CssClass="btn btn-sm me-2" Text="Delete"
                OnClick="btnDeleteStaff_Click"
                OnClientClick="return confirm('Are you sure you want to delete this staff?');" />

            <asp:Button ID="btnClearStaff" runat="server" CssClass="btn btn-sm" Text="Clear"
                OnClick="btnClearStaff_Click" />
        </div>
    </div>
</div>
  

<!-- FRIDAY BREAK SETTINGS -->
<div class="card mb-4 shadow-sm">
    <div class="card-body">
        <div class="form-section-title">Friday Break Settings</div>

        <div class="row g-3 mb-3">
            <div class="col-md-4">
                <label class="form-label">Break Time </label>
                <asp:TextBox ID="txtFridayBreakTime" runat="server" CssClass="form-control" placeholder=" eg : 1315-1445"></asp:TextBox>
            </div>
        </div>

        <asp:Button ID="btnSaveFridayBreak" runat="server" Text="Save Friday Break Time"
            CssClass="btn btn-sm"
            OnClick="btnSaveFridayBreak_Click"
            OnClientClick="return confirm('Are you sure you want to save this Friday break time?');" />
    </div>
</div>

    <!-- GUARD MANAGEMENT -->
    <div class="card shadow-sm">
        <div class="card-body">

            <div class="row g-2 align-items-end mb-3">
                <div class="col-md-4">
                     
                    <asp:TextBox ID="txtGuardName" runat="server" CssClass="form-control" placeholder="Guard Name"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    
                    <asp:TextBox ID="txtGuardPhone" runat="server" CssClass="form-control"  placeholder="Phone Number"></asp:TextBox>
                </div>
                <div class="col-md-4">
                  <asp:Button ID="btnAddGuard" runat="server" CssClass="btn btn-sm" 
    Text="Add Guard" 
    OnClick="btnAddGuard_Click"
    OnClientClick="return confirm('Are you sure you want to add this guard?');" />

                </div>
            </div>

            <hr class="my-4" />
<asp:GridView ID="gvGuards" runat="server"
    CssClass="table table-bordered table-sm guard-table"
    AutoGenerateColumns="False"
    DataKeyNames="NAME"
    OnRowDeleting="gvGuards_RowDeleting">
    <Columns>
        <asp:BoundField DataField="NAME" HeaderText="Guard Name" />
        <asp:BoundField DataField="NO_TEL" HeaderText="Phone No" />

        <asp:TemplateField >
            <ItemTemplate>
                <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete"
                    OnClientClick="return confirm('Are you sure you want to delete this guard?');"
                    CssClass="delete-btn" ToolTip="Delete">
                    <i class="fas fa-trash-alt"></i>
                </asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

        </div>
    </div>
</asp:Content>
