<%@ Page Title="Request Status" Language="C#" MasterPageFile="~/v1/Main.Master" AutoEventWireup="true" CodeBehind="RequestStatus.aspx.cs" Inherits="vms.v1.RequestStatus" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


     <!-- Page Header -->
 <div class="content-header">
     <div class="container-fluid">
         <div class="row mb-2">
             <div class="col-sm-6">
                 <h6 class="font-weight-bold mb-0 mr-3">Request Status </h6>
             </div>
             <div class="col-sm-6">
                 <ol class="breadcrumb float-sm-right">
                     <li class="breadcrumb-item"><a href="Dashboard.aspx">Home</a></li>
                     <li class="breadcrumb-item active">Request Status</li>
                 </ol>
             </div>
         </div>
     </div>
 </div>
<section class="content">
    <div class="container-fluid custom-container">
        

            <div class="form-box">
                <!-- User Info -->
                <div class="row mb-2">
                    <div class="col-md-6">
                        <label class="form-label">Name:</label>
                        <span class="form-control-static"><asp:Label ID="txtName" runat="server" /></span>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Department:</label>
                        <span class="form-control-static"><asp:Label ID="txtDepartment" runat="server" /></span>
                    </div>
                </div>

                <div class="row mb-2">
                    <div class="col-md-6">
                        <label class="form-label">Employee No:</label>
                        <span class="form-control-static"><asp:Label ID="txtEmpNo" runat="server" /></span>
                    </div>
                       <div class="col-md-6">
        <label class="form-label">Working Hours:</label>
        <span class="form-control-static"><asp:Label ID="lblWorkingTime" runat="server" /></span>
    </div>
</div>
                </div>

                <!-- Date Filter -->
                <div class="row mb-3">
                    <div class="col-md-4">
                        <label class="form-label">Start Date:</label>
                        <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control date-small" TextMode="Date" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">End Date:</label>
                        <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control date-small" TextMode="Date" />
                    </div>
                    <div class="col-md-4 d-flex align-items-end">
                    <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn btn-filter w-100" OnClick="btnFilter_Click" />
                    </div>
                </div>
                

                <!-- Tabs -->
               <ul class="nav nav-pills mb-2" id="requestTabs" role="tablist">
    <li class="nav-item" role="presentation">
        <a class="nav-link active" id="pending-tab" data-toggle="pill" href="#pending" role="tab" aria-controls="pending" aria-selected="true">Pending</a>
    </li>
    <li class="nav-item" role="presentation">
        <a class="nav-link" id="approved-tab" data-toggle="pill" href="#approved" role="tab" aria-controls="approved" aria-selected="false">Approved</a>
    </li>
    <li class="nav-item" role="presentation">
        <a class="nav-link" id="rejected-tab" data-toggle="pill" href="#rejected" role="tab" aria-controls="rejected" aria-selected="false">Rejected</a>
    </li>
</ul>

                <div class="tab-content" id="requestTabContent">
                    <div class="tab-pane fade show active" id="pending" role="tabpanel">
                       
                        <asp:GridView ID="gvPending" runat="server" AutoGenerateColumns="False"
    CssClass="table table-sm table-bordered custom-gridview"
    OnRowCommand="gvPending_RowCommand" OnRowDataBound="gvPending_RowDataBound" >
    <Columns>
        <asp:BoundField DataField="DATE_OUT" HeaderText="Date Request" DataFormatString="{0:yyyy-MM-dd}" />
        <asp:BoundField DataField="TIME_OUT" HeaderText="Time Out" />
        <asp:BoundField DataField="TYPE" HeaderText="Type of Exit" />
        <asp:TemplateField HeaderText="Request By">
            <ItemTemplate>
                <asp:Literal ID="litRequestBy" runat="server"></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
       <asp:TemplateField HeaderText="Action">
    <ItemTemplate>
        <asp:Button ID="btnView" runat="server" Text="View"
            CommandName="ViewDetails1"
            CommandArgument='<%# Container.DataItemIndex %>' CssClass="btn btn-sm btn-primary" />
        &nbsp;
        <asp:Button ID="btnWithdraw" runat="server" Text="Withdraw"
            CommandName="WithdrawRequest"
            CommandArgument='<%# Container.DataItemIndex %>' CssClass="btn btn-sm btn-danger" OnClientClick="return confirm('Are you sure you want to withdraw this request?');" />
    </ItemTemplate>
