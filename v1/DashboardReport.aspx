<%@ Page Title="Report Dashboard" Language="C#" MasterPageFile="~/v1/Main.Master" AutoEventWireup="true" CodeBehind="DashboardReport.aspx.cs" Inherits="vms.v1.DashboardReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet" />
<link href="https://cdn.jsdelivr.net/npm/bootstrap@4.6.0/dist/css/bootstrap.min.css" rel="stylesheet" />
<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@4.6.0/dist/js/bootstrap.bundle.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/js/bootstrap-datepicker.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
  

   <!-- Summary Cards -->
<div class="row justify-content-center mb-4">
    <div class="col-auto">
     
    </div>

    <div class="col-auto">
        <div class="card summary-card text-center">
            <div class="card-head">
                <strong>Staff Check Status</strong>
            </div>
            <div class="card-body">
                <canvas id="staffDonutChart" width="50" height="50"></canvas>
            </div>
        </div>
    </div>

    <div class="col-auto">
        <div class="card summary-card text-center">
            <div class="card-head">
                <strong>Visitor Check Status</strong>
            </div>
            <div class="card-body">
                <canvas id="visitorDonutChart" width="50" height="50"></canvas>
            </div>
        </div>
    </div>
</div>

<!-- Go to Full Daily Report Button -->
<div class="row">
    <div class="col-md-12">
        <a href="ListReport.aspx" class="btn w-100 text-white" style="background-color: #3c4f60;">
            Go to Full Daily Report &gt;
        </a>
    </div>
</div>

<hr/>

<!-- Dashboard Cards -->
<div class="container">
    <div class="row">
<div class="col-md-6 col-lg-3 mb-4">
    <asp:LinkButton ID="btnAttendance" runat="server" CssClass="summary-box bg-gradient-security" OnClick="btnAttendance_Click">
        <div class="icon"><i class="fas fa-user-shield"></i></div>
        <div class="label">Guard Attendance</div>
        <asp:Label ID="lblAttendanceRate" runat="server" CssClass="value" />
    </asp:LinkButton>
</div>
              <!-- Modal: Attendance -->
        <div class="modal fade" id="attendanceModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Guard Attendance</h5>
                        <button type="button" class="close text-white" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body">
                        <asp:TextBox ID="txtReportDate" runat="server" TextMode="Date" CssClass="form-control mb-3" AutoPostBack="true" OnTextChanged="txtReportDate_TextChanged" />
                        <asp:Literal ID="attendanceList" runat="server" Mode="PassThrough" />
                    </div>
                </div>
            </div>
        </div>
<div class="col-md-6 col-lg-3 mb-4">
    <asp:LinkButton ID="btnClocking" runat="server" CssClass="summary-box bg-gradient-clocking" OnClick="btnClocking_Click">
        <div class="icon"><i class="fas fa-clock"></i></div>
        <div class="label">Clocking</div>
        <asp:Label ID="lblClockingStatus" runat="server" CssClass="value" />
    </asp:LinkButton>
</div>
           <!-- Modal: Clocking -->
        <div class="modal fade" id="clockingModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Clocking Status</h5>
                        <button type="button" class="close text-white" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body">
                        <asp:TextBox ID="txtClockingDate" runat="server" TextMode="Date" CssClass="form-control mb-3" AutoPostBack="true" OnTextChanged="txtReportDate_TextChanged" />
                        <asp:Literal ID="ClockingList" runat="server" Mode="PassThrough" />
                    </div>
                </div>
            </div>
        </div>
<div class="col-md-6 col-lg-3 mb-4">
    <asp:LinkButton ID="btnCCTV" runat="server" CssClass="summary-box bg-gradient-cctv" OnClick="btnCCTV_Click">
        <div class="icon"><i class="fas fa-video"></i></div>
        <div class="label">CCTV</div>
        <div class="value small">
            Online: <asp:Label ID="lblCCTVOnline" runat="server" CssClass="font-weight-bold" /> <br />
            Offline: <asp:Label ID="lblCCTVOffline" runat="server" CssClass="font-weight-bold" />
        </div>
    </asp:LinkButton>
</div>
        
         <!-- Modal: CCTV -->
 <div class="modal fade" id="cctvModal" tabindex="-1">
     <div class="modal-dialog">
         <div class="modal-content">
             <div class="modal-header">
                 <h5 class="modal-title">CCTV OFFLINE</h5>
                 <button type="button" class="close text-white" data-dismiss="modal">&times;</button>
             </div>
             <div class="modal-body">
                 <asp:TextBox ID="txtAbnormDate" runat="server" TextMode="Date" CssClass="form-control mb-3" AutoPostBack="true" OnTextChanged="txtReportDate_TextChanged" />
                 <asp:Literal ID="litCCTVAbnorm" runat="server"></asp:Literal>
             </div>
         </div>
     </div>
 </div>

<div class="col-md-6 col-lg-3 mb-4">
    <asp:LinkButton ID="btnVisitors" runat="server" PostBackUrl="~/v1/VisitorList.aspx" CssClass="summary-box bg-gradient-visitors">
        <div class="icon"><i class="fas fa-users"></i></div>
        <div class="label">Visitors</div>
        <div class="value small">
            Transporter: <asp:Label ID="lblTransporter" runat="server" /><br />
            Container: <asp:Label ID="lblContainer" runat="server" /><br />
            Vehicle: <asp:Label ID="lblVehicleVisitor" runat="server" />
        </div>
    </asp:LinkButton>
