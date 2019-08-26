angular.module("umbraco").controller("ETC.Connector.SettingsController", ['$scope', '$http', 'notificationsService', 'dialogService', function ($scope, $http, notificationsService, dialogService) {


	$scope.Data = {};
	$scope.contentForm.$dirty = false;

	$scope.logitems = {};
	$scope.options = {
		includeProperties: [
			{ alias: "productId", header: "Product Guid" },			
			{ alias: "message", header: "Message" }
		]
	};

	$http.get('backoffice/ETCConnector/Spider/GetGlobalSettings/',
		{
			cache: false
		}).then(function (response) {
			$scope.Data = response.data;
		}, function (err) {
			notificationsService.error("oops :" + err.data.ExceptionMessage, err.ExceptionMessage);
		});

	$http.get('backoffice/ETCConnector/Spider/GetLogItems/',
		{
			cache: false
		}).then(function (response) {
			$scope.logitems = response.data;
		}, function (err) {
			notificationsService.error("oops :" + err.data.ExceptionMessage, err.ExceptionMessage);
		});


	$scope.openDialog = function (item) {

		var dialog = dialogService.open({ template: '/app_plugins/etcconnector/backoffice/etcconnector/validationdialog.html', show: true, item: item, callback: done });

		function done(data){
			//The dialog has been submitted 
			//data contains whatever the dialog has selected / attached
		}   
	};

	$scope.Commit = function () {
		$http.post('backoffice/ETCConnector/Spider/SaveGlobalSettings/', $scope.Data).then(function (res) {
			
			$scope.Data = res.data;
			$scope.contentForm.$dirty = false;
			notificationsService.success("Document Published", "Gelukt, de wijzigingen zijn succesvol opgelagen");

		}, function (err) {
			notificationsService.error("Niet opgeslagen", err);
		});
		return false;

	};
	
}]);

angular.module("umbraco").controller("ETC.Connector.ValiationDialogController", ['$scope', '$http', 'notificationsService', 'dialogService',
	function ($scope, $http, notificationsService, dialogService) {

		var vm = this;

		vm.item = $scope.dialogOptions.item;
		vm.messages = vm.item.message.split(',');

		vm.close = function () {
			this.close();
		};

	}]);


