angular.module('merchello').controller('Merchello.Directives.EntityStaticCollectionsDirectiveController',
    ['$scope', 'notificationsService', 'dialogService', 'entityCollectionHelper', 'entityCollectionResource', 'dialogDataFactory', 'entityCollectionDisplayBuilder',
    function($scope, notificationsService, dialogService, entityCollectionHelper, entityCollectionResource, dialogDataFactory, entityCollectionDisplayBuilder) {

		$scope.collections = [];

		// exposed methods
		$scope.remove = remove;
		$scope.save = save;
		$scope.openStaticEntityCollectionPicker = openStaticEntityCollectionPicker;
		$scope.blur = blur;

        function init() {
            $scope.$watch('preValuesLoaded', function(pvl) {
                if (pvl) {
                    loadCollections();
                }
            });
        }

        function loadCollections() {
            entityCollectionResource.getEntityCollectionsByEntity($scope.entity, $scope.entityType).then(function(collections) {
                $scope.collections = entityCollectionDisplayBuilder.transform(collections);
            }, function(reason) {
              notificationsService.error('Failed to get entity collections for entity ' + reason);
            });
        }

        function openStaticEntityCollectionPicker() {
            var dialogData = dialogDataFactory.createAddEditEntityStaticCollectionDialog();
            dialogData.entityType = $scope.entityType.toLocaleLowerCase();
            dialogData.collectionKeys = [];

            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/pick.staticcollection.html',
                show: true,
                callback: processAddEditStaticCollection,
                dialogData: dialogData
            });
        }

        function processAddEditStaticCollection(dialogData) {
            var key = $scope.entity.key;
            entityCollectionResource.addEntityToCollections(key, dialogData.collectionKeys).then(function() {
                loadCollections();
            }, function(reason) {
                notificationsService.error('Failed to add entity to collections ' + reason);
            });
        }

        function remove(collection) {
            entityCollectionResource.removeEntityFromCollection($scope.entity.key, collection.key).then(function() {
                loadCollections();
            });
		}

		// blur is raised in the directive
		function blur(scope) {
			if (scope.collection && scope.collection.changed) {
				save(scope.collection);
				scope.collection.changed = false;
			}
		}

		function save(collection) {
			entityCollectionResource.saveEntityFromCollection($scope.entity.key, collection.key, collection.listSortOrder).then(function () {
				loadCollections();
			});
		}

        // initialize the controller
        init();
}]);
