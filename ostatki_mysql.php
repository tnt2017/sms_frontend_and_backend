<?php


ini_set('display_errors', 'Off');
header("Access-Control-Allow-Origin: *");
header("Content-Type: application/json; charset=UTF-8");

function cmp($a, $b)  
{ 
return strnatcmp($a["Name"], $b["Name"]); 
} 

include("bd.php");

$q="SELECT Sklad.Income, Sklad.Outcome, Tovar.Name, Tovar.Id FROM `Sklad` INNER JOIN `Tovar` ON Sklad.TovarId = Tovar.Id";
$result = $conn->query($q);  

$outp = "";
while($rs = $result->fetch_array(MYSQLI_ASSOC)) 
{
    $arr[$rs["Name"]][Id]=$rs["Id"];

    $arr[$rs["Name"]][Name]=$rs["Name"];
    $arr[$rs["Name"]][income]+=$rs["Income"];
    $arr[$rs["Name"]][outcome]-=$rs["Outcome"];

    $arr[$rs["Name"]][ostatok]+=$rs["Income"];
    $arr[$rs["Name"]][ostatok]-=$rs["Outcome"];
}
$conn->close();

foreach ( $arr as $value )
{
 if($value[ostatok]>0)
 { 
	$arr2[$value[Name]][Id]=$value[Id];
	$arr2[$value[Name]][Name]=$value[Name];
	$arr2[$value[Name]][ostatok]=$value[ostatok];
 }
}

usort($arr2, "cmp"); 

foreach ( $arr2 as $rs )
{
    if($rs["ostatok"]>0) 
    {
    if ($outp != "") {$outp .= ",";}
    $outp .= '{"Name":'  . json_encode($rs["Name"]) . ',';
    $outp .= '"ostatok":"'   . $rs["ostatok"]        . '",';
    $outp .= '"Id":"'   . $rs["Id"]        . '",';
    $outp .= '"Header":'. json_encode ($rs["Header"])  . '}';
    }
}
$outp ='{"records":['.$outp.']}';
echo $outp;

?>