</div>

       </div>
        </div>
<hr/>
<!-- Chart Card -->
<div class="card shadow-sm rounded" style="background-color: white;">
    <div class="card-body">
       <div class="d-flex flex-wrap gap-2">
                <asp:DropDownList 
                    ID="ddlChartCategory" 
                    runat="server" 
                    AutoPostBack="true" 
                    CssClass="form-select form-select-sm small-dropdown"
                    OnSelectedIndexChanged="ddlChartCategory_SelectedIndexChanged">
                    <asp:ListItem Text="Visitor" Value="Visitor" />
                    <asp:ListItem Text="CCTV" Value="CCTV" />
                </asp:DropDownList>
          

                <asp:DropDownList 
                    ID="ddlMonthRange" 
                    runat="server" 
                    AutoPostBack="true"
                    CssClass="form-select form-select-sm small-dropdown"
                    OnSelectedIndexChanged="ddlMonthRange_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
       

        <div class="row mt-4">
            <div class="col-12">
                <canvas id="chartCanvas" height="100"></canvas>
            </div>
        </div>
     </div>
</div>




  
    <script>
    var chart; // keep global to update chart

        function renderChart(category, data) {
            console.log(data); 
        const ctx = document.getElementById('chartCanvas').getContext('2d');

        if (chart) chart.destroy(); // Destroy existing chart

        let config;

        if (category === "Visitor") {
            config = {
                type: 'bar',
                data: {
                    labels: data.labels,
                    datasets: [
                        {
                            label: 'Container',
                            backgroundColor: '#ad1660',
                            data: data.container
                        },
                        {
                            label: 'Vehicle Visitor',
                            backgroundColor: '#16acad',
                            data: data.visitor
                        },
                        {
                            label: 'Transporter',
                            backgroundColor: '#ad16ac',
                            data: data.transporter
                        }
                    ]
                },
                options: {
                    responsive: true,
                    title: { display: true, text: ' Visitor Trend' },
                    scales: { y: { beginAtZero: true } }
                }
            };
        } else if (category === "CCTV") {
            config = {
                type: 'bar',
                data: {
                    labels: data.labels,
                    datasets: [
                        {
                            label: 'Online',
                            backgroundColor: '#00b894',
                            data: data.online
                        },
                        {
                            label: 'Offline',
                            backgroundColor: '#d63031',
                            data: data.offline
                        }
                    ]
                },
                options: {
                    responsive: true,
                    title: { display: true, text: ' CCTV Status' },
                    scales: { y: { beginAtZero: true } }
                }
            };
        }

        chart = new Chart(ctx, config);
    }
    </script>
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



          // Register plugin
          Chart.register(centerTextPlugin);

          document.addEventListener("DOMContentLoaded", function () {
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



 

    <style>
        .small-dropdown {
    font-size: 11px !important;
    padding: 2px 6px !important;
    
    max-width: 140px; /* optional: constrain width if too long */
    border-radius: 4px;
}

        .modal-content {
    border-radius: 10px;
}

.modal-body {
    max-height: 400px;
    overflow-y: auto;
}

.modal-body div {
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}
.modal-header{
    
    background-color: #3c4f60;
       color: white;
    
   
}  .modal-title {
            font-size: 1.25rem !important;
            font:'Segoe UI';
        }
.small-dropdown {
       font-size: 14px; 
       padding: 2px 5px; 
       height: auto; 
       width: auto;
        }
#visitorOverstayModal .modal-dialog {
  max-width: 600px; /* or any width you want */
  margin: 1.75rem auto;
}
        .summary-box {
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 20px 15px;
    border-radius: 15px;
    color: white;
    text-align: center;
    transition: all 0.3s ease;
    height: 170px;
    justify-content: center;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.summary-box:hover {
    transform: translateY(-5px);
    box-shadow: 0 6px 20px rgba(0, 0, 0, 0.25);
    text-decoration: none;
}

.summary-box .icon {
    font-size: 20px;
    margin-bottom: 4px;
}

.summary-box .label {
    font-size: 12px;
    font-weight: bold;
    margin-bottom: 6px;
}

.summary-box .value {
    font-size: 12px;
    font-weight: bold;
}

.bg-gradient-security {
    background: linear-gradient(135deg, #005f73, #0a9396);
}

.bg-gradient-clocking {
    background: linear-gradient(135deg, #0a9396, #94d2bd);
}

.bg-gradient-cctv {
    background: linear-gradient(135deg, #f7b886, #e76f51);
}

.bg-gradient-visitors {
    background: linear-gradient(135deg, #e76f51, #a4501c);
}

    .summary-card
    {
            display: flex;
            flex-direction: column;
            height: 100%;

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

       

      
    </style>


    <!-- Bootstrap CSS -->
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet">

    <!-- jQuery Full Version (not slim) -->
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>

    <!-- Bootstrap JS -->
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>


</asp:Content>


