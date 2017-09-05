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
                    childScope.repository.Id = childScope.repository.Name.replace(/[^A-Za-z0-9]/g, '-').toLowerCase();
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

            $scope.model.repositories = [];

            $scope.model.repositoryRows = [];

            $scope.$watch('model.repositories', function () {
                var reps = [];

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
                then(function(response) {
                    // this callback will be called asynchronously
                    // when the response is available
                    $scope.model.repositories = response.data;

                    //We are done with AJAX loading
                    $scope.model.isAjaxInProgress = false;
                }).
                catch(function(response) {
                    // called asynchronously if an error occurs
                    // or server returns response with an error status.
                    $scope.model.errorMessage = "Error occurred status:" + response.status;

                    //We are done with AJAX loading
                    $scope.model.isAjaxInProgress = false;
                });

            $scope.save = function (repository) {
                repository.errorName = '';
                repository.errorDescription = '';

                var failed = false;

                if (repository.Name.length < 3) {
                    repository.errorName = "Name is to short";
                    failed = true;
                }

                if (repository.Description.length < 1) {
                    repository.errorDescription = "Description is to short";
                    failed = true;
                }

                if (!failed) {
                    $http({
                        url: '/api/repository',
                        method: "POST",
                        data: repository
                    }).then(function () {
                        repository.isNew = false;

                        var rowCount = $scope.model.repositoryRows.length;

                        var row = $scope.model.repositoryRows[rowCount - 1];

                        if (row.length == 4) {
                            row = [];
                            $scope.model.repositoryRows.push(row);
                        }

                        //Add new repository
                        row.push(repositoryListViewModelFactory.create({ Id: '', Name: '', Description: '', isNew: true }));

                    }).catch(function (response) {
                        repository.errorName = response.data.ExceptionMessage;
                    });
                }
            };
    }
    ]);