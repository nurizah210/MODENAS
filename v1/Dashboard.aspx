<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/v1/Main.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="vms.v1.Dashboard" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="dashboard-container">
        <h2><strong> Dashboard</strong></h2>
       <asp:Label ID="lblWelcome" runat="server" CssClass="welcome-message" />


     <!-- Quick Actions Card -->
<div class="card quick-actions-card">
    <p class="card-title">Quick Action</p>
    <div class="quick-actions">
        <asp:LinkButton ID="btnSubmitExit" runat="server" CssClass="btn submit" OnClick="btnSubmitExit_Click">
            <i class="fa-solid fa-arrow-right-from-bracket"></i> Submit Exit Request
        </asp:LinkButton>

        <asp:LinkButton ID="btnApprove" runat="server" CssClass="btn approve" OnClick="btnApprove_Click">
            <i class="fa-solid fa-circle-check"></i> Approve Requests
        </asp:LinkButton>

        <asp:LinkButton ID="btnVisitor" runat="server" CssClass="btn visitor" OnClick="btnVisitor_Click">
            <i class="fa-solid fa-truck"></i> Add Visitor
        </asp:LinkButton>
    </div>
</div>

<div class="dashboard-row">
    <!-- Summary Cards Left -->
    <div class="summary-grid">
      <% if (Session["DEPARTMENT"] != null && Session["DEPARTMENT"].ToString() == "HCD") { %>
    <a href="DashboardReport.aspx" style="text-decoration: none;">
        <div class="summary-card red">
            Today's Visitors<br />
            <strong><asp:Label ID="lblTodayVisitors" runat="server"></asp:Label></strong>
        </div>
    </a>
<% } else { %>
    <div class="summary-card red">
        Today's Visitors<br />
        <strong><asp:Label ID="Label1" runat="server"></asp:Label></strong>
    </div>
<% } %>

        <a href="RequestStatus.aspx" style="text-decoration: none;">
            <div class="summary-card blue">
                Approved Exits<br />
                <strong><asp:Label ID="lblTotalExit" runat="server" /></strong>
            </div>
        </a>
    </div>

    <!-- Schedule Carousel Right -->
    <div class="schedule-carousel-card">
        <p>Upcoming Schedule</p>
        <div class="schedule-carousel-wrapper">
            <div class="schedule-carousel-track" id="scheduleCardTrack">
                <asp:Repeater ID="rptSchedule" runat="server">
                    <ItemTemplate>
                        <div class="schedule-slide-card">
                            <%# Container.DataItem %>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</div>



<div style="background: #ffffff; border: 1px solid #ddd; border-radius: 6px; padding: 10px; margin-top: 20px; font-size: 12px;">
  <strong style="color: #d32f2f; font-size: 13px;">⚠️ Emergency Contacts</strong>
  <table style="width: 100%; border-collapse: collapse; margin-top: 8px; font-size: 12px;">
    <tr>
      <td style="padding: 4px 8px;">Security Post 1</td>
      <td style="padding: 4px 8px;"><a href="tel:044668074" style="color: #1565c0; text-decoration: none;">+604 466 8074</a></td>
    </tr>
    <tr>
      <td style="padding: 4px 8px;">Security Post 2</td>
      <td style="padding: 4px 8px;"><a href="tel:044668073" style="color: #1565c0; text-decoration: none;">+604 466 8073</a></td>
    </tr>
    <tr>
      <td style="padding: 4px 8px;">HR</td>
      <td style="padding: 4px 8px;">
        <a href="tel:044668108" style="color: #1565c0; text-decoration: none;">+604 466 8108</a> /
        <a href="tel:044668130" style="color: #1565c0; text-decoration: none;">+604 466 8130</a>
      </td>
    </tr>
  </table>
