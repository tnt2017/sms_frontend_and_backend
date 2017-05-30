<?php
header("Access-Control-Allow-Origin: *");

header("Content-Type: application/json; charset=UTF-8");



$out = shell_exec('modemlist.exe ui');
$arr=split("\n", $out);

echo "{\"records\":[";

for($i=0;$i<count($arr)-1;$i++)
{
 $com=split(":", $arr[$i]);
 echo $arr[$i];
 if($i!=count($arr)-2)
 echo ",";
}

echo "]}";