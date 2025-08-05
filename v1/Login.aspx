<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="vms.v1.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>SMS | Login</title>
    <link rel="icon" type="image/x-icon" href="Theme/dist/img/favicon_ios.png" />

    <!-- Google Font: Source Sans Pro -->
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;600;700&display=swap" rel="stylesheet" />
    <!-- Font Awesome -->
    <link rel="stylesheet" href="../Theme/plugins/fontawesome-free/css/all.min.css" />
    <!-- Bootstrap + AdminLTE -->
    <link rel="stylesheet" href="../Theme/plugins/icheck-bootstrap/icheck-bootstrap.min.css" />
    <link rel="stylesheet" href="../Theme/dist/css/adminlte.min.css" />

    <style>
        body {
            background-color: #f4f6f9;
            font-family: 'Inter', sans-serif;
        }

        .login-box {
            max-width: 420px;
            margin: 6% auto;
        }

        .card {
            border-radius: 1rem;
            box-shadow: 0 8px 20px rgba(0, 0, 0, 0.1);
        }

        .card-header {
            background-color: #1A1A40;
            color: white;
            border-top-left-radius: 1rem;
            border-top-right-radius: 1rem;
        }

       

        .form-control:focus {
            border-color: #1A1A40;
            box-shadow: 0 0 6px rgba(26, 26, 64, 0.4);
        }

        .btn-primary {
            background-color: #1A1A40;
            border-color: #1A1A40;
        }

        .btn-primary:hover {
            background-color: #2a2a5c;
            border-color: #2a2a5c;
        }

        .btn-warning:hover {
            background-color: #f7b500;
            border-color: #f7b500;
        }

        .login-logo {
            font-size: 1.5rem;
            font-weight: 600;
            margin-bottom: 0;
            color: #1A1A40;
        }

        .login-box-msg {
            font-size: 1rem;
            color: #555;
        }

        .input-group-text {
            background-color: #f1f1f1;
        }
    </style>
</head>
<body class="hold-transition login-page">
    <form id="form1" runat="server">
        <div class="login-box">
            <div class="login-logo">
                SMS <br />
                <small>(Security Management System)</small>
            </div>

            <div class="card card-outline">
            
                <div class="card-body">
                    <p class="login-box-msg">Sign in to start your session</p>

                    <div class="input-group mb-3">
                        <asp:TextBox CssClass="form-control" ID="txtUsername" placeholder="Username" runat="server" autofocus="autofocus"></asp:TextBox>
                        <div class="input-group-append">
                            <div class="input-group-text"><span class="fas fa-user"></span></div>
                        </div>
                    </div>

                    <div class="input-group mb-3">
                        <asp:TextBox CssClass="form-control" ID="txtPassword" TextMode="Password" placeholder="Password" runat="server"></asp:TextBox>
                        <div class="input-group-append">
                            <div class="input-group-text"><span class="fas fa-lock"></span></div>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-6">
                            <asp:Button CssClass="btn btn-primary btn-block" ID="btnLogin" runat="server" Text="Sign In" OnClick="btnLogin_Click" />
                        </div>
                        <div class="col-6">
                            <asp:Button CssClass="btn btn-warning btn-block" ID="btnReset" runat="server" Text="Reset" OnClick="btnReset_Click" />
                        </div>
                    </div>

                    <div class="text-center text-danger">
                        <asp:Label ID="lblErrorLogin" runat="server" Text=""></asp:Label>
                    </div>
                </div>
            </div>
        </div>

        <!-- Scripts -->
        <script src="../Theme/plugins/jquery/jquery.min.js"></script>
        <script src="../Theme/plugins/bootstrap/js/bootstrap.bundle.min.js"></script>
        <script src="../Theme/dist/js/adminlte.min.js"></script>
    </form>
</body>
</html>
