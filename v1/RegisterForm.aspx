<%@ Page Title="Register Form" Language="C#" MasterPageFile="~/v1/Main.Master" AutoEventWireup="true" CodeBehind="RegisterForm.aspx.cs" Inherits="vms.v1.RegisterForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Content Header -->
    <div class="content-header">
        <div class="container-fluid px-4">
            <div class="row mb-2 justify-content-between align-items-center">
                <!-- Left Side: POST 1 + Post 2 Button -->
                <div class="col d-flex align-items-center">
                    <h4 class="mb-0 d-flex align-items-center"> <i class="fas fa-user-shield text-dark me-2"></i><strong>POST 1 </strong></h4>

                </div>
                <!-- Right Side: Breadcrumbs -->
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
                <asp:DropDownList ID="ddlRecordType" runat="server" CssClass="form-control form-control-sm" AutoPostBack="true">
                    <asp:ListItem Text="- Select -" Value="" />
                    <asp:ListItem Text="Vehicle In/Out" Value="vehicle in/out" />
                    <asp:ListItem Text="TNB / Substation" Value="tnb" />
                    <asp:ListItem Text="Transporter" Value="parking" />
                    <asp:ListItem Text="Late Staff" Value="late staff" />
                    <asp:ListItem Text="Staff Movement (OT / Company Closed)" Value="staff movement" />
                </asp:DropDownList>
            </div>
        </div>
    </div>


    <!-- Main content -->
    <section class="content">
        <div class="container-fluid px-4">
            <div>
                <p id="pMsg" class="text-center" runat="server" role="alert">
                    <strong>
                        <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
                    </strong>
                </p>
            </div>




            <asp:Panel ID="pnlVehicleForm" runat="server" Visible="false">
                <!-- Form Card -->
                <div class="card w-100">
                    <div class="card-header text-white" style="background-color: #1A1A40;">
                        <h3 class="card-title">Vehicle Registration</h3>
                    </div>

                    <div class="card-body">
                        <!-- IC and Name -->
                        <div class="row">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label for="txtIC" class="small">IC No</label>
                                    <input type="text" class="form-control form-control-sm" runat="server" id="txtIC" placeholder="Enter IC No" />
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label for="txtName" class="small">Name</label>
                                    <input type="text" class="form-control form-control-sm" runat="server" id="txtName" placeholder="Enter Name" />
                                </div>
                            </div>
                        </div>

                        <!-- No Plate and Company -->
                        <div class="row">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label for="txtNoPlate" class="small">No Plate</label>
                                    <input type="text" class="form-control form-control-sm" runat="server" id="txtNoPlate" placeholder="Enter Vehicle Plate No" />
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label for="txtCompany" class="small">Company</label>
                                    <input type="text" class="form-control form-control-sm" runat="server" id="txtCompany" placeholder="Enter Company Name" />
                                </div>
                            </div>
                        </div>

                        <!-- Location and Vehicle Type -->
                        <div class="row">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label for="ddlLocation" class="small">Location</label>
                                    <asp:DropDownList ID="ddlLocation" runat="server" CssClass="form-control form-control-sm"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label for="ddlVehicleType" class="small">Type of Vehicle</label>
                                    <asp:DropDownList ID="ddlVehicleType" runat="server" CssClass="form-control form-control-sm">
                                        <asp:ListItem Text="- Select -" Value="" />
                                        <asp:ListItem Text="Car" Value="Car" />
                                        <asp:ListItem Text="Motorcycle" Value="Motorcycle" />
                                        <asp:ListItem Text="Lorry" Value="Lorry" />
                                        <asp:ListItem Text="Van" Value="Van" />
                                        <asp:ListItem Text="Bus" Value="Bus" />
                                        <asp:ListItem Text="Others" Value="Others" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Submit Button -->
                    <div class="card-footer text-center">
                        <asp:Button ID="btnSave" runat="server" CssClass="btn Btn-softyellow btn-sm" Text="Save" OnClick="btnSave_Click" OnClientClick="return confirm('Are you sure you want to checkin this visitor?')" />
                    </div>
                </div>
            </asp:Panel>



            <asp:Panel ID="pnlTNBForm" runat="server" Visible="false">
                <!-- TNB/Substation Form Card -->
                <div class="card w-100 mt-3">
                    <div class="card-header text-white" style="background-color: #1A1A40;">
                        <h3 class="card-title">TNB / Substation Registration</h3>
                    </div>

                    <div class="card-body">
                        <div class="row">
                            <!-- No Plate -->
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label for="txtTNBNoPlate" class="small">No Plate</label>
                                    <input type="text" class="form-control form-control-sm" runat="server" id="txtTNBNoPlate" placeholder="Enter Vehicle Plate No" />
                                </div>
                            </div>

                            <!-- Name -->
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label for="txtTNBName" class="small">Name</label>
                                    <input type="text" class="form-control form-control-sm" runat="server" id="txtTNBName" placeholder="Enter Name" />
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <!-- IC No -->
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label for="txtTNBIC" class="small">IC No</label>
                                    <input type="text" class="form-control form-control-sm" runat="server" id="txtTNBIC" placeholder="Enter IC No" />
                                </div>
                            </div>

                            <!-- Purpose -->
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label for="txtTNBPurpose" class="small">Purpose</label>
                                    <input type="text" class="form-control form-control-sm" runat="server" id="txtTNBPurpose" placeholder="Enter Visit Purpose" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Submit Button -->
                    <div class="card-footer text-center">
                        <asp:Button ID="btnSaveTNB" runat="server" CssClass="btn btn-sm btn-warning" Text="Save" OnClick="btnSaveTNB_Click" OnClientClick="return confirm('Are you sure you want to checkin this visitor?');" />
                    </div>
                </div>
            </asp:Panel>


            <asp:Panel ID="pnlParkingRentalForm" runat="server" Visible="false">
                <div class="card w-100 mt-3">
                    <div class="card-header text-white" style="background-color: #1A1A40;">
                        <h3 class="card-title">Transporter</h3>
                    </div>

                    <div class="card-body">
                        <div class="row">
                            <!-- Driver Name -->
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="small">Driver Name</label>
                                    <input type="text" class="form-control form-control-sm" id="txtDriverName" runat="server" placeholder="Enter driver name" />
                                </div>
                            </div>

                            <!-- Vehicle Type (In) -->
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="small">Type of Vehicle</label>
                                    <asp:DropDownList ID="ddlVehicleTypeIn" runat="server" CssClass="form-control form-control-sm">
                                        <asp:ListItem Text="- Select -" Value="" />
                                        <asp:ListItem Text="Car" Value="Car" />
                                        <asp:ListItem Text="Motorcycle" Value="Motorcycle" />
                                        <asp:ListItem Text="Lorry" Value="Lorry" />
                                        <asp:ListItem Text="Van" Value="Van" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <!-- No Plate (In) -->
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="small">No Plate</label>
                                    <input type="text" class="form-control form-control-sm" id="txtNoPlateIn" runat="server" placeholder="Enter vehicle no plate" />
                                </div>
                            </div>

                            <!-- Purpose -->
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="small">Purpose</label>
                                    <asp:DropDownList ID="ddlPurpose" runat="server" CssClass="form-control form-control-sm">
                                        <asp:ListItem Text="- Select -" Value="" />
                                        <asp:ListItem Text="Take Lorry" Value="Take Lorry" />
                                        <asp:ListItem Text="Parking Lorry" Value="Parking Lorry" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <!-- Company -->
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="small">Company</label>
                                    <input type="text" class="form-control form-control-sm" id="txtCompanyParking" runat="server" placeholder="Enter company name" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Submit Button -->
                    <div class="card-footer text-center">
                        <asp:Button ID="btnSaveParkingRental" runat="server" CssClass="btn btn-sm btn-warning" Text="Save" OnClick="btnSaveParkingRental_Click" OnClientClick="return confirm('Are you sure you want to checkin this visitor?');" />
                    </div>
                </div>
            </asp:Panel>


            <!-- Late Staff Form Panel -->
            <asp:Panel ID="pnlLateStaffForm" runat="server" Visible="false" CssClass="card w-100 mt-3">

                <!-- Header -->
                <div class="card-header text-white py-2" style="background-color: #1A1A40;">
                    <h5 class="card-title mb-0">Late Staff Report</h5>
                </div>

                <!-- Body -->
                <div class="card-body">
                    <div class="row">

                        <!-- Employee No + Search -->
                        <div class="col-md-6 mb-2">
                            <label for="txtEmpNoLate" class="form-label small">Employee No</label>
                            <div class="input-group input-group-sm">
                                <input runat="server" id="txtEmpNoLate" class="form-control form-control-sm" placeholder="Enter Employee Number"  />
                                <div class="input-group-append">
                                    <asp:Button ID="btnSearchEmp" runat="server" CssClass="btn btn-secondary btn-sm" Text="Search" OnClick="btnSearchEmp_Click" />
                                </div>
                            </div>
                        </div>

                        <!-- Employee Name -->
                        <div class="col-md-6 mb-2">
                            <label for="txtEmpNameLate" class="form-label small">Employee Name</label>
                            <input runat="server" id="txtEmpNameLate" class="form-control form-control-sm" readonly />
                        </div>

                        <!-- Department -->
                        <div class="col-md-6 mb-2">
                            <label for="txtDeptLate" class="form-label small">Department</label>
                            <input runat="server" id="txtDeptLate" class="form-control form-control-sm" readonly />
                        </div>

                        <!-- Phone Number -->
                        <div class="col-md-6 mb-2">
                            <label for="txtPhoneLate" class="form-label small">Phone Number</label>
                            <input type="text" id="txtPhoneLate" name="txtPhoneLate" class="form-control form-control-sm" runat="server" placeholder="Enter Phone Number" />
                        </div>

                        <!-- Reason Late (DropDownList) -->
                        <div class="col-12 mb-2">
                            <label for="ddlReasonLate" class="form-label small">Reason Late</label>
                            <asp:DropDownList ID="ddlReasonLate" runat="server" CssClass="form-control form-control-sm">
                                <asp:ListItem Text="-- Select Reason --" Value="" />
                                <asp:ListItem Text="Traffic Jam" Value="Traffic Jam" />
                                <asp:ListItem Text="Overslept" Value="Overslept" />
                                <asp:ListItem Text="Family Emergency" Value="Family Emergency" />
                                <asp:ListItem Text="Public Transport Delay" Value="Public Transport Delay" />
                                <asp:ListItem Text="Medical Reason" Value="Medical Reason" />
                                <asp:ListItem Text="Other" Value="Other" />
                            </asp:DropDownList>
                        </div>

                    </div>
                </div>

                <!-- Footer -->
                <div class="card-footer text-center py-2">
                    <asp:Button ID="btnSaveLateStaff" runat="server" Text="Save" CssClass="btn btn-warning btn-sm px-4" OnClick="btnSaveLateStaff_Click" OnClientClick="openModal(); return false;" />
                </div>

            </asp:Panel>


            <!-- Modal for Additional Report Info -->
            <div class="modal fade" id="reportModal" tabindex="-1" role="dialog" aria-labelledby="reportModalLabel" aria-hidden="true">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">

                        <!-- Modal Header -->
                        <div class="modal-header text-white" style="background-color: #1A1A40;">

                            <button type="button" class="close text-white" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>

                        <!-- Modal Body -->
                        <div class="modal-body">
                            <div class="form-group">
                                <label for="txtDateReportLate" class="small">Date:</label>
                                <input type="date" id="txtDateReportLate" name="txtDateReportLate" class="form-control form-control-sm" required />
                            </div>



                            <div class="form-group">
                                <label for="txtSecurityNote" class="small">Security Note:</label>
                                <textarea id="txtSecurityNote" name="txtSecurityNote" class="form-control form-control-sm"></textarea>
                            </div>
                        </div>

                        <!-- Modal Footer -->
                        <div class="modal-footer">
                            <asp:Button ID="btnSubmitReport" runat="server" Text="Submit Report" CssClass="btn btn-sm btn-warning" OnClick="btnSubmitReport_Click" OnClientClick="return confirm('Are you sure you want to checkin this visitor?');" />
                        </div>

                    </div>
                </div>
            </div>


            <!-- Staff Movement Form Panel -->
            <asp:Panel ID="pnlStaffMovement" runat="server" Visible="false" CssClass="card w-100 mt-3">
                <div class="card-header text-white" style="background-color: #1A1A40;">
                    <h3 class="card-title">Staff Movement Report (OT/ Company Closed) </h3>
                </div>
                <div class="card-body">
                    <div class="row">

                        <div class="col-sm-6">
                            <div class="form-group">
                                <label for="txtOTEmpNo" class="small">Employee No.</label>
                                <div class="input-group input-group-sm">
                                    <input type="text" id="txtOTEmpNo" name="txtOTEmpNo" class="form-control form-control-sm" runat="server" placeholder="Enter Employee Number" />
                                    <div class="input-group-append">
                                        <asp:Button ID="btnSearchOTEmp" runat="server" CssClass="btn btn-sm btn-secondary" Text="Search" OnClick="btnSearchOTEmp_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Name -->
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label for="txtOTName" class="small">Name</label>
                                <input id="txtOTName" name="txtOTName" class="form-control form-control-sm" runat="server" readonly />
                            </div>
                        </div>


                        <div class="col-sm-6">
                            <div class="form-group">
                                <label for="txtOTDepartment" class="small">Department</label>
                                <input id="txtOTDepartment" name="txtOTDepartment" class="form-control form-control-sm" runat="server" readonly />


                            </div>
                        </div>
                        <!-- Block -->
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label for="ddlBlock" class="small">Location</label>
                                <asp:DropDownList ID="ddlBlock" runat="server" CssClass="form-control form-control-sm">
                                    <asp:ListItem Text="-- Select Location --" Value="" />
                                    <asp:ListItem Text="Main Office" Value="Main Office" />
                                     <asp:ListItem Text="Learning Centre" Value="Learning Centre" />
                                    <asp:ListItem Text="R&D Office" Value="R&D Office" />
                                    <asp:ListItem Text="A" Value="A" />
                                    <asp:ListItem Text="B" Value="B" />
                                    <asp:ListItem Text="C" Value="C" />
                                </asp:DropDownList>
                            </div>
                        </div>


                        <!-- Department -->
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label for="txtOTNoPlate" class="small">No. Plate</label>
                                <asp:TextBox ID="txtOTNoPlate" runat="server" placeholder="Enter Plate Number" class="form-control form-control-sm" />
                            </div>
                        </div>

                        <!-- Purpose -->
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label for="txtOTPurpose" class="small">Purpose</label>
                                <textarea id="txtOTPurpose" name="txtOTPurpose" class="form-control form-control-sm" runat="server" placeholder="Enter Purpose" /> 
                            </div>
                        </div>
                    </div>
                </div>


                <!-- Submit Button -->
                <div class="card-footer text-center">
                    <asp:Button ID="btnSaveStaffMove" runat="server" Text="Save" CssClass="btn btn-sm btn-warning" OnClick="btnSaveStaffMove_Click" OnClientClick="return confirm('Are you sure you want to checkin this visitor?');" />
                </div>

            </asp:Panel>
        </div>


        <div id="toast" class="toast"></div>

    </section>

    <!-- Styling -->
    <style>
        /* Align the button and Post 1 side by side */
        .content-header .row {
            margin-bottom: 0;
        }

        .content-header .col.d-flex {
            margin-bottom: 0;
            /* Remove any unwanted bottom margin */
        }

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
            padding: 0.3rem 1.0rem;
            background-color: #e1eaf4;
            border-color: #e1eaf4;
            color: black;
            margin-top: 8px;
        }



        /* Move the form card up */
        .card.w-100 {
            margin-top: -20px;
            /* You can adjust this value to fit your needs */
        }

        /* Adjust the container padding if needed */
        .container-fluid.px-4 {
            padding-top: 0;
            /* Or reduce the top padding */
        }

        /* Toast Styles */
        .toast {
            visibility: hidden;
            min-width: 250px;
            margin-left: -125px;
            background-color: #333;
            color: #fff;
            text-align: center;
            border-radius: 2px;
            padding: 16px;
            position: fixed;
            z-index: 1;
            left: 50%;
            bottom: 30px;
            font-size: 17px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.3);
        }

        .toast.show {
            visibility: visible;
            animation: fadein 0.5s, fadeout 0.5s 2.5s;
        }

        @keyframes fadein {
            from {
                bottom: 0;
                opacity: 0;
            }

            to {
                bottom: 30px;
                opacity: 1;
            }
        }

        @keyframes fadeout {
            from {
                bottom: 30px;
                opacity: 1;
            }

            to {
                bottom: 0;
                opacity: 0;
            }
        }
    </style>

    <!-- Toast Notification Element -->
    <div id="toast" class="toast"></div>

    <!-- jQuery -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <!-- Bootstrap JS (modal requires this) -->
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.bundle.min.js"></script>

    <script>
        // Function to open the modal
        function openModal() {
            $('#reportModal').modal('show');
        }

        // Remove the "required" attribute if the input is not visible
        $('form :input[required]').each(function () {
            if (!$(this).is(':visible')) {
                $(this).removeAttr('required');
            }
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