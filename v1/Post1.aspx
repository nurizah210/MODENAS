<%@ Page Title="Post1" Language="C#" MasterPageFile="~/v1/Main.Master" AutoEventWireup="true" CodeBehind="Post1.aspx.cs" Inherits="vms.v1.Post1" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Saira&display=swap" rel="stylesheet">
      
   <div class=" mb-4">
    <div class="col-md-12">
        <a href="RegisterForm.aspx" class="btn w-100 text-white" style="background-color: #3c4f60; ">
            Register Visitor &gt;
        </a>
    </div>
</div>


    <!-- Summary Cards -->
    <div class="row d-flex justify-content-center mb-4">


        <div class="col-auto">
            <div class="card summary-card">
                <div class="card-head text-center">


                    <div class="card-body">

                        <strong id="date">--/--/----</strong>

                        <div class="card-body text-center">

                            <strong>Gurun, Kedah</strong>

                        </div>
                        <div class="digital-clock text-center">
                            <h3 id="digital-time">
                                <span class="digit" id="hour1">0</span><span class="digit" id="hour2">0</span>:
                                <span class="digit" id="min1">0</span><span class="digit" id="min2">0</span>:
                                <span class="digit" id="sec1">0</span><span class="digit" id="sec2">0</span>
                            </h3>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-auto">
            <div class="card summary-card">
                <div class="card-head text-center">
                    <strong>Staff Check Status</strong>
                </div>
                <div class="card-body">
                    <canvas id="staffDonutChart" width="50" height="50"></canvas>
                </div>
            </div>



        </div>

        <div class="col-auto">
            <div class="card summary-card">
                <div class="card-head text-center">
                    <strong>Visitor Check Status</strong>
                </div>
                <div class="card-body">
                    <canvas id="visitorDonutChart" width="50" height="50"></canvas>
                </div>
            </div>
        </div>

    </div>

    <!-- Main Content Section -->
    <section class="content">
        <div class="container-fluid">


            <div class="tab-content">

                <!-- Post 1 Tab -->
                <div id="post1" class="tab-pane fade show active p-3">

                    <!-- Staff Return List -->
                    <div class="table-responsive" style="max-height: 300px; overflow-y: auto;">
                        <asp:GridView ID="gvStaffList" runat="server" CssClass="small-header-grid table table-bordered table-sm" AutoGenerateColumns="False" OnRowDataBound="gvStaffList_RowDataBound" OnRowCommand="gvStaffList_RowCommand" style="table-layout: fixed;">
                            <Columns>
                                <asp:BoundField DataField="STAFF_NAME" HeaderText="STAFF NAME">
                                    <ItemStyle Font-Size="smaller" />
                                </asp:BoundField>

                                <asp:TemplateField HeaderText="TIME OUT">
                                    <ItemTemplate>
                                        <div style="display: flex; align-items: center;">
                                            <!-- Label for Time Out -->
                                            <asp:Label ID="lblTimeOut" runat="server" Text='<%# Eval("TIME_OUT") %>' Font-Size="Smaller" style="margin-right: 20px;" />

                                            <!-- Button beside the Label -->
                                            <asp:Button ID="btnOut" runat="server" Text="Out" CommandName="TimeOut" CommandArgument='<%# Container.DataItemIndex %>' CssClass="btn btn-sm" style="background-color: #1A1A40; font-size:0.8rem; color : white;" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="DATE_OUT" HeaderText="DATE OUT" DataFormatString="{0:dd/MM/yyyy}">
                                    <ItemStyle Font-Size="Smaller" />
                                </asp:BoundField>

                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Button ID="btnStaffReturn" runat="server" Text="Return" OnClick="btnStaffReturn_Click" CommandArgument='<%# Eval("STAFF_NAME") + "|" + Eval("DATE_OUT", "{0:yyyy-MM-dd}") + "|" + Eval("TIME_OUT") %>' OnClientClick="return confirm('Are you sure you want to checkin this staff?')" CssClass="btn btn-sm" style="background-color: #1A1A40; color: white;" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>

                    <!-- Post 1 Sub-Tabs -->
                    <ul class="nav nav-pills mb-2">
                        <li class="nav-item"><a class="nav-link active" style="font-size: 13px; font-weight: bold;" data-toggle="pill" href="#vehicleVisitor">VEHICLE VISITOR</a></li>
                        <li class="nav-item"><a class="nav-link" style="font-size: 13px; font-weight: bold;" data-toggle="pill" href="#tnb">TNB</a></li>
                        <li class="nav-item"><a class="nav-link" style="font-size: 13px; font-weight: bold;" data-toggle="pill" href="#parking">TRASNPORTER</a></li>
                        <li class="nav-item"><a class="nav-link" style="font-size: 13px; font-weight: bold;" data-toggle="pill" href="#movement">STAFF MOVEMENT</a></li>
                    </ul>

                    <div class="tab-content">
                        <div id="vehicleVisitor" class="tab-pane fade show active">
                            <!-- Wrap the GridView in a container div -->
                            <div class="table-responsive" style="max-height: 300px; overflow-y: auto;">
                                <asp:GridView ID="gvVehicleVisitor" runat="server" AutoGenerateColumns="False" CssClass="small-header-grid table table-bordered table-sm" style="table-layout: fixed;">
                                    <Columns>
                                        <asp:BoundField DataField="PLATE_NO" HeaderText="PLATE NO.">
                                            <ItemStyle Font-Size="smaller" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="NAME" HeaderText="NAME">
                                            <ItemStyle Font-Size="smaller" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TIME_IN" HeaderText="TIME IN" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                                            <ItemStyle Font-Size="smaller" />
                                        </asp:BoundField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:Button ID="btnCheckOut1" runat="server" Text="Check Out" CommandName="CheckOut" CommandArgument='<%# Eval("PLATE_NO") %>' OnClick="btnCheckOut1_Click" OnClientClick="return confirm('Are you sure you want to checkout this visitor?')" CssClass="btn btn-sm" style="background-color: #1A1A40; color: white;" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                        <div id="tnb" class="tab-pane fade  ">
                            <div class="table-responsive" style="max-height: 300px; overflow-y: auto;">
                                <asp:GridView ID="gvTNB" runat="server" AutoGenerateColumns="False" CssClass="small-header-grid table table-bordered table-sm" style="table-layout: fixed;">
                                    <Columns>
                                        <asp:BoundField DataField="NO_PLATE" HeaderText="PLATE NO.">
                                            <ItemStyle Font-Size="smaller" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="NAME" HeaderText="NAME">
                                            <ItemStyle Font-Size="smaller" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TIME_IN" HeaderText="TIME IN" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                                            <ItemStyle Font-Size="smaller" />
                                        </asp:BoundField>

                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:Button ID="btnCheckOut2" runat="server" Text="Check Out" CommandName="CheckOut" CommandArgument='<%# Eval("NO_PLATE") %>' OnClick="btnCheckOut2_Click" CssClass="btn btn-sm" OnClientClick="return confirm('Are you sure you want to checkout this visitor?')" style="background-color: #1A1A40; color: white;" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                        <div id="parking" class="tab-pane fade ">
                            <div class="table-responsive" style="max-height: 300px; overflow-y: auto;">
                                <asp:GridView ID="gvParking" runat="server" AutoGenerateColumns="False" CssClass="small-header-grid table table-bordered table-sm" style="table-layout: fixed;">
                                    <Columns>
                                        <asp:BoundField DataField="VEHICLE_TYPE" HeaderText="VEHICLE IN">
                                            <ItemStyle Font-Size="smaller" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="NO_PLATE" HeaderText="PLATE NO.">
                                            <ItemStyle Font-Size="smaller" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="DRIVER_NAME" HeaderText="DRIVER NAME">
                                            <ItemStyle Font-Size="smaller" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TIME_IN" HeaderText="TIME IN" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                                            <ItemStyle Font-Size="smaller" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="OUT PLATE NO">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtPlateNoOut" runat="server" CssClass="form-control form-control-sm" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="VEHICLE OUT">
                                            <ItemTemplate>

                                                <asp:DropDownList ID="ddlVehicleTypeOut" runat="server" CssClass="form-control form-control-sm">
                                                    <asp:ListItem Text="-Select-" Value="" />
                                                    <asp:ListItem Text="Lorry" Value="Lorry" />
                                                    <asp:ListItem Text="Car" Value="Car" />
                                                    <asp:ListItem Text="Motorcycle" Value="Motorcycle" />
                                                    <asp:ListItem Text="Others" Value="Others" />
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:Button ID="btnCheckOut3" runat="server" Text="Check Out" CommandName="CheckOut" CommandArgument='<%# Eval("NO_PLATE") %>' OnClick="btnCheckOut3_Click" CssClass="btn btn-sm" OnClientClick="return confirm('Are you sure you want to checkout this visitor?')" style="background-color: #1A1A40; color: white;" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                        <div id="movement" class="tab-pane fade ">
                            <div class="table-responsive" style="max-height: 300px; overflow-y: auto;">
                                <asp:GridView ID="gvMovement" runat="server" AutoGenerateColumns="False" CssClass="small-header-grid table table-bordered table-sm" style="table-layout: fixed;">
                                    <Columns>
                                        <asp:BoundField DataField="NO_PLATE" HeaderText="PLATE NO.">
                                            <ItemStyle Font-Size="smaller" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="EMP_NAME" HeaderText="NAME">
                                            <HeaderStyle Font-Size="smaller" Width="250px" />
                                            <ItemStyle Font-Size="smaller" Width="250px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="EMP_NO" HeaderText="EMP NO.">
                                            <ItemStyle Font-Size="smaller" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TIME_IN" HeaderText="TIME IN" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                                            <ItemStyle Font-Size="smaller" />
                                        </asp:BoundField>

                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:Button ID="btnCheckOut4" runat="server" Text="Check Out" CommandName="CheckOut" CommandArgument='<%# Eval("NO_PLATE") %>' OnClick="btnCheckOut4_Click" OnClientClick="return confirm('Are you sure you want to checkout this visitor?')" CssClass="btn btn-sm" style="background-color: #1A1A40; color: white;" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>


                </div>


                <!-- Modal -->
                <div class="modal fade" id="visitorOverstayModal" tabindex="-1" role="dialog" aria-labelledby="modalLabel" aria-hidden="true">
                    <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
                        <div class="modal-content">
                            <div class="modal-header text-white" style="background-color: #7a0900;">

                                <h5 class="modal-title" id="modalLabel">⚠️ Overstay Alert </h5>
                            </div>
                            <div class="modal-body">
                                <p> Visitors overstayed! Please check IMMEDIATELY.</p>
                                <table class="table table-bordered table-sm">
                                    <thead>
                                        <tr>
                                            <th>Category</th>
                                            <th>Identifier</th>
                                            <th>Time In</th>
                                            <th>Duration (Hours)</th>
                                            <th>Location</th>
                                        </tr>
                                    </thead>
                                    <tbody id="modalVisitorData">
                                        <!-- Populated from code-behind -->
                                    </tbody>
                                </table>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-sm" style="background-color: #7a0900; color: white;" data-dismiss="modal">Okay</button>

                            </div>
                        </div>
                    </div>
                </div>

    </section>

    <style>

