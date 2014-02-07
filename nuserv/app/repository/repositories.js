var app = angular.module('repositoryApp.controllers', [
    function() {
    }
]);

app.factory('repositoryListViewModelFactory', ['$rootScope', function($rootScope) {

    var repository = function (atts) {
            var childScope = $rootScope.$new(true);

            childScope.repository = {};

            var self = childScope.repository;

            var initialSettings = atts || {};
            //initial settings if passed in
            for (var setting in initialSettings) {
                if (initialSettings.hasOwnProperty(setting))
                    self[setting] = initialSettings[setting];
            };

            if (self.isNew) {
                childScope.$watch('repository.Name', function () {
                    childScope.repository.Id = childScope.repository.Name.replace(/[^A-Za-z0-9\/]/g, '-').replace(/\/{2,}/, '/').replace(/^\/+|\/+$/g, '').toLowerCase();
                });
            }

            return self;
        };

    return {
        create: repository
    };
}]);

app.controller('RepositoriesController', [
        '$scope', '$http', 'repositoryListViewModelFactory', function ($scope, $http, repositoryListViewModelFactory) {
            //We define the model
            $scope.model = {};

            //We define the allMessages array in the model 
            //that will contain all the messages sent so far
            $scope.model.repositories = [];

            $scope.model.repositoryRows = [];

            $scope.$watch('model.repositories', function () {
                var reps = []; //$scope.model.repositories.slice(0);

                angular.forEach($scope.model.repositories, function(repository) {
                    var repositoryListViewModel = repositoryListViewModelFactory.create(repository);

                    repositoryListViewModel.isNew = false;

                    reps.push(repositoryListViewModel);
                });

                //Add new repository
                reps.push(repositoryListViewModelFactory.create({ Id: '', Name: '', Description: '', isNew: true }));

                var result = [];
                for (var i = 0; i < reps.length; i += 4) {
                    var row = [];
                    for (var j = 0; j < 4; ++j) {
                        if (reps[i + j]) {
                            // Add isNew property
                            if (typeof reps[i + j].isNew === 'undefined') {
                                reps[i + j].isNew = false;
                            }

                            row.push(reps[i + j]);
                        }
                    }
                    result.push(row);
                }

                $scope.model.repositoryRows = result;
            }, true);

            //The error if any
            $scope.model.errorMessage = undefined;

            //We initially load data so set the isAjaxInProgress = true;
            $scope.model.isAjaxInProgress = true;

            //Load all the messages
            $http({
                    url: '/api/repository',
                    method: "GET"
                }).
                success(function(data, status, headers, config) {
                    // this callback will be called asynchronously
                    // when the response is available
                    $scope.model.repositories = data;

                    //We are done with AJAX loading
                    $scope.model.isAjaxInProgress = false;
                }).
                error(function(data, status, headers, config) {
                    // called asynchronously if an error occurs
                    // or server returns response with an error status.
                    $scope.model.errorMessage = "Error occurred status:" + status;

                    //We are done with AJAX loading
                    $scope.model.isAjaxInProgress = false;
                });

            $scope.save = function (repository) {
                repository.errorName = '';
                repository.errorDescription = '';

                if (repository.Name.length < 3) {
                    repository.errorName = "Name is to short";
                }

                if (repository.Description.length < 1) {
                    repository.errorDescription = "Description is to short";
                }

                $http({
                    url: '/api/repository',
                    method: "POST",
                    data: repository
                }).success(function (data, status, headers, config) {
                    repository.isNew = false;
                }).error(function (data, status, headers, config) {
                    $scope.errorName = status;
                });
                
            };

        $scope.delete = function(repository) {
            $http({
                url: '/api/repository',
                method: "DELETE",
                data: repository.Id
            }).success(function (data, status, headers, config) {
                var index = $scope.model.repositories.indexOf(repository);
                repository.splice(index, 1);
            }).error(function (data, status, headers, config) {
                $scope.errorName = status;
            });
        };
    }
    ]);