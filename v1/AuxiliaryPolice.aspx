<%@ Page Title="Auxiliary Police" Language="C#" MasterPageFile="~/v1/Main.Master" AutoEventWireup="true" CodeBehind="AuxiliaryPolice.aspx.cs" Inherits="vms.v1.AuxiliaryPolice" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Content Header -->
    <div class="content-header">
        <div class="container-fluid">
            <div class="row mb-2">
                <div class="col-sm-6">
                    <h6><strong>Verified Report</strong></h6>
                </div>
                <div class="col-sm-6">
                    <ol class="breadcrumb float-sm-right">
                        <li class="breadcrumb-item"><a href="Dashboard.aspx">Home</a></li>
                        <li class="breadcrumb-item active">AP Report </li>
                    </ol>
                </div>
            </div>
        </div>
    </div>

    <!-- Main Content -->
    <div class="container-fluid">
        <div class="card shadow-sm">
            <div class="card-header text-white" style="background-color: #1A1A40;">
                Auxiliary Police Summary Report
            </div>
            <div class="card-body scrollable-card">
                <!-- Date -->
                <div class="form-group">
                    <label for="txtDate" style="font-size: 12px;">Select Date:</label>
                    <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" style="font-size: 13px;" TextMode="Date" AutoPostBack="true" OnTextChanged="txtDate_TextChanged" />
                </div>

                <!-- Shift -->
                <div class="form-group">
                    <label for="ddlShift" style="font-size: 12px;">Select Shift:</label>
                    <asp:DropDownList ID="ddlShift" runat="server" CssClass="form-control" style="font-size: 13px;"  AutoPostBack="true" OnSelectedIndexChanged="ddlShift_SelectedIndexChanged">
                        <asp:ListItem Text="0800-2000" Value="0800-2000" />
                        <asp:ListItem Text="2000-0800" Value="2000-0800" />
                    </asp:DropDownList>
                </div>

         <div class="form-group">
    <label style="font-size: 12px;">Day :</label>
    <asp:TextBox ID="lblDayName" runat="server" CssClass="form-control" style="font-size: 13px;"  />
</div>

  <div class="form-group">
    <label style="font-size: 12px;">Guard:</label>
</div>
<div style="font-size: 13px;">
    <asp:Literal ID="litAllGuards" runat="server" />
</div>     <hr />
                  <div class="form-group">
  <label style="font-size: 12px;" >Attach Patrolling Report </label>
  <asp:FileUpload ID="FileUpload2" runat="server" CssClass="form-control-file" style="font-size: 13px;"  />
  </div>
                <!-- CCTV Abnormal -->
                <div class="form-group">
                    <label style="font-size: 12px;" >Offline CCTV:</label>
                    <asp:TextBox ID="lblCCTVAbnorm" runat="server" CssClass="form-control" style="font-size: 13px;" ></asp:TextBox> 
                </div>

                 <div class="form-group">
     <label style="font-size: 12px;" >Contractor Details / Workers On-Site:</label>
     <asp:TextBox ID="ContractorDetails" runat="server" CssClass="form-control" style="font-size: 13px;" ></asp:TextBox> 
 </div>
 <hr />
                


                <!-- Incident Information -->
                <div class="form-group">
                    <label style="font-size: 12px;" >Incident Title:</label>
                   <asp:TextBox ID="lblIncidentTitle" runat="server" CssClass="form-control" style="font-size: 13px;"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label style="font-size: 12px;" >Incident Description:</label>
                  <asp:TextBox ID="lblIncidentDesc" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="1" style="font-size: 12px;"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label style="font-size: 12px;" >Action Taken:</label>
                    <asp:TextBox ID="lblActionTaken" runat="server" CssClass="form-control" style="font-size: 13px;" TextMode="MultiLine" Rows="1" ></asp:TextBox>
                </div>
                
                <div class="form-group">
                    <label style="font-size: 12px;" >Incident Photo :</label>
                    <asp:Literal ID="litIncidentPhoto" runat="server" />

                </div>
                 <hr />

                <!-- KPL Comment -->
                <div class="form-group">
                    <label for="txtKPLComments" style="font-size: 12px;" >Auxiliary Police Summary:</label>
                    <asp:TextBox ID="txtKPLComments" runat="server" CssClass="form-control"  style="font-size: 13px;"  TextMode="MultiLine" Rows="2" />
                </div>

                <asp:Button ID="btnVerified" runat="server" Text="Verified" CssClass="btn btn-sm btn-dark" OnClick="btnVerified_Click" />
            </div>
        </div>
    </div>

    <style>
        .scrollable-card {
            max-height: 70vh;
            overflow-y: auto;
        }

       

       

        .form-control[readonly],
        .form-control:disabled {
            background-color: #f8f9fa;
            color: #495057;
        }
    </style>
</asp:Content>
