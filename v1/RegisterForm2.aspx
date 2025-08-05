<%@ Page Title="Register Form 2" Language="C#" MasterPageFile="~/v1/Main.Master" AutoEventWireup="true" CodeBehind="RegisterForm2.aspx.cs" Inherits="vms.v1.RegisterForm2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Content Header -->
    <div class="content-header">
        <div class="container-fluid px-4">
            <div class="row mb-2 justify-content-between align-items-center">
                <div class="col d-flex align-items-center">
                    <h4 class="mb-0 d-flex align-items-center"><i class="fas fa-user-shield text-dark me-2"></i><strong>POST 2</strong></h4>

               
                </div>

                <div class="col-auto">
                    <ol class="breadcrumb mb-0">
                        <li class="breadcrumb-item"><a href="Dashboard.aspx">Home</a></li>
                        <li class="breadcrumb-item active">Register-IN</li>
                    </ol>
                </div>
            </div>

            <!-- Record Type Dropdown -->
            <div class="form-group">
                <label for="ddlRecordType" class="small">Record Type</label>
                <asp:DropDownList ID="ddlRecordType" runat="server" CssClass="form-control form-control-sm" AutoPostBack="true" OnSelectedIndexChanged="ddlRecordType_SelectedIndexChanged">
                    <asp:ListItem Text="Select Type" Value="" />
                    <asp:ListItem Text="Vehicle" Value="vehicle" />
                    <asp:ListItem Text="Walk-in Visitor" Value="visitor" />
                    <asp:ListItem Text="Item Declaration" Value="item" />
                    <asp:ListItem Text="Container" Value="container" />
                </asp:DropDownList>
            </div>
        </div>
    </div>

    <!-- Main Content -->
    <section class="content">
        <div class="container-fluid px-4">

            <!-- Alert Message -->
            <div>
                <p id="pMsg" class="text-center" runat="server" role="alert">
                    <strong>
                        <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
                    </strong>
                </p>
            </div>

            <!-- Vehicle Panel -->
            <asp:Panel ID="pnlVehicleList" runat="server" Visible="false">
                <div class="card w-100">
                    <div class="card-header text-white" style="background-color: #1A1A40;">
                        <h5 class="card-title mb-0">Vehicle Registration</h5>
                    </div>
                    <div class="card-body p-2" style="max-height: 400px; overflow-y: auto; overflow-x: auto;">
                        <asp:Repeater ID="rptVehicles" runat="server" OnItemCommand="rptVehicles_ItemCommand">
                            <HeaderTemplate>
                                <table class="table table-sm table-bordered text-sm align-middle">
                                    <thead class="table-light">
                                        <tr>
                                            <th>Plate</th>
                                            <th>Vehicle</th>
                                            <th>Name</th>
                                            <th>Company</th>
                                            <th>Purpose</th>
                                            <th>No D.O</th>
                                            <th>Item Type</th>
                                            <th>Block</th>
                                            <th>Post 2</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("PLATE_NO") %></td>
                                    <td><%# Eval("VEHICLE_TYPE") %></td>
                                    <td><%# Eval("NAME") %></td>
                                    <td><%# Eval("COMPANY") %></td>
                                    <td>
                                        <asp:TextBox ID="txtPurpose" runat="server" CssClass="form-control form-control-sm" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDO" runat="server" CssClass="form-control form-control-sm" />
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtItemType" runat="server" CssClass="form-control form-control-sm" />
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlBlock" runat="server" CssClass="form-select form-select-sm">
                                            <asp:ListItem Text="..." Value="" />
                                            <asp:ListItem Text="A" Value="A" />
                                            <asp:ListItem Text="B" Value="B" />
                                            <asp:ListItem Text="C" Value="C" />
                                            <asp:ListItem Text="Others" Value="Others" />
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:LinkButton ID="btnRegister" runat="server" CommandName="RegisterPost2" CommandArgument='<%# Eval("VEHICLE_ID") %>' CssClass="btn btn-sm btn-primary">
                                            Register
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </tbody>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </asp:Panel>


            <!-- Visitor Form Panel -->
            <asp:Panel ID="pnlVisitorForm" runat="server" Visible="false">
                <div class="card w-100">
                    <div class="card-header text-white" style="background-color: #1A1A40;">
                        <h3 class="card-title">Walk-IN Visitor Registration</h3>
                    </div>

                    <div class="card-body">

                        <div class="row ">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label for="txtVisitorName" class="small">Name</label>
                                    <input type="text" class="form-control form-control-sm" runat="server" id="txtVisitorName" placeholder="Enter Name" />
                                </div>
                            </div>

                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label for="txtVisitorCompany" class="small">Company</label>
                                    <input type="text" class="form-control form-control-sm" runat="server" id="txtVisitorCompany" placeholder="Enter Company" />
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label for="txtVisitorIC" class="small">IC Number</label>
                                    <input type="text" class="form-control form-control-sm" runat="server" id="txtVisitorIC" placeholder="Enter IC No" />
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label for="txtVisitorPurpose" class="small">Purpose</label>
                                    <asp:TextBox ID="txtVisitorPurpose" runat="server" CssClass="form-control form-control-sm" placeholder="Enter Purpose" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label for="ddlVisitorBlock" class="small">Block</label><br />
                                    <asp:DropDownList ID="ddlVisitorBlock" runat="server" CssClass="form-select form-control-sm">
                                        <asp:ListItem Text="..." Value="" />
                                        <asp:ListItem Text="A" Value="A" />
                                        <asp:ListItem Text="B" Value="B" />
                                        <asp:ListItem Text="C" Value="C" />
                                        <asp:ListItem Text="Others" Value="Others" />
                                    </asp:DropDownList>
                                </div>

                            </div>
                        </div>
                    </div>

                    <div class="card-footer text-center">
                        <asp:Button ID="btnSubmitVisitor" runat="server" Text="Submit" CssClass="btn btn-sm btn-primary" OnClick="btnSubmitVisitor_Click" />
                    </div>
                </div>
            </asp:Panel>

            <!-- Item Declaration Panel -->
            <asp:Panel ID="pnlItemDeclareForm" runat="server" Visible="false">
                <div class="card w-100">
                    <div class="card-header text-white" style="background-color: #1A1A40;">
                        <h3 class="card-title">Item Declaration Form</h3>
                    </div>
                    <div class="card-body row g-3">

                        <div class="col-sm-6">
                            <div class="form-group">
                                <label for="txtItemName" class="form-label small">Name</label>
                                <input type="text" id="txtItemName" runat="server" class="form-control form-control-sm" placeholder="Enter Name" />
                            </div>
                        </div>

                        <div class="col-sm-6">
                            <div class="form-group">
                                <label for="txtEmpNo" class="form-label small">Employee No </label>
                                <input type="text" id="txtEmpNo" runat="server" class="form-control form-control-sm" placeholder="Enter Employee No" />
                            </div>
                        </div>

                        <div class="col-sm-6">
                            <div class="form-group">
                                <label for="txtItemType" class="form-label small">Item Type</label>
                                <input type="text" id="txtDeclaredItemType" runat="server" class="form-control form-control-sm" placeholder="Enter Item Type" />
                            </div>
                        </div>

                        <div class="col-sm-6">
                            <div class="form-group">
                                <label for="txtSerialPartNo" class="form-label small">Serial No / Part No </label>
                                <input type="text" id="txtSerialPartNo" runat="server" class="form-control form-control-sm" placeholder="Enter Serial/Part No" />
                            </div>
                        </div>

                        <div class="col-sm-6">
                            <div class="form-group">
                                <label for="txtTotalItemIn" class="form-label small">Total Item In</label>
                                <input type="number" id="txtTotalItemIn" runat="server" class="form-control form-control-sm" placeholder="Enter Total" />
                            </div>
                        </div>

                    </div>
                    <div class="card-footer text-center">
                        <asp:Button ID="btnSubmitItemDeclare" runat="server" Text="Declare Item" CssClass="btn btn-sm btn-success" OnClick="btnSubmitItemDeclare_Click" />
                    </div>
                </div>
            </asp:Panel>
            <!-- Container Declaration Panel -->
            <asp:Panel ID="pnlContainerForm" runat="server" Visible="false">
                <div class="card w-100">
                    <div class="card-header text-white" style="background-color: #1A1A40;">
                        <h3 class="card-title">Container Registration</h3>
                    </div>
                    <div class="card-body row g-3" style="max-height: 300px; overflow-y: auto;">

                        <!-- Driver Name -->
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label for="txtDriverName" class="form-label small">Driver Name</label>
                                <input type="text" id="txtDriverName" runat="server" class="form-control form-control-sm" placeholder="Enter Driver Name" />
                            </div>
                        </div>

                        <!-- IC No -->
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label for="txtDriverICNo" class="form-label small">IC No</label>
                                <input type="text" id="txtDriverICNo" runat="server" class="form-control form-control-sm" placeholder="Enter IC Number" />
                            </div>
                        </div>

                        <!-- Vehicle Plate No -->
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label for="txtContainerPlateNo" class="form-label small">Vehicle Plate No</label>
                                <input type="text" id="txtContainerPlateNo" runat="server" class="form-control form-control-sm" placeholder="Enter Plate Number" />
                            </div>
                        </div>

                        <!-- Company Address -->
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label for="txtContainerCompany" class="form-label small">Company Address</label>
                                <input type="text" id="txtContainerCompany" runat="server" class="form-control form-control-sm" placeholder="Enter Company Address" />
                            </div>
                        </div>

                        <!-- Prime Mover No -->
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label for="txtPrimeMoverNo" class="form-label small">Prime Mover No</label>
                                <input type="text" id="txtPrimeMoverNo" runat="server" class="form-control form-control-sm" placeholder="Enter Prime Mover No" />
                            </div>
                        </div>

                        <!-- Choose Option for Container -->
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label for="ddlContainerOption" class="form-label small">Select Option</label>
                                <select id="ddlContainerOption" runat="server" class="form-control form-control-sm">
                                    <option value="">-Select-</option>
                                    <option value="headOnly">Head Only - No container yet</option>
                                    <option value="withContainerEmpty">With Container (Empty)</option>
                                    <option value="withContainerFilled">With Container (Filled)</option>
                                </select>
                            </div>
                        </div>

                        <!-- Container No (conditional display) -->
                        <div class="col-sm-6" id="containerNoDiv" style="display:none;">
                            <div class="form-group">
                                <label for="txtContainerNo" class="form-label small">Container No</label>
                                <input type="text" id="txtContainerNo" runat="server" class="form-control form-control-sm" placeholder="Enter Container Number" />
                            </div>
                        </div>

                        <!-- Seal No (conditional display) -->
                        <div class="col-sm-6" id="sealNoDiv" style="display:none;">
                            <div class="form-group">
                                <label for="txtSealNo" class="form-label small">Seal No</label>
                                <input type="text" id="txtSealNo" runat="server" class="form-control form-control-sm" placeholder="Enter Seal Number" />
                            </div>
                        </div>

                        <!-- Acknowledgement Section (same row) -->
                        <div class="col-sm-6" id="acknowledgementDiv" style="display: none;">
                            <div class="form-group">
                                <label class="form-label small">Received in good order/condition?</label>
                                <div>
                                    <input type="radio" id="ackYes" name="ackCondition" value="Yes" runat="server" />
                                    <label for="ackYes" style="font-size: 13px;">Yes</label>
                                    <input type="radio" id="ackNo" name="ackCondition" value="No" runat="server" class="ms-3" />
                                    <label for="ackNo" style="font-size: 13px;">No</label>
                                </div>
                                <small class="text-muted">
                                    Select "Yes" if the seal number matches the documentation, the seal is not broken or cut, and there is no visible damage to the container.
                                </small>
                            </div>
                        </div>

                        <!-- Approved by Section (Security Name Dropdown) -->
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label for="ddlSecurityName" class="form-label small fw-bold">Approved by</label>
                                <asp:DropDownList ID="ddlSecurityName" runat="server" CssClass="form-control form-control-sm">
                                </asp:DropDownList>
                            </div>
                        </div>

                    </div>
                    <div class="card-footer text-center">
                        <asp:Button ID="btnRegisterEntry" runat="server" Text="Register Entry" CssClass="btn btn-sm btn-primary" OnClick="btnRegisterEntry_Click" />
                    </div>
                </div>
            </asp:Panel>






        </div>
    </section>

    <!-- Styles -->
    <style>
        .btn.btn-sm {
            font-size: 0.875rem;
            padding: 0.475rem 1.45rem;
            background-color: #1A1A40;
            border-color: #1A1A40;
            color: white;
        }

        .btn.btn-sm:hover {
            background-color: #8989ae;
            border-color: #8989ae;
            color: black;
        }

        .btn.btn-switch-post {
            font-size: 0.75rem;
            padding: 0.3rem 1rem;
            background-color: #e1eaf4;
            border-color: #e1eaf4;
            color: black;
            margin-top: 8px;
        }

        .btn.btn-switch-post:hover {
            background-color: #8989ae;
            border-color: #8989ae;
            color: black;
        }
    </style>

    <script>
        document.addEventListener("DOMContentLoaded", function() {
            var ddl = document.getElementById('<%= ddlContainerOption.ClientID %>');
            var containerDiv = document.getElementById("containerNoDiv");
            var sealDiv = document.getElementById("sealNoDiv");
            var ackDiv = document.getElementById('acknowledgementDiv');

            function toggleFields() {
                var option = ddl.value;
                containerDiv.style.display = "none";
                sealDiv.style.display = "none";
                ackDiv.style.display = "none";

                if (option === "withContainerEmpty" || option === "withContainerFilled") {
                    containerDiv.style.display = "block";
                }
                if (option === "withContainerFilled") {
                    sealDiv.style.display = "block";
                    ackDiv.style.display = "block";
                }
            }

            ddl.addEventListener("change", toggleFields);

            // run it once on load if you want to set visibility based on selected option
            toggleFields();
        });
    </script>
    <script type="text/javascript">
        // Function to show the Toast notification
        function showToast(message) {
            var toast = document.getElementById("toast");
            toast.textContent = message;
            toast.className = "toast show";
            setTimeout(function () {
                toast.className = toast.className.replace("show", "");
            }, 3000); // Toast will disappear after 3 seconds
        }
    </script>

</asp:Content>