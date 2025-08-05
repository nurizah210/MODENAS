<%@ Page Title="Leave Request 2" Language="C#" MasterPageFile="~/v1/Main.Master" AutoEventWireup="true" CodeBehind="LeaveRequest2.aspx.cs" Inherits="vms.v1.LeaveRequest2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Page Header -->
    <div class="content-header">
        <div class="container-fluid">
            <div class="row mb-2">
                <div class="col-sm-6">
                    <h6 class="font-weight-bold mb-0 mr-3">Exit Request Form (Office) </h6>
                </div>
                <div class="col-sm-6">
                    <ol class="breadcrumb float-sm-right">
                        <li class="breadcrumb-item"><a href="Dashboard.aspx">Home</a></li>
                        <li class="breadcrumb-item active">Exit Request</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>

    <!-- Main Content -->
    <section class="content">
        <div class="container-fluid custom-container">
            <div class="card">
                <div class="card-header text-white" style="background-color: #1A1A40;">
                    <h3 class="card-title">Submit Your Request</h3>
                </div>

                <div class="card-body">
                    <!-- Leave Type Section -->
                    <div class="form-group">
                        <label for="ddlLeaveType" class="small">Exit Type</label>
                        <asp:DropDownList ID="ddlLeaveType" runat="server" CssClass="form-control form-control-sm" AutoPostBack="true" OnSelectedIndexChanged="ddlLeaveType_SelectedIndexChanged">
                            <asp:ListItem Text="-- Select Exit Type --" Value="" />
                            <asp:ListItem Text="OFFICE BUSINESS" Value="Office Matter" />
                            <asp:ListItem Text="PERSONAL REASON" Value="Personal Reason" />
                        </asp:DropDownList>
                    </div>

                    <!-- Name and Employee No Section -->
                    <div class="form-group row">
                        <div class="col-md-6">
                            <label for="txtName" class="small">Name</label>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control form-control-sm" ReadOnly="true" />
                        </div>
                        <div class="col-md-6">
                            <label for="txtEmpNo" class="small">Employee No</label>
                            <asp:TextBox ID="txtEmpNo" runat="server" CssClass="form-control form-control-sm" ReadOnly="true" />
                        </div>
                    </div>

                    <!-- Department and Return Status Section -->
                    <div class="form-group row">
                        <div class="col-md-6">
                            <label for="txtDepartment" class="small">Department</label>
                            <asp:TextBox ID="txtDepartment" runat="server" CssClass="form-control form-control-sm" ReadOnly="true" />
                        </div>
                        <div class="col-md-6">
                            <label for="rblReturnStatus" class="small">Will you return on the same day?</label>
                            <div class="bg-white" style="width: 200px; font-size: 12px;">
                                <asp:RadioButtonList ID="rblReturnStatus" runat="server" CssClass="radio-button-list" RepeatLayout="Flow">
                                    <asp:ListItem Text="Yes" Value="Yes" />
                                    <asp:ListItem Text="No" Value="No" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>

                <!-- Date Out and Time Out in one row (centered neatly) -->
<div class="form-group row">
    <div class="col-md-6">
        <label for="txtDateOut" class="small">Date Out</label>
        <asp:TextBox ID="txtDateOut" runat="server" CssClass="form-control form-control-sm" TextMode="Date" />
    </div>

    <div class="col-md-6">
        <label for="txtTimeOut" class="small">Time Out</label>
        <asp:TextBox ID="txtTimeOut" runat="server" CssClass="form-control form-control-sm" TextMode="Time" />
    </div>
