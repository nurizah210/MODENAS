<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SessionExpired.aspx.cs" Inherits="vms.v1.SessionExpired" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Session Expired Page</title> 
    <link href="../Theme/Style.css" rel="stylesheet" />
    <script src="../Scripts/jquery-3.4.1.min.js"></script>
    <script type="text/javascript" language="javascript">
        //To disable and enable parent window when child window is being opened
        window.onload = function () {
            window.opener.document.body.disabled = true;
        }
        window.onunload = function () {
            window.opener.document.body.disabled = false;
        }
        function fExitSession() {
            if (window.opener != null) {
                if (window.opener.parent_disable() == false) {
                    window.opener.fExitSession();
                    //window.close();
                }
            } else {
                window.location.href = "pgLogin.aspx";
            }
        }
        window.onunload = function () {
            fExitSession();
        }
        window.setTimeout("fExitSession()", 6000);

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table border="0" cellpadding="0" cellspacing="0" align="center" width="450">
                <tr>
                    <td>
                        <table border="0" cellpadding="0" cellspacing="0" width="95%" align="left" class="">
                            <tr class="tblText11BgWhite" valign="bottom">
                                <td class="txt4"><font color="red">Your session has expired!<hr /></font></td>
                            </tr>
                            <tr class="tblText11BgWhite" valign="top">
                                <td class="txt4">Please login again...</td>
                            </tr>
                            <tr>
                                <td>You will be redirect to login page in <span id="time">5</span> seconds!</td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table border="0" cellpadding="0" cellspacing="0" width="95%" align="left">
                            <tr>
                                <td>
                                    <img src="0.gif" height="3" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <input id="btnClose" type="button" class="button" style="font-weight: bold;" value="Close" onclick="Javascript: fExitSession();" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
<script type="text/javascript">
    function startTimer(duration, display) {
        var timer = duration, minutes, seconds;
        setInterval(function () {
            // minutes = parseInt(timer / 60, 10)
            seconds = parseInt(timer % 5, 10);

            // minutes = minutes < 10 ? "0" + minutes : minutes;
            seconds = seconds < 10 ? "0" + seconds : seconds;

            display.text(seconds);

            if (--timer < 0) {
                timer = duration;
            }
        }, 1000);
    }

    jQuery(function ($) {
        var fiveMinutes = 60 * 5,
            display = $('#time');
        startTimer(fiveMinutes, display);
    });
</script>
</html>
