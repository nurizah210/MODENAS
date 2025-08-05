<%@ Page Title="HOD Approval" Language="C#" MasterPageFile="~/v1/Main.Master" AutoEventWireup="true" CodeBehind="HODApproval.aspx.cs" Inherits="vms.v1.HODApproval" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Content Header -->
    <div class="content-header">
        <div class="container-fluid">
            <div class="row mb-2">
                <div class="col-sm-6">
                    <h6 class="font-weight-bold mb-0 mr-3">HOD Approval</h6>
                </div>
                <div class="col-sm-6">
                    <ol class="breadcrumb float-sm-right">
                        <li class="breadcrumb-item"><a href="Dashboard.aspx">Home</a></li>
                        <li class="breadcrumb-item active">HOD Approval</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>

    <section class="content">
        <div class="container-fluid custom-container">
            <div class="card">
                <div class="card-body">
                    <div class="form-group">
                        <label for="requestType">Filter by Request Type:</label>
                        <asp:DropDownList ID="ddlRequestType" runat="server" CssClass="form-control dropdown-small" AutoPostBack="true" OnSelectedIndexChanged="ddlRequestType_SelectedIndexChanged">
                            <asp:ListItem Text="-- Select Type --" Value="" />
                            <asp:ListItem Text="Office Business" Value="Office Matter" />
                            <asp:ListItem Text="Personal Reason" Value="Personal Reason" />
                        </asp:DropDownList>
                    </div>

                    <div class="table-responsive">
                        <asp:GridView ID="gvPendingRequests" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered" OnRowCommand="gvPendingRequests_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="STAFF_NAME" HeaderText="Staff Name" SortExpression="STAFF_NAME" />
                                <asp:BoundField DataField="DEPARTMENT" HeaderText="Department" SortExpression="DEPARTMENT" />
                                <asp:BoundField DataField="REASONS" HeaderText="Reason" SortExpression="REASONS" />
                                <asp:BoundField DataField="FORMATTED_DATE_OUT" HeaderText="Date Out" SortExpression="DATE_OUT" />
                                <asp:BoundField DataField="TIME_OUT" HeaderText="Time Out" SortExpression="TIME_OUT" />
                                <asp:BoundField DataField="PLATE_NO" HeaderText="Plate No" SortExpression="PLATE_NO" />
                                <asp:TemplateField>
                                    <ItemTemplate>
                                    <asp:Button ID="btnApprove" runat="server" Text="Approve" CommandName="Approve" CommandArgument='<%# Eval("STAFF_NAME") + "|" + Eval("TIME_OUT")  %>'
 OnClientClick="return confirm('Are you sure you want to approve this request?')" CssClass="btn btn-sm" style="background-color: #1A1A40; color: white;" />
                                    <asp:Button 
    ID="btnReject" 
    runat="server" 
    Text="Reject"
    CommandName="Reject" 
    CommandArgument='<%# Eval("STAFF_NAME") + "|" + Eval("TIME_OUT") %>' 
    OnClientClick='<%# "storeRejectArgument(\"" + Eval("STAFF_NAME") + "|" + Eval("TIME_OUT") + "\"); return false;" %>' 
    CssClass="btn btn-danger btn-sm" />

                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        </div>
                </div>
            </div>

            <div class="form-group text-center">
                <asp:Label ID="lblMessage" runat="server" CssClass="text-success" Visible="false"></asp:Label>
            </div>
        </div>


        <!-- Rejection Reason Modal -->
<div class="modal fade" id="rejectionModal" tabindex="-1" role="dialog" aria-labelledby="rejectionModalLabel" aria-hidden="true">
  <div class="modal-dialog" role="document">
    <asp:Panel runat="server" DefaultButton="btnSubmitRejectionReason">
        <div class="modal-content">
           <div class="modal-header text-white" style="background-color: #1A1A40;">
            <h6 class="modal-title" id="rejectionModalLabel">Rejection Reason</h6>
            <button type="button" class="close" data-dismiss="modal" aria-label="Close" style="color: white;">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div class="modal-body">
            <asp:HiddenField ID="hfRejectCommandArg" runat="server" />
            <asp:TextBox ID="txtRejectionReason" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="Enter reason "></asp:TextBox>
          </div>
          <div class="modal-footer">
            <asp:Button ID="btnSubmitRejectionReason" runat="server" Text="Submit" CssClass="btn btn-danger" OnClick="btnSubmitRejectionReason_Click" />
            
          </div>
        </div>
    </asp:Panel>
  </div>
</div>

        <style>
        

            /* Apply a cleaner, more compact style to the table */
            .table {
                width: 100%;
                border-collapse: collapse;
            }

            .table th,
            .table td {
                padding: 8px 12px;
                /* Reduced padding for a more compact look */
                text-align: left;
                /* Left-align text for better readability */
                vertical-align: middle;
                /* Vertically align content */
                border: 1px solid #ddd;
                /* Light border for clean separation */
                font-size: 13px;
            }

            /* Adjust the header row */
            .table th {
                background-color: #1A1A40;
                color:white;
                font-size: 14px;
                font-weight: bold;
                text-align: center;
            }

          

            /* Hover effect for rows */
            .table tbody tr:hover {
                background-color: #f1f1f1;
            }

            /* Make the buttons smaller */
            .btn {
                font-size: 12px;
                padding: 5px 10px;
                margin: 0;
            }
            .dropdown-small {
    font-size: 13px !important;
}
        </style>


        <script type="text/javascript">
    function storeRejectArgument(commandArg) {
        document.getElementById('<%= hfRejectCommandArg.ClientID %>').value = commandArg;
        $('#rejectionModal').modal('show');
    }
        </script>

    </section>
</asp:Content>