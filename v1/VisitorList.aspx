<%@ Page Title="" Language="C#" MasterPageFile="~/v1/Main.Master" AutoEventWireup="true" CodeBehind="VisitorList.aspx.cs" Inherits="vms.v1.VisitorList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Processing div -->
    <div class="modal fade" id="pleaseWaitDialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h1>Processing...</h1>
                </div>
                <div class="modal-body">
                    <div class="progress">
                        <div class="progress-bar progress-bar-striped progress-bar-animated active" role="progressbar" aria-valuenow="75" aria-valuemin="0" aria-valuemax="100" style="width: 100%"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- End Processing div -->

    <div id="page-wrapper">
        <div class="container-fluid">

            <div class="col-md-8 col-lg-12">
                <div class="card">
                    <div class="card-body">
                        <div class="row">
    <div class="form-group col-md-4">
        <label>FROM </label>
        <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control  form-control-sm" TextMode="Date"></asp:TextBox>
    </div>

    <div class="form-group col-md-4">
        <label>TO </label>
        <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control form-control-sm" TextMode="Date"></asp:TextBox>
    </div>
</div>

                        <div class="row">

                            <!-- Visitor Type Dropdown and Submit Button in Same Row -->

                            <div class="form-group col-md-8">
                                <!-- Dropdown -->
                                <asp:DropDownList ID="txtVisitorType" runat="server" CssClass="form-control form-control-sm" style="width: 100%;">
                                     <asp:ListItem Text="--Choose Category--" Value="" />
                                    <asp:ListItem Text="Vehicle Visitor" Value="vehicle visitor" />
                                    <asp:ListItem Text="TNB and Communication Subs Contractor" Value="contractor tnb" />
                                    <asp:ListItem Text="Transporter" Value="parking" />
                                     <asp:ListItem Text="Staff Movement" Value="staff movement" />
                                     <asp:ListItem Text="Vehicle Visitor 2" Value="vehicle visitor 2" />
                                     <asp:ListItem Text="Walk-in Visitor" Value="walkin visitor" />
                                     <asp:ListItem Text="Item Declaration" Value="item declaration" />
                                     <asp:ListItem Text="Container" Value="container" />

                                </asp:DropDownList>
                            </div>

                            <div class="form-group col-md-4">
                                <!-- Submit Button -->
                               
                               <asp:Button ID="btnSave" class="btn Btn-softyellow btn-sm" runat="server" Text="Confirm" OnClick="btnSubmit_Click" />
                                <asp:Button ID="btnDownloadCSV" runat="server" Text="Download CSV" class="btn Btn-softyellow btn-sm" OnClick="btnDownloadCSV_Click" />

                            </div>
                        </div>
                    </div>
                </div>

                <div class="card">
                    <div class="card-body">
                        <!-- Display Visitor List Here -->
                        <div id="dvInfo" runat="server">
                            <asp:Label ID="lblTable" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <style>
        .btn-sm {
            font-size: 0.875rem;
            padding: 0.475rem 1.45rem;
            background-color: #1A1A40;
            border-color: #1A1A40;
            color: white; /* Make text white */
        }

        .btn-sm:hover {
            background-color: #a3a8be;
            border-color: #a3a8be;
            color: black; /* Ensure text stays white */
        }
      
    .form-control-sm {
        font-size: 15px;
        padding: 0.25rem 0.5rem;
        height: auto;
    }
      label {
        font-size: 15px;
    }
</style>


    <script type="text/javascript">
        // jQuery for AJAX handling (Optional, if you're using AJAX for loading data)
        $(document).ready(function () {
            $(document)
                .ajaxStart(function () {
                    $('#pleaseWaitDialog').modal('toggle');
                })
                .ajaxStop(function () {
                    $('#pleaseWaitDialog').modal('toggle');
                });
        });
    </script>

</asp:Content>
