<%@ Page Title="Payroll Use" Language="C#" MasterPageFile="~/v1/Main.Master" AutoEventWireup="true" CodeBehind="Payroll.aspx.cs" Inherits="vms.v1.Payroll" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
         .nav-tabs .nav-link{
             color: black;
             background-color: transparent;
         }
        .nav-tabs .nav-link.active {
            background-color: #1A1A40;
            color: white;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 
    <div class="content-header">
        <div class="container-fluid">
            <div class="row mb-2">
                <div class="col-sm-6">
        

        <label for="ddlMonth">Select Month:</label>
        <asp:DropDownList ID="ddlMonth" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlMonth_SelectedIndexChanged" CssClass="form-control dropdown-small">
            <asp:ListItem Value="1">January</asp:ListItem>
            <asp:ListItem Value="2">February</asp:ListItem>
            <asp:ListItem Value="3">March</asp:ListItem>
            <asp:ListItem Value="4">April</asp:ListItem>
            <asp:ListItem Value="5">May</asp:ListItem>
            <asp:ListItem Value="6">June</asp:ListItem>
            <asp:ListItem Value="7">July</asp:ListItem>
            <asp:ListItem Value="8">August</asp:ListItem>
            <asp:ListItem Value="9">September</asp:ListItem>
            <asp:ListItem Value="10">October</asp:ListItem>
            <asp:ListItem Value="11">November</asp:ListItem>
            <asp:ListItem Value="12">December</asp:ListItem>
        </asp:DropDownList>
   

                </div>
                <div class="col-sm-6">
                    <ol class="breadcrumb float-sm-right">
                        <li class="breadcrumb-item"><a href="Dashboard.aspx">Home</a></li>
                        <li class="breadcrumb-item active">Staff Report</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>

            <div class="card">
        <div class="card-header">
            <ul class="nav nav-tabs card-header-tabs" id="payrollTabs" role="tablist">
                <li class="nav-item">
                    <a class="nav-link active" id="late-tab" data-toggle="tab" href="#late" role="tab" aria-controls="late" aria-selected="true">Late Staff</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" id="personal-tab" data-toggle="tab" href="#personal" role="tab" aria-controls="personal" aria-selected="false">Personal Reason Out</a>
                </li>
                  <li class="nav-item">
      <a class="nav-link" id="office-tab" data-toggle="tab" href="#office" role="tab" aria-controls="office" aria-selected="false">Office Business Out</a>
  </li>
            </ul>
        </div>

        <div class="card-body">
            <div class="tab-content" id="payrollTabContent">
                <div class="tab-pane fade show active" id="late" role="tabpanel" aria-labelledby="late-tab">
                  <asp:Button ID="btnDownloadLate" runat="server" Text="Download CSV" OnClick="btnDownloadLate_Click" CssClass="btn btn-success btn-sm mb-2" />


                    <asp:GridView ID="gvLateStaff" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover">
                        <Columns>
                            <asp:BoundField DataField="EMP_NAME" HeaderText="Name" />
                            <asp:BoundField DataField="EMP_NO" HeaderText="Emp No." />
                            <asp:BoundField DataField="REASON_LATE" HeaderText="Reason" />
                            <asp:BoundField DataField="TIME_IN" HeaderText="Time In" />
                        </Columns>
                    </asp:GridView>
                </div>

             
                <div class="tab-pane fade" id="personal" role="tabpanel" aria-labelledby="personal-tab">
                    <asp:Button ID="btnDownloadPersonal" runat="server" Text="Download CSV" OnClick="btnDownloadPersonal_Click" CssClass="btn btn-success btn-sm mb-2" />
                    <asp:GridView ID="gvStaffLeave" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover">
                       



                        <Columns>
                            <asp:BoundField DataField="STAFF_NAME" HeaderText="Name" />
                            <asp:BoundField DataField="DEPARTMENT" HeaderText="Department" />
                            <asp:BoundField DataField="TIME_OUT" HeaderText=" Time" />
                           <asp:TemplateField HeaderText="Date">
    <ItemTemplate>
        <%# Eval("DATE_OUT", "{0:dd/MM/yyyy}") %>
    </ItemTemplate>
</asp:TemplateField>

                            <asp:BoundField DataField="REASONS" HeaderText="Reason" />
                            <asp:BoundField DataField="LEAVE_FOR" HeaderText="Out For (Minute)" />
                            <asp:BoundField DataField="REPLACEMENT" HeaderText="Replacement" />
                           <asp:BoundField DataField="RETURN_STATUS" HeaderText="Return" />
                        </Columns>
                    </asp:GridView>
                </div>
                <div class="tab-pane fade" id="office" role="tabpanel" aria-labelledby="office-tab">
                    <asp:Button ID="btnDownloadOffice" runat="server" Text="Download CSV" OnClick="btnDownloadOffice_Click" CssClass="btn btn-success btn-sm mb-2" />

    <asp:GridView ID="gvOffice" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover">
        
        <Columns>
            <asp:BoundField DataField="STAFF_NAME" HeaderText="Name" />
            <asp:BoundField DataField="DEPARTMENT" HeaderText="Department" />
            <asp:BoundField DataField="TIME_OUT" HeaderText=" Time" />
       <asp:TemplateField HeaderText="Date">
    <ItemTemplate>
        <%# Eval("DATE_OUT", "{0:dd/MM/yyyy}") %>
    </ItemTemplate>
</asp:TemplateField>

            <asp:BoundField DataField="REASONS" HeaderText="Reason" />
            <asp:BoundField DataField="PLATE_NO" HeaderText="Plate No" />
            <asp:BoundField DataField="RETURN_STATUS" HeaderText="Return" />

        </Columns>
    </asp:GridView>
</div>
            </div>
        </div>
    </div>

    <style>
.nav-tabs .nav-link {
    font-size: 13px !important;
}

        .table-bordered{
            font-size: 13px !important;
        }
                    .dropdown-small {
    font-size: 13px !important;
}
    </style>

      
</asp:Content>
