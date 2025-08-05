<%@ Page Title="Post2" Language="C#" MasterPageFile="~/v1/Main.Master" AutoEventWireup="true" CodeBehind="Post2.aspx.cs" Inherits="vms.v1.Post2" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Saira&display=swap" rel="stylesheet">
      
   <div class=" mb-4">
    <div class="col-md-12">
        <a href="RegisterForm2.aspx" class="btn w-100 text-white" style="background-color: #3c4f60; ">
            Register Visitor &gt;
        </a>
    </div>
</div>


    

    <!-- Main Content Section -->
    <section class="content">
        <div class="container-fluid">


            <div class="tab-content">

                       <!-- Post 2 Tab -->
        <div id="post2" class="tab-pane show active fade p-3">

            <!-- Post 2 Sub-Tabs -->
            <ul class="nav nav-pills mb-2">
                <li class="nav-item"><a class="nav-link active" style="font-size: 13px; font-weight: bold;" data-toggle="pill" href="#vehicleVisitor2">VEHICLE VISITOR</a></li>
                <li class="nav-item"><a class="nav-link" style="font-size: 13px; font-weight: bold;" data-toggle="pill" href="#walkin">WALK-IN VISITOR</a></li>
                <li class="nav-item"><a class="nav-link" style="font-size: 13px; font-weight: bold;" data-toggle="pill" href="#itemDeclaration">ITEM DECLARATION</a></li>
                <li class="nav-item"><a class="nav-link" style="font-size: 13px; font-weight: bold;" data-toggle="pill" href="#container">CONTAINER</a></li>
            </ul>

            <div class="tab-content">
                <div id="vehicleVisitor2" class="tab-pane fade show active">
                    <div class="table-responsive" style="max-height: 300px; overflow-y: auto;">
                        <asp:GridView ID="gvVehicle2" runat="server" AutoGenerateColumns="False" CssClass="small-header-grid table table-bordered table-sm" style="table-layout: fixed;">
                            <Columns>
                                <asp:BoundField DataField="PLATE_NO" HeaderText="PLATE NO.">
                                    <ItemStyle Font-Size="smaller" />
                                </asp:BoundField>
                                <asp:BoundField DataField="NAME" HeaderText="NAME">
                                    <HeaderStyle Font-Size="smaller" Width="250px" />
                                    <ItemStyle Font-Size="smaller" Width="250px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="BLOCK" HeaderText="BLOCK">
                                    <ItemStyle Font-Size="smaller" />
                                </asp:BoundField>
                                <asp:BoundField DataField="REGISTER_DATE" HeaderText="TIME IN" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                                    <ItemStyle Font-Size="smaller" />
                                </asp:BoundField>

                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Button ID="btnCheckOut5" runat="server" Text="Check Out" CommandName="CheckOut" CommandArgument='<%# Eval("PLATE_NO") %>' OnClick="btnCheckOut5_Click" OnClientClick="return confirm('Are you sure you want to checkout this visitor?')" CssClass="btn btn-sm" style="background-color: #1A1A40; color: white;" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
                <div id="walkin" class="tab-pane fade">
                    <div class="table-responsive" style="max-height: 300px; overflow-y: auto;">
                        <asp:GridView ID="gvWalkin" runat="server" AutoGenerateColumns="False" CssClass="small-header-grid table table-bordered table-sm" style="table-layout: fixed;">
                            <Columns>
                                <asp:BoundField DataField="NAME" HeaderText="NAME">
                                    <HeaderStyle Font-Size="smaller" Width="250px" />
                                    <ItemStyle Font-Size="smaller" Width="250px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="IC_NO" HeaderText="IC NO.">
                                    <HeaderStyle Font-Size="smaller" />
                                    <ItemStyle Font-Size="smaller" />
                                </asp:BoundField>
                                <asp:BoundField DataField="BLOCK" HeaderText="BLOCK">
                                    <ItemStyle Font-Size="smaller" />
                                </asp:BoundField>
                                <asp:BoundField DataField="REGISTER_DATE" HeaderText="TIME IN" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                                    <ItemStyle Font-Size="smaller" />
                                </asp:BoundField>

                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Button ID="btnCheckOut6" runat="server" Text="Check Out" CommandName="CheckOut" CommandArgument='<%# Eval("NAME") %>' OnClick="btnCheckOut6_Click" OnClientClick="return confirm('Are you sure you want to checkout this visitor?')" CssClass="btn btn-sm" style="background-color: #1A1A40; color: white;" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
                <div id="itemDeclaration" class="tab-pane fade">
                    <div class="table-responsive" style="max-height: 300px; overflow-y: auto;">
                        <asp:GridView ID="gvItemDeclare" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-sm">
                            <Columns>
                                <asp:BoundField DataField="NAME" HeaderText="NAME">
                                    <HeaderStyle Font-Size="Smaller" Width="150px" />
                                    <ItemStyle Font-Size="Smaller" Width="150px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="DECLARE_DATE" HeaderText="TIME IN" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                                    <HeaderStyle Font-Size="Smaller" />
                                    <ItemStyle Font-Size="Smaller" />
                                </asp:BoundField>
                                <asp:BoundField DataField="SERIAL_PART_NO" HeaderText="SERIAL NO">
                                    <HeaderStyle Font-Size="Smaller" />
                                    <ItemStyle Font-Size="Smaller" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ITEM_TYPE" HeaderText="ITEM">
                                    <HeaderStyle Font-Size="Smaller" />
                                    <ItemStyle Font-Size="Smaller" />
                                </asp:BoundField>
                                <asp:BoundField DataField="TOTAL_ITEM_IN" HeaderText="TOTAL IN">
                                    <HeaderStyle Font-Size="Smaller" Width="80px" />
                                    <ItemStyle Font-Size="Smaller" Width="80px" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="TOTAL OUT">
                                    <HeaderStyle Font-Size="Smaller" Width="80px" />
                                    <ItemStyle Font-Size="Smaller" Width="80px" />
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtTotalItemOut" runat="server" CssClass="form-control form-control-sm" Placeholder="Enter Total" TextMode="Number" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Button ID="btnCheckOut7" runat="server" Text="Check Out" CommandName="CheckOut" CommandArgument='<%# Eval("NAME") %>' OnClick="btnCheckOut7_Click" OnClientClick="return confirm('Are you sure you want to checkout this visitor?')" CssClass="btn btn-sm" Style="background-color: #1A1A40; color: white;" />
                                    </ItemTemplate>
                                    <HeaderStyle Font-Size="Smaller" Width="100px" />
                                    <ItemStyle Font-Size="Smaller" Width="100px" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
                <div id="container" class="tab-pane fade active">
                    <div class="table-responsive" style="max-height: 300px; overflow-y: auto;">
                        <asp:GridView ID="gvContainer" runat="server" AutoGenerateColumns="False" CssClass="small-header-grid table table-bordered table-sm" Style="table-layout: fixed;">
                            <Columns>
                                <asp:BoundField DataField="PLATE_NO" HeaderText="PLATE NO.">
                                    <ItemStyle Font-Size="Smaller" />
                                </asp:BoundField>

                                <asp:BoundField DataField="DRIVER_NAME" HeaderText="NAME">
                                    <HeaderStyle Font-Size="Smaller" Width="250px" />
                                    <ItemStyle Font-Size="Smaller" Width="250px" />
                                </asp:BoundField>

                                <asp:BoundField DataField="REGISTER_DATE" HeaderText="TIME IN" DataFormatString="{0:dd/MM/yyyy HH:mm}">
                                    <ItemStyle Font-Size="Smaller" />
                                </asp:BoundField>

                                <asp:TemplateField HeaderText="CONTAINER NO.">
                                    <ItemTemplate>
                                        <asp:Label ID="lblContainerNo" runat="server" Text='<%# Eval("CONTAINER_NO") %>' Visible='<%# !String.IsNullOrEmpty(Eval("CONTAINER_NO").ToString()) %>'></asp:Label>
                                        <asp:TextBox ID="txtContainerNo" runat="server" CssClass="form-control form-control-sm" Visible='<%# String.IsNullOrEmpty(Eval("CONTAINER_NO").ToString()) %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="SEAL NO.">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSealNo" runat="server" Text='<%# Eval("SEAL_NO") %>' Visible='<%# !String.IsNullOrEmpty(Eval("SEAL_NO").ToString()) %>'></asp:Label>
                                        <asp:TextBox ID="txtSealNo" runat="server" CssClass="form-control form-control-sm" Visible='<%# String.IsNullOrEmpty(Eval("SEAL_NO").ToString()) %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Button ID="btnCheckOut8" runat="server" Text="Check Out" CommandName="CheckOut" CommandArgument='<%# Eval("PLATE_NO") %>' OnClick="btnCheckOut8_Click" OnClientClick="return confirm('Are you sure you want to checkout this visitor?')" CssClass="btn btn-sm" Style="background-color: #1A1A40; color: white;" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
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




    


</asp:Content>