#visitorOverstayModal .modal-dialog {
  max-width: 600px; /* or any width you want */
  margin: 1.75rem auto;
}

    .card {
            display: flex;
            flex-direction: column;
            height: 100%;

        }


        .row {
            display: flex;
            justify-content: center;
            align-items: stretch;
        }

        .summary-card {

            border: 1px solid #808080;
            width: 200px;
            background-color: #eff4f9;
        }

        .summary-card .card-body {
            font-size: 0.85rem;
            padding: 10px;
            color: black;

        }

        .summary-card .card-head {
            font-size: 0.85rem;
            padding: 10px;
            color: black;
        }

        .nav-tabs .nav-link,
        .nav-pills .nav-link {
            color: black;
            /* White text for all tabs */
            background-color: transparent;
            /* Transparent or default background */
        }

        .nav-tabs .nav-link.active,
        .nav-pills .nav-link.active {
            background-color: #c2ced9 !important;
            color: black !important;
        }

        .nav-pills .nav-link:hover,
        .nav-tabs .nav-link:hover {
            color: black;




        }

        .table-responsive {
            max-height: 300px;

            overflow-y: auto;

        }

        .table th,
        .table td {
            white-space: nowrap;
            /* Prevents text wrapping */
        }

        .table tr {
            background: white;
        }


        .table th {
            position: sticky;
            /* Keeps the header at the top */
            top: 0;
            /* Sticks it to the top of the scrollable area */
            background: #757b86;
            color: white;

        }

        .small-header-grid th {
            font-size: 12px;
            /* adjust as needed */
        }

        /* Style for the digital clock container */
        .digital-clock {
            background-color: #3c4f60;
            padding: 12px;
            border-radius: 8px;
            font-family: 'Saira', sans-serif;
            font-weight: bold;
            box-shadow: 0 0 5px rgb(11, 11, 35);
            margin: 0 auto;
            margin-top: 40px;
            color: white;
            display: flex;

            letter-spacing: 1px;
            width: 145px;
            /* Slightly wider */
            height: 50px;
            /* Taller to accommodate bigger digits */
            text-align: center;
            /* Center the digits */
            border: 2px solid #d0d0f0;
            justify-content: center;
            /* Horizontally center the digits */
            align-items: center;
            /* Vertically center the digits */
        }

        .digit {
            display: inline-block;
            transition: transform 0.3s cubic-bezier(0.65, 0.05, 0.36, 1), color 0.3s ease-in-out;
            /* Smooth transition for color too */
            font-size: 20px;
            font-weight: bold;
            line-height: normal;
            vertical-align: middle;

        }

        .digit.changed {
            transform: translateY(-3px);

        }

        .content {
            background-color: #e1eaf4;
            /* Replace with your desired color */
            height: 400px;
        }
    </style>


    <!-- Bootstrap CSS -->
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet">

    <!-- jQuery Full Version (not slim) -->
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>

    <!-- Bootstrap JS -->
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>




    <script>
        var staffDonutChart;
        var visitorDonutChart;
        if (typeof staffOut === 'undefined') var staffOut = 0;
        if (typeof staffIn === 'undefined') var staffIn = 0;
        if (typeof visitorIn === 'undefined') var visitorIn = 0;
        if (typeof visitorOut === 'undefined') var visitorOut = 0;

        const centerTextPlugin = {
            id: 'centerText',
            beforeDraw(chart) {
                const {
                    width,
                    height,
                    ctx
                } = chart;
                ctx.save();

                const total = chart.data.datasets[0].data.reduce((a, b) => a + b, 0);

                // First line: "Total:"
                ctx.font = ' 14px Aria';
                ctx.fillStyle = '#333';
                const centerY = height / 2 + 6;
                ctx.textAlign = 'center';
                ctx.textBaseline = 'middle';
                ctx.fillText('Total:', width / 2, height / 2 - 2);
                // Second line: the number
                ctx.font = 'bold 24px Saira';
                ctx.fillText(total, width / 2, height / 2 + 19);

                ctx.restore();
            }
        };
   
        Chart.register(centerTextPlugin);

        document.addEventListener("DOMContentLoaded", function() {
            // Staff donut chart
            var ctx1 = document.getElementById('staffDonutChart').getContext('2d');
            staffDonutChart = new Chart(ctx1, {
                type: 'doughnut',
                data: {
                    labels: ['Out', 'In'],
                    datasets: [{
                        data: [staffOut, staffIn],
                        backgroundColor: ['#56e4e1', '#e456a0'],
                        hoverOffset: 4
                    }]
                },
                options: {
                    responsive: true,
                    cutoutPercentage: 80,
                    plugins: {
                        legend: {
                            position: 'bottom',
                            labels: {
                                color: '#1A1A40',
                                align: 'start',
                                textAlign: 'left',
                                font: {
                                    size: 24,
                                    weight: 'bold'
                                },
                            }
                        }
                    }
                },
                plugins: [centerTextPlugin]
            });

            // Visitor donut chart
            var ctx2 = document.getElementById('visitorDonutChart').getContext('2d');
            visitorDonutChart = new Chart(ctx2, {
                type: 'doughnut',
                data: {
                    labels: ['In', 'Out'],
                    datasets: [{
                        data: [visitorIn, visitorOut],
                        backgroundColor: ['#ffd73e', '#ff5314'],
                        hoverOffset: 4
                    }]
                },
                options: {
                    responsive: true,
                    cutoutPercentage: 80,
                    plugins: {
                        legend: {
                            position: 'bottom',
                            labels: {
                                color: 'white',
                                font: {
                                    size: 24
                                }
                            }
                        }
                    }
                },
                plugins: [centerTextPlugin]
            });
        });


        // Function to update both donut charts
        function updateDonutChart() {
            // Update the staff chart data
            staffDonutChart.data.datasets[0].data = [staffOut, staffIn];
            staffDonutChart.update(); // Refresh the staff chart

            // Update the visitor chart data
            visitorDonutChart.data.datasets[0].data = [visitorIn, visitorOut];
            visitorDonutChart.update(); // Refresh the visitor chart
        }
    </script>



    <script>
        // Function to update the digital clock
        function updateDigitalClock() {
            const now = new Date();
            let hours = String(now.getHours()).padStart(2, '0'); // Ensure 2 digits for hours
            let minutes = String(now.getMinutes()).padStart(2, '0'); // Ensure 2 digits for minutes
            let seconds = String(now.getSeconds()).padStart(2, '0'); // Ensure 2 digits for seconds

            // Update each digit with a transition effect
            updateDigit('hour1', hours[0]);
            updateDigit('hour2', hours[1]);
            updateDigit('min1', minutes[0]);
            updateDigit('min2', minutes[1]);
            updateDigit('sec1', seconds[0]);
            updateDigit('sec2', seconds[1]);
        }

        // Function to update individual digits with transition
        function updateDigit(id, newValue) {
            const digitElement = document.getElementById(id);
            if (digitElement.textContent !== newValue) {
                digitElement.classList.add('changed');
                setTimeout(() => {
                    digitElement.textContent = newValue;
                    digitElement.classList.remove('changed');
                }, 300); // Match the transition duration
            }
        }

        // Update the clock every second
        setInterval(updateDigitalClock, 1000);
        updateDigitalClock(); // Initial call to set the clock on load
    </script>


    <script>
        function updateDate() {
            const today = new Date();

            const days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
            const dayName = days[today.getDay()];

            const day = String(today.getDate()).padStart(2, '0');
            const month = String(today.getMonth() + 1).padStart(2, '0'); // Months start from 0
            const year = today.getFullYear();

            const formattedDate = `${dayName}, ${day}/${month}/${year}`;
            document.getElementById('date').textContent = formattedDate;
        }

        window.onload = updateDate;
    </script>




</asp:Content>