</div>


    </div>
   <script>
       window.onload = function () {
           let currentIndex = 0;
           const items = document.querySelectorAll('.schedule-slide-card');
           const track = document.getElementById('scheduleCardTrack');
           const wrapper = document.querySelector('.schedule-carousel-wrapper');
           let intervalId;

           function startSlider() {
               intervalId = setInterval(() => {
                   currentIndex = (currentIndex + 1) % items.length;
                   track.style.transform = `translateX(-${currentIndex * 100}%)`;
               }, 3000);
           }

           function stopSlider() {
               clearInterval(intervalId);
           }

           if (items.length > 1) {
               startSlider();

               wrapper.addEventListener('mouseenter', stopSlider);
               wrapper.addEventListener('mouseleave', startSlider);
           }
       };
   </script>



    <style>

       
     .schedule-carousel-wrapper {
    position: relative;
    overflow: hidden;
    width: 100%;
    height: 80px;
    background: none;
    margin-top: 15px;
}

.schedule-carousel-track {
    display: flex;
    transition: transform 0.5s ease-in-out;
    width: 100%; /* Fix width */
}


.schedule-slide-card {
    min-width: 100%;
    padding: 14px;
    margin: 0 5px;
    background: white;
    border-radius: 10px;
    box-shadow: 0 2px 6px rgba(0,0,0,0.1);
    font-size: 14px;
    font-weight: 500;
    color: #333;
    display: flex;
    align-items: center;
    justify-content: center;
}
/* Right panel takes 40% */
.right-panel {
  flex: 0 1 38%;
}

/* 2x2 Summary Grid */
.summary-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 15px;
    flex: 1 1 60%;
    min-width: 300px;
}
.summary-card {
  padding: 20px;
  border-radius: 10px;
  color: white;
  font-size: 14px;
  text-align: center;
  height: 110px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.summary-card strong {
  font-size: 22px;
  margin-top: 5px;
  display: block;
}
.dashboard-container {
        font-family: 'Segoe UI', sans-serif;
        padding: 20px;
        background-color: #f4f7fd;
    }
   
.dashboard-row {
    display: flex;
    gap: 20px;
    flex-wrap: wrap; /* responsive */
    margin-top: 20px;
}
 .welcome-message {
        font-size: 14px;
          margin-bottom: 20px;
        display: block;
        color: #333;
    }


  .card {
    background-color: white;
    padding: 20px;
    border-radius: 12px;
    box-shadow: 0 2px 10px rgba(0,0,0,0.08);
    margin-bottom: 20px;
}

/* Optional title styling */
.card-title {
    font-size: 14px;
    margin-bottom: 15px;
}

/* Button layout */
.quick-actions {
    display: flex;
    flex-wrap: wrap;
    gap: 10px;
}

.quick-actions .btn {
    display: inline-flex;
    align-items: center;
    gap: 8px;
    padding: 10px 16px;
    background-color: #6c63ff;
    color: white;
    text-decoration: none;
    border-radius: 6px;
    border: none;
    font-size: 14px;
    cursor: pointer;
}

/* Optional variations */
.quick-actions .btn.submit {
    background-color: #3cb51a;
}

.quick-actions .btn.approve {
    background-color: #2577c9;
}

.quick-actions .btn.visitor {
    background-color: #552baa;
}

  


    .summary-card.red { background-color: #dacef3; 
                        color:#552baa;
    }
    .summary-card.blue { background-color: #d0e3f6;
                         color: #2577c9;
    }
    

   
    
 .schedule-card p {
    font-size: 12px;
   
}


.schedule-card {
    background: white;
    padding: 15px;
    border-radius: 12px;
    box-shadow: 0 2px 10px rgba(0,0,0,0.08);
    flex: 1 1 35%;
    min-width: 250px;
}


    .status {
        padding: 4px 8px;
        border-radius: 5px;
        font-size: 12px;
    }

    .status.completed { background-color: #00c851; color: white; }
    .status.pending { background-color: #ffbb33; color: white; }
    .status.confirmed { background-color: #33b5e5; color: white; }
    .status.urgent { background-color: #ff4444; color: white; }
</style>

</asp:Content>