</div>

                    <!-- Personal Reason Panel -->
                    <asp:Panel ID="pnlPersonal" runat="server" Visible="false">
                        <div class="form-group row">
                            <!-- Replacement Radio Button Section -->
                            <div class="col-md-6">
                                <label class="small">Will Be Replaced By</label>
                                <div class="bg-white" style="width: 200px; font-size: 12px;">
                                    <asp:RadioButtonList ID="rblReplacement" runat="server" CssClass="radio-button-list" RepeatLayout="Flow">
                                        <asp:ListItem Text="Salary Cut" Value="Salary Cut" />
                                        <asp:ListItem Text="Annual Leave Cut" Value="Annual Leave Cut" />
                                    </asp:RadioButtonList>
                                </div>
                            </div>

                            <!-- Reason DropDown List Section -->
                            <div class="col-md-6">
                                <label for="ddlReason" class="small">Reason</label>
                                <asp:DropDownList ID="ddlReason" runat="server" CssClass="form-control form-control-sm">
                                    <asp:ListItem Text="-- Select Reason --" Value="" />
                                    <asp:ListItem Text="Family Emergency" Value="Family Emergency" />
                                    <asp:ListItem Text="Personal Health" Value="Personal Health" />
                                    <asp:ListItem Text="Personal Appointment" Value="Personal Appointment" />
                                    <asp:ListItem Text="Attending to Children" Value="Attending to Children" />
                                    <asp:ListItem Text="Elderly Care" Value="Elderly Care" />
                                    <asp:ListItem Text="Transportation Issue" Value="Transportation Issue" />
                                    <asp:ListItem Text="Other Reasons" Value="Other Reasons" />
                                </asp:DropDownList>
                            </div>
                        </div>
                    </asp:Panel>

                    <!-- Office Matter Panel -->
                    <asp:Panel ID="pnlOffice" runat="server" Visible="false">
                        <div class="form-group row">
                            <!-- Vehicle Plate No. Section -->
                            <div class="col-md-6">
                                <label for="txtPlateNo" class="small">Vehicle Plate No.</label>
                                <asp:TextBox ID="txtPlateNo" runat="server" CssClass="form-control form-control-sm" Style="width: 100%;" />
                            </div>

                            <!-- Reason DropDown List Section -->
                            <div class="col-md-6">
                                <label for="ddlReason" class="small">Reason</label>
                                <asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-control form-control-sm">
                                    <asp:ListItem Text="-- Select Reason --" Value="" />
                                    <asp:ListItem Text="Official Meeting" Value="Official Meeting" />
                                    <asp:ListItem Text="Site Visit" Value="Site Visit" />
                                    <asp:ListItem Text="Client Appointment" Value="Client Appointment" />
                                    <asp:ListItem Text="Work-related Emergency" Value="Work-related Emergency" />
                                    <asp:ListItem Text="External Training" Value="External Training" />
                                    <asp:ListItem Text="Work Errand" Value="Work Errand" />
                                    <asp:ListItem Text="Inspection Visit" Value="Inspection Visit" />
                                    <asp:ListItem Text="Official Event" Value="Official Event" />
                                    <asp:ListItem Text="Follow-up at Government Office" Value="Follow-up at Government Office" />
                                    <asp:ListItem Text="Other Office-related Reasons" Value="Other Office-related Reasons" />
                                </asp:DropDownList>
                            </div>
                        </div>
                    </asp:Panel>

                    <!-- Request To Section -->
                    <div class="form-group">
                        <label for="ddlHodList" class="small">Request to:</label>
                        <asp:DropDownList ID="ddlHodList" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Select HOD" Value="" />
                        </asp:DropDownList>
                    </div>

                    <!-- Submit Button Section -->
                    <div class="form-group text-center mt-3">
                        <asp:Button ID="BtnSubmit" runat="server" CssClass="btn Btn-softyellow btn-sm" Text="Submit" OnClick="BtnSubmit_Click" OnClientClick="return confirm('Are you sure you want to send this request?');" />
                    </div>

                    <!-- Error Message Section -->
                    <div class="form-group text-center">
                        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" Visible="false" />
                    </div>



                    <!-- Note Section -->
                    <div class="form-group mt-3 text-muted text-center">
                        <small>Note: Workers who leave for more than 4 hours are considered absent from duty and not paid salary on that day.</small>
                    </div>
                </div>


                <!-- Button Styles -->
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
                </style>
    </section>
</asp:Content>