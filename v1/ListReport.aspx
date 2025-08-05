<%@ Page Title="List Report" Language="C#" MasterPageFile="~/v1/Main.Master" AutoEventWireup="true" CodeBehind="ListReport.aspx.cs" Inherits="vms.v1.ListReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Content Header -->
    <div class="content-header">



        <div class="row g-3">
            <!-- Incidents Card (Dropdown Style) -->
            
                <div class="card text-white text-center w-100" style="background-color: #1A1A40; font-size: 0.8rem;">
                    <div class="card-header" style="font-size: 0.85rem;">
                        Incidents - <asp:Label ID="currentMonthLabel2" runat="server"></asp:Label>
                    </div>

                    <div class="dropdown">
                        <button class="btn btn-outline-light dropdown-toggle w-100" type="button" id="incidentDropdown" data-bs-toggle="dropdown" aria-expanded="false">

                        </button>
                        <ul id="Incidents" runat="server" class="dropdown-menu w-100" style="max-height: 200px; overflow-y: auto;">
                            <!-- Server-side content will be injected here -->
                        </ul>
                     </div>

                </div>
            </div>


     
       


    </div>


    <!-- Main content -->
    <section class="content">
        <div class="container-fluid px-4">
            <div class="row">

                <div class="col-md-6">
                    <!-- Date Picker + Search -->
                    <div class="date-picker-section d-flex align-items-center justify-content-start gap-2">
                        <asp:Label ID="lblDate" runat="server" Text="Choose Date  :  " CssClass="me-1" style="margin-right: 10px;"></asp:Label>
                        <asp:TextBox ID="txtReportDate" runat="server" CssClass="form-control form-control-sm" TextMode="Date" />


                        <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-search" OnClick="btnSearch_Click" ToolTip="Search">
                            <i class="fas fa-search"></i>
                        </asp:LinkButton>

                        <asp:LinkButton ID="btnAddNewReport" runat="server" CssClass="btn btn-search text-black" OnClick="btnAddNewReport_Click" ToolTip="Add New Report">
                            <i class="fas fa-plus me-1"></i>
                        </asp:LinkButton>
                        <asp:Button ID="btnDownloadCSV" runat="server" Text="Download CSV" CssClass="btn btn-sm btn-success" OnClick="btnDownloadCSV_Click" />

                    </div>
                </div>
            </div>

            <!-- Security Records Display -->
            <div class="row mt-4">
                <asp:Label ID="lblNoRecords" runat="server" ForeColor="Red" CssClass="mt-2 d-block" Visible="false" />
                <div class="report-box">
                    <asp:Literal ID="ltReportContent" runat="server"></asp:Literal>
                </div>
            </div>
        </div>
    </section>


    <!-- Styling -->
    <style>
        .dropdown-menu {
            background-color: white;
            border: 1px solid #ccc;
            border-radius: 4px;
            color: white !important;
            font-size: 0.85rem;
        }

        /* Menu Item Hover Effect */
        .dropdown-item:hover {
            background-color: #1A1A40;
            color: white;
        }

        /* Optional: Card Styling Tweaks */
        .card-header {
            font-size: 0.85rem;
            font-weight: bold;
        }

        .btn-add-report {
            background-color: #1A1A40;
            color: white;
            display: inline-flex;
            align-items: center;
            padding: 0.375rem 0.75rem;
            border: none;
            font-size: 0.875rem;
        }


        .btn-search {
            background-color: transparent;

            border: none;

            color: black;

            font-size: 0.50rem;

            padding: 0.375rem 0.75rem;

        }

        .btn-search:hover {
            color: #555;
            /* Optional: darken color on hover for better visibility */
        }

        .btn-search i {
            font-size: 1.00rem;
            /* Optional: increase icon size */
        }


        .date-picker-section {
            font-size: 0.875rem;
            /* Slightly smaller text */
        }

        .date-picker-section .form-control {
            font-size: 0.875rem;
            /* Small input field text */
            padding: 0.25rem 0.5rem;
            /* Adjust padding for a compact look */
        }

        .text-dark {
            background: none;
            border: none;
            text-decoration: none;
            font-size: 16px;
            cursor: pointer;
        }

        .text-dark:hover {
            color: #007bff;
            /* Optional hover color */
        }

        .report-box {
            max-height:
                370px;
            /* increase height */
            overflow-y: auto;
            width: 100%;


        }

        .report-content {
            font-size: 14px;
            line-height: 1.6;
            width: 100%;
            padding: 20px;
            border: 1px solid #ddd;
            border-radius: 8px;
            margin-bottom: 20px;
            background-color: #fdfdfd;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.05);
        }


        .report-header {
            font-size: 14px;
            margin-bottom: 10px;
            font-weight: bold;

        }

        .report-in-charge ul,
        .report-remarks ul {
            margin: 0;
            padding-left: 20px;
            list-style-type: disc;

        }

        .report-in-charge li,
        .report-remarks li {
            margin-bottom: 5px;

        }

        .report-footer {
            font-weight: bold;
            margin-top: 15px;

        }

        .report-box hr {
            margin: 15px 0;
            border: 0;
            border-top: 1px solid #ccc;

        }

        .report-in-charge {
            margin-bottom: 15px;

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

        .form-control {
            width: 200px;
        }
    </style>
    <script type="text/javascript">
        function printReport() {
            var content = document.getElementById('reportContent').innerHTML;
            var printWindow = window.open('', '', 'height=600,width=800');
            printWindow.document.write('<html><head><title>Print Report</title></head><body>');
            printWindow.document.write(content);
            printWindow.document.write('</body></html>');
            printWindow.document.close(); // Necessary for IE
            printWindow.print();
        }
    </script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>


</asp:Content>