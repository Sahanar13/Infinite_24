<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="question3.aspx.cs" Inherits="Test__1.question3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Button Click Event</title>
</head>
<body>

<button id="myButton">Click Me</button>

<script>
    function handleClick() {
        alert("Button clicked!");
    }

    document.addEventListener("DOMContentLoaded", function () {
        var button = document.getElementById("myButton");
        button.addEventListener("click", handleClick);
    });

    var clickCount = 0;

    function addButton() {
        var button = document.createElement("button");
        button.innerHTML = "Click me Here!!";
        button.addEventListener("click", function () {
            clickCount++;
            console.log("Button clicked! Total clicks: " + clickCount);
        });
        document.body.appendChild(button);
    }
</script>

<form id="form1" runat="server">
    <h2>Button Event</h2>
    <button type="button" onclick="addButton()">Add Button</button>
</form>

</body>
</html>