<%@ Page Title="Security Report" Language="C#" MasterPageFile="~/v1/Main.Master" AutoEventWireup="true" CodeBehind="SecurityReport.aspx.cs" Inherits="vms.v1.SecurityReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Content Header -->
    <div class="content-header">
        <div class="container-fluid">
            <div class="row mb-2">
                <div class="col-sm-6">
                    <h6><strong>Report Form</strong> </h6>
                </div>
                <div class="col-sm-6">
                    <ol class="breadcrumb float-sm-right">
                        <li class="breadcrumb-item"><a href="Dashboard.aspx">Home</a></li>
                        <li class="breadcrumb-item active">Security Report</li>
                    </ol>
                </div>

            </div>
        </div>
    </div>
    
    <!-- Card Layout -->
    <section class="content">
        <div class="container-fluid custom-container">
            <div class="card">
                <div class="card-body">

                   
                    <div class="scrollable-card">

                        <!-- Card: Daily Attendance -->
                        <div class="card mb-4">
                            <div class="card-header text-white
                                " style="background-color: #1A1A40;">
                                1. Daily Attendance
                            </div>
                            <div class="card-body">
                                <div class="form-group row">
                                    <label class="col-sm-2 col-form-label">Date</label>
                                    <div class="col-sm-4">
                                        <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date" />
                                    </div>
                                </div>

                                <div class="form-group row">
                                    <label class="col-sm-2 col-form-label">Time (Shift)</label>
                                    <div class="col-sm-4">
                                        <asp:TextBox ID="txtShiftTime" runat="server" CssClass="form-control" placeholder="e.g. 2000hrs - 0800hrs" />
                                    </div>
                                </div>

                                <div class="form-group row">
                                    <label class="col-sm-2 col-form-label">Day</label>
                                    <div class="col-sm-4">
                                        <asp:DropDownList ID="ddlDay" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="MON" />
                                            <asp:ListItem Text="TUE" />
                                            <asp:ListItem Text="WED" />
                                            <asp:ListItem Text="THU" />
                                            <asp:ListItem Text="FRI" />
                                            <asp:ListItem Text="SAT" />
                                            <asp:ListItem Text="SUN" />
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label>Post 1</label>
                                    <asp:DropDownList ID="ddlPost1Guard1" runat="server" CssClass="form-control" />
                                    <asp:DropDownList ID="ddlPost1Guard2" runat="server" CssClass="form-control mt-1" />
                                </div>

                                <div class="form-group">
                                    <label>Post 2</label>
                                    <asp:DropDownList ID="ddlPost2Guard1" runat="server" CssClass="form-control" />
                                    <asp:DropDownList ID="ddlPost2Guard2" runat="server" CssClass="form-control mt-1" />
                                    <asp:DropDownList ID="ddlPost2Guard3" runat="server" CssClass="form-control mt-1" />
                                </div>

                                <div class="form-group">
                                    <label>Post Emos</label>
                                    <asp:DropDownList ID="ddlEmosGuard1" runat="server" CssClass="form-control" />
                                    <asp:DropDownList ID="ddlEmosGuard2" runat="server" CssClass="form-control mt-1" />
                                </div>
                             <asp:Literal ID="litNames" runat="server" />


                            </div>
                        </div>

                        <!-- Card: Daily Report -->
                        <div class="card mb-4">
                            <div class="card-header text-white" style="background-color: #1A1A40;">
                                2. Daily Report
                            </div>
                            <div class="card-body">
                            
                                <div class="form-group">
                                    <label>CCTV Status</label>
                                    <asp:TextBox ID="txtCCTVWorking" runat="server" CssClass="form-control" placeholder="ONLINE CCTV" />
                                    <asp:TextBox ID="txtCCTVOffline" runat="server" CssClass="form-control mt-1" placeholder="OFFLINE CCTV" />
                                    <asp:TextBox ID="txtAbnormalities" runat="server" CssClass="form-control mt-1" placeholder ="Enter which CCTV that offline " />
                                </div>

                                <div class="form-group">
                                    <label>Contractor Details / Workers On-Site</label>
                                    <asp:TextBox ID="txtContractors" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="Provide details of contractors/ workers on-site" />
                                </div>

                               
                            </div>
                        </div>

                        <!-- Card: Incident Report -->
                        <div class="card mb-4">
                            <div class="card-header text-white" style="background-color: #800000;">
                                3. Incident Report (If any)
                            </div>
                            <div class="card-body">
                                <div class="form-group">
                                    <label>Incident Title</label>
                                    <asp:TextBox ID="txtIncidentTitle" runat="server" CssClass="form-control" placeholder="Enter the title of the incident" />
                                </div>

                                <div class="form-group">
                                    <label>Description</label>
                                    <asp:TextBox ID="txtIncidentDesc" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="Describe the incident in detail" />
                                </div>

                                <div class="form-group">
                                    <label>Actions Taken</label>
                                    <asp:TextBox ID="txtActionTaken" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="1" placeholder="Provide the actions taken to resolve the incident" />
                                </div>
                   
               
    <div class="form-group">
        <label>Attach Photos (Optional)</label>
        <asp:FileUpload ID="FileUpload1" runat="server" CssClass="form-control-file" AllowMultiple="true" />
    </div>
                            </div>
                        </div>
                  

                        <!-- Submit Button -->
                        <div class="form-group mt-4 text-center">
                            <asp:Button ID="btnSubmitReport" runat="server" Text="Submit Report" CssClass="btn btn-sm" OnClick="btnSubmitReport_Click"  OnClientClick="return confirm('Are you sure you want to submit this report?');"  />
                        </div>

                    </div>
                 

                    <style>
                        .scrollable-card {
                            max-height: 70vh;
                            overflow-y: auto;
                            padding-right: 10px;
                        }

                        .form-group {
                            font-size: 0.875rem;
                           
                        }

                        .form-control {
                            font-size: 0.875rem;
                           
                        }

                        .card-header {
                            font-size: 1rem;
                       
                        }

                        .btn-sm {
                            font-size: 0.75rem;
                           
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

                </div>
            </div>
        </div>
    </section>
</asp:Content>