</asp:TemplateField>
    </Columns>
</asp:GridView>
             </div>
                    
                                        <div class="tab-pane fade" id="approved" role="tabpanel">
                       
                            <asp:GridView ID="gvApproved" runat="server" AutoGenerateColumns="False" CssClass="table table-sm table-bordered custom-gridview" OnRowCommand="gvApproved_RowCommand" OnRowDataBound="gvApproved_RowDataBound">
    <Columns>
        <asp:BoundField DataField="DATE_OUT" HeaderText="Date Request" DataFormatString="{0:yyyy-MM-dd}" />
        <asp:BoundField DataField="TIME_OUT" HeaderText="Time Out" />
        <asp:BoundField DataField="TYPE" HeaderText="Type of Exit" />

    <asp:TemplateField HeaderText="Request By">
    <ItemTemplate>
        <asp:Literal ID="litRequestBy" runat="server"></asp:Literal>
    </ItemTemplate>
</asp:TemplateField>
        <asp:TemplateField HeaderText="Action">
            <ItemTemplate>
                <asp:Button ID="btnView" runat="server" Text="View"
                    CommandName="ViewDetails2"
                    CommandArgument='<%# Container.DataItemIndex %>' />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

                        </div>
                
                

                    <div class="tab-pane fade" id="rejected" role="tabpanel">  
                        <asp:GridView ID="gvRejected" runat="server" AutoGenerateColumns="False" CssClass="table table-sm table-bordered custom-gridview" OnRowCommand="gvRejected_RowCommand" OnRowDataBound="gvRejected_RowDataBound"
>
    <Columns>
        <asp:BoundField DataField="DATE_OUT" HeaderText="Date Request" DataFormatString="{0:yyyy-MM-dd}" />
       <asp:BoundField DataField="TIME_OUT" HeaderText="Time Out" />
<asp:BoundField DataField="TYPE" HeaderText="Type of Exit" />

     <asp:TemplateField HeaderText="Request By">
    <ItemTemplate>
        <asp:Literal ID="litRequestBy" runat="server"></asp:Literal>
    </ItemTemplate>
</asp:TemplateField>


        <asp:TemplateField HeaderText="Action">
            <ItemTemplate>
                <asp:Button ID="btnView" runat="server" Text="View"
                    CommandName="ViewDetails"
                    CommandArgument='<%# Container.DataItemIndex %>' />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

                        </div>
                
                </div>
            </div>

            <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="text-danger mt-3" />

      
    </div>




    <!-- View Request Modal -->
<div class="modal fade" id="viewModal" tabindex="-1" role="dialog" aria-labelledby="viewModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg" role="document">
        <div class="modal-content">
            <div class="modal-header text-white" style="background-color: #1A1A40;">

                <h6 class="modal-title" id="viewModalLabel">Request Details</h6>
                <button type="button" class="close text-white" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>

            <div class="modal-body">
                <table class="table table-bordered table-striped">
                    <tr>
                        <th>Name</th>
                        <td><asp:Label ID="lblName" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Department</th>
                        <td><asp:Label ID="lblDepartment" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Date Out</th>
                        <td><asp:Label ID="lblDateOut" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Time Out</th>
                        <td><asp:Label ID="lblTimeOut" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Type</th>
                        <td><asp:Label ID="lblType" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Reason</th>
                        <td><asp:Label ID="lblReason" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Request To (HOD)</th>
                        <td><asp:Label ID="lblRequestTo" runat="server" /></td>
                    </tr>
                    <tr>
    <th>Rejection Reason </th>
    <td><asp:Label ID="lblRejectionReason" runat="server" /></td>
</tr>

                </table>
            </div>

            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>

      <!-- View Request Modal -->
<div class="modal fade" id="viewModal1" tabindex="-1" role="dialog" aria-labelledby="viewModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg" role="document">
        <div class="modal-content">
            <div class="modal-header text-white" style="background-color: #1A1A40;">

                <h6 class="modal-title" id="viewModalLabel1">Request Details</h6>
                <button type="button" class="close text-white" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>

            <div class="modal-body">
                <table class="table table-bordered table-striped">
                    <tr>
                        <th>Name</th>
                        <td><asp:Label ID="lblName1" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Department</th>
                        <td><asp:Label ID="lblDepartment1" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Date Out</th>
                        <td><asp:Label ID="lblDateOut1" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Time Out</th>
                        <td><asp:Label ID="lblTimeOut1" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Type</th>
                        <td><asp:Label ID="lblType1" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Reason</th>
                        <td><asp:Label ID="lblReason1" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Request To (HOD)</th>
                        <td><asp:Label ID="lblRequestTo1" runat="server" /></td>
                    </tr>
                    <tr>
   

                </table>
            </div>

            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>
          <!-- View Request Modal -->
<div class="modal fade" id="viewModal2" tabindex="-1" role="dialog" aria-labelledby="viewModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg" role="document">
        <div class="modal-content">
            <div class="modal-header text-white" style="background-color: #1A1A40;">

                <h6 class="modal-title" id="viewModalLabel2">Request Details</h6>
                <button type="button" class="close text-white" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>

            <div class="modal-body">
                <table class="table table-bordered table-striped">
                    <tr>
                        <th>Name</th>
                        <td><asp:Label ID="lblName2" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Department</th>
                        <td><asp:Label ID="lblDepartment2" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Date Out</th>
                        <td><asp:Label ID="lblDateOut2" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Time Out</th>
                        <td><asp:Label ID="lblTimeOut2" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Type</th>
                        <td><asp:Label ID="lblType2" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Reason</th>
                        <td><asp:Label ID="lblReason2" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Request To (HOD)</th>
                        <td><asp:Label ID="lblRequestTo2" runat="server" /></td>
                    </tr>
                    <tr>
       <th>Approve on</th>
    <td><asp:Label ID="lblApproveDate" runat="server" /></td>
</tr>

                </table>
            </div>

            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>



</section>
   
    <!-- Bootstrap CSS -->
  <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet">

  <!-- jQuery Full Version (not slim) -->
  <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>

  <!-- Bootstrap JS -->
  <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
  <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>

 
    <style>

        .date-small{
            font-size: 14px;
        }
    /* Your existing styles */
    .custom-gridview {
    font-size: 0.85rem; /* smaller font */
    border-color: #ccc; /* border color */
}

.custom-gridview th {
    background-color: #b2b2b2;
    color: #000;
    font-weight: 600;
    text-align: left;
}

.custom-gridview td {
    vertical-align: middle;
}

.custom-gridview tr:hover {
    background-color: #f0f0ff; /* subtle hover effect */
}

.custom-gridview .btn {
    font-size: 0.8rem;
    padding: 2px 8px;
    background-color: #e4e4f7;
    border-color: #d0d0e7;
    color: #000;
}

.custom-gridview .btn:hover {
    background-color: #cfcff5;
    color: #000;
}

    .form-box {
        background-color: #f9f9f9;
        padding: 15px;
        border-radius: 8px;
        border: 1px solid #ccc;
        font-size: 0.85rem;
    }

   

    /* Fix for nav-pills active tab */
    .nav-pills .nav-link.active {
        background-color:#1A1A40 !important;
        color: #ffffff !important;
    }

    /* Custom button color matching active tab */
    .btn-filter {
        background-color: #1A1A40 !important;
        border-color: #d0d0e7 !important;
        color: #ffffff !important;
        font-size: 0.85rem;
    }

    .btn-filter:hover, .btn-filter:focus {
        background-color: #cfcff5 !important;
        color: #000 !important;
    }
    
        .content {
    background-color: #e1eaf4; /* Replace with your desired color */
     height: 700px; 
}
</style>

</asp:Content>
