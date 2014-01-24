angular.module('repositoryApp.controllers', [
        function() {
        }
    ])
    .controller('RepositoryController', [
        '$scope', '$http', function($scope, $http) {
            //We define the model
            $scope.model = {};

            //We define the allMessages array in the model 
            //that will contain all the messages sent so far
            $scope.model.repositories = [];

            $scope.model.repositoryRows = [];

            $scope.$watch('model.repositories', function () {
                var reps = $scope.model.repositories;
                var result = [];
                for (var i = 0; i < reps.length; i += 4) {
                    var row = [];
                    for (var j = 0; j < 4; ++j) {
                        if (reps[i + j]) {
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
        }
    ]);