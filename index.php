<?php
ini_set('display_errors', 'Off'); // теперь сообщений НЕ будет
header('Content-type: text/html; charset=cp-1251');
session_start(); 
?>

<!DOCTYPE html>
<html>
<script src="https://ajax.googleapis.com/ajax/libs/angularjs/1.4.8/angular.min.js"></script>

<body>
<head>
    <link rel="stylesheet" type="text/css" href="css/bootstrap.min.css"/>
    <link rel="stylesheet" type="text/css" href="css/taskman.css"/>
    <link href="http://fonts.googleapis.com/css?family=Open+Sans:400,600,300,700" rel="stylesheet" type="text/css">
</head>

<a href='?logout=1'>Выход</a>

<?php

if($_POST[login]=="admin" && $_POST[pass]=="1234")
{
 $_SESSION[login]=$_POST[login];
}


if($_GET[logout]=="1")
{
$_SESSION[login]="";
}

if($_SESSION[login]!="")
{
 include ("main.php");
}
else
{
 include ("login.php");
}

?>

<script src="angular-material.min.js"></script>
</body>
</html>
</body>
</html>
