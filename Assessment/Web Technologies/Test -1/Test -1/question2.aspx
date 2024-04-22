<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="question2.aspx.cs" Inherits="Test__1.question2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Odd Even Checker</title>
    <script>
        function checkOddEven() {
            var output = "";
            for (var i = 0; i <= 15; i++) {
                if (i % 2 === 0) {
                    output += i + " is even<br>";
                } else {
                    output += i + " is odd<br>";
                }
            }
            document.getElementById("output").innerHTML = output;
        }
    </script>
</head>
<body>
    <h2>Odd Even Checker</h2>
    <button onclick="checkOddEven()">Check Odd/Even</button>
    <!-- Placeholder to display the output messages -->
    <div id="output"></div>
</body>
</html>
