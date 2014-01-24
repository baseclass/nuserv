angular.module('repositoryApp', ['ngRoute', 'repositoryApp.controllers', function () { }])//Configure the routes
.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/repository', {
        templateUrl: '/app/repository/repositories.html',
        controller: 'RepositoriesController'
    });

    $routeProvider.otherwise({ redirectTo: '/repository' });
}]);