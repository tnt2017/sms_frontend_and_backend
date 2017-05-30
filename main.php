<div ng-app="myApp" ng-controller="customersCtrl" ng-init="tovar='';">
<blockquote>
<h2>SMS front-end</h2>

<h3>Модемы</h3>
<table border=1>
	<tr><td>ID</td><td>COM-port</td><td>Название устройства</td><td>IMEI</td><td>IMSI</td><td>Model</td><td>Revision</td><td>X</td></tr>
	  <tr ng-repeat="x in modems1"  ng-click="alert('1')">
	    <td><span ng-click="span_click_ostatki1($event)" id="{{ x.id }}" >{{ x.id }}</span></td>
	    <td><span ng-click="span_click_ostatki1($event)" id="{{ x.comport }}" >{{ x.comport }}</span></td>
	    <td><span ng-click="span_click_ostatki1($event)" id="{{ x.comport }}" >{{ x.modem }}</span></td>
	    <td><span ng-click="span_click_ostatki1($event)" id="{{ x.comport }}" >{{ x.imei }} </span></td>
	    <td><span ng-click="span_click_ostatki1($event)" id="{{ x.comport }}" >{{ x.imsi }} </span></td>
	    <td><span ng-click="span_click_ostatki1($event)" id="{{ x.model }}" >{{ x.model }} </span></td>
	    <td><span ng-click="span_click_ostatki1($event)" id="{{ x.comport }}" >{{ x.revision }} </span></td>
 	    <td><a ng-click="deleteTask(task.ID)" class="pull-right"><i class="glyphicon glyphicon-trash"></i></a></td>
	  </tr>
	</table>

<h3>Сопутствующие UI-интерфейсы</h3>
<table border=1>
	<tr><td>ID</td><td>COM-port</td><td>Название устройства</td><td>X</td></tr>
	  <tr ng-repeat="x in modems2"  ng-click="alert('1')">
	    <td><span ng-click="span_click_ostatki2($event)" id="{{ x.id }}" >{{ x.id }}</span></td>
	    <td><span ng-click="span_click_ostatki2($event)" id="{{ x.comport }}" >{{ x.comport }}</span></td>
	    <td><span ng-click="span_click_ostatki2($event)" id="{{ x.comport }}" >{{ x.modem }}</span></td>
 	    <td><a ng-click="deleteTask(task.ID)" class="pull-right"><i class="glyphicon glyphicon-trash"></i></a></td>
	  </tr>
</table>

<input ng-model="test" class="form-control" ng-model="text" placeholder = "Название" />
<form action="index.php" method="POST">

COM-PORT1
	<select name="comport1" ng-model="comport1" ng-change="select_change1()">
	<option value="">---Please select---</option>
	<option ng-repeat="x in modems1" value="{{x.comport}}">{{ x.modem}}</option>
	</select><br>

COM-PORT2
	<select name="comport2" ng-model="comport2" ng-change="select_change2()">
	<option value="">---Please select---</option>
	<option ng-repeat="x in modems2" value="{{x.comport}}">{{ x.modem}}</option>
	</select><br>

<br>
Номер получателя:<br>
<input  ng-model="phone" name="phone" value="+79538000300">
<br>
Текст смс:<br>
<textarea name='textsms'></textarea>

<br>
<button>Отправить</button>
</form>

Вывод консоли:<br>
<textarea name="out" cols="80" rows="10">

<?php

if($_POST['textsms']!="")
{
	$cmd='testsms.exe "'. $_POST['comport1'] . '" "'. $_POST['comport2'] . '" "' . $_POST['phone'] . '" "' . $_POST['textsms'] . '"';
	$out = shell_exec($cmd); //
	echo iconv("CP866", "windows-1251", $out);
}
else
{
	$cmd='testsms.exe "'. $_POST['comport1'] . '" "'. $_POST['comport2'] . '"';
	$out = shell_exec($cmd); //
	echo iconv("CP866", "windows-1251", $out);
}
?>

</textarea>
</div>


<script>
	var app = angular.module('myApp', []);
	app.controller('customersCtrl', function($scope, $http) {
	$scope.comport1="";
	$scope.comport2="";
	$scope.phone="+79538000300";
	
	$http.get("modems1.php")
	.then(function (response) {$scope.modems1 = response.data.records;});

	$http.get("modems2.php")
	.then(function (response) {$scope.modems2 = response.data.records;});

 	$scope.username="<? echo $_SESSION[login]; ?>"

$scope.span_click_ostatki1 = function($event) 
{
	//alert(angular.element($event.target).attr('id'));
      	angular.element($event.target).addClass('badge');
	
	$scope.comport1=angular.element($event.target).attr('id'); 

	var index;
	for (index = 0; index < $scope.modems1.length; ++index) 
	{
	    if($scope.comport1==$scope.modems1[index].comport)
	    {
	    $scope.comport2=$scope.modems2[index].comport;
	    }
	}
}

$scope.span_click_ostatki2 = function($event) 
{
	//alert(angular.element($event.target).attr('id'));
      	angular.element($event.target).addClass('badge');
	
	$scope.comport2=angular.element($event.target).attr('id'); 
	
	var index;
	for (index = 0; index < $scope.modems2.length; ++index) 
	{
	    if($scope.comport2==$scope.modems2[index].comport)
	    {
	    $scope.comport1=$scope.modems1[index].comport;
	    }
	}
}

});


</script>