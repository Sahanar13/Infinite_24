<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="question1.aspx.cs" Inherits="Test__1.question1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Multiplication and Division Calculator</title>
    <script>
        function calculate() {
            var num1 = parseFloat(document.getElementById("num1").value);
            var num2 = parseFloat(document.getElementById("num2").value);

            var multiplication = num1 * num2;
            var division = num1 / num2;

            document.getElementById("resultMultiply").innerText = "Multiplication Result: " + multiplication;
            document.getElementById("resultDivide").innerText = "Division Result: " + division;
        }
    </script>
</head>
<body>

    <h2>Enter two numbers and click on "Calculate" to see multiplication and division results:</h2>

    <form>
        1st Number: <input type="number" id="num1"><br>
        2nd Number: <input type="number" id="num2"><br><br>
        <input type="button" value="Calculate" onclick="calculate()">
    </form>

    <div id="resultMultiply"></div>
    <div id="resultDivide"></div>

</body>
</html>
