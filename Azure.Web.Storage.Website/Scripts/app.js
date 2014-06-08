angular.module('app', ['ng', 'ngRoute', 'ngCookies', 'ui.bootstrap.datetimepicker',
    'googlechart', "checklist-model", "ajaxSpinner", "tablePager", "xeditable"])
    .config([
        '$routeProvider',
        function ($routeProvider) {
            $routeProvider.
                when('/Tables', {
                    templateUrl: 'templates/tables.html',
                    controller: 'TablesController',
                    resolve: { tableData: function () { return { isSystem: false } } }
                }).
                when('/SystemTables', {
                    templateUrl: 'templates/tables.html',
                    controller: 'TablesController',
                    resolve: { tableData: function () { return { isSystem: true } } }
                }).
                when('/Tables/:tableId', {
                    templateUrl: 'templates/table.html',
                    controller: 'TableController',
                    resolve: {
                        tableData: ['storageService', '$route', function (storageService, $route) {
                            return storageService.getTableProperties($route.current.params.tableId);
                        }]
                    }
                }).
                when('/Deployments', {
                    templateUrl: 'templates/deployments.html',
                    controller: 'DeploymentsController',
                    resolve: { routeData: function () { return { link: 'Deployments' } } }
                }).
                when('/Deployments/:id/:deployId', {
                    templateUrl: 'templates/deployment.html',
                    controller: 'DeploymentController'
                }).
                when('/PerfCounters', {
                    templateUrl: 'templates/deployments.html',
                    controller: 'DeploymentsController',
                    resolve: { routeData: function () { return { link: 'PerfCounters' } } }
                }).
                when('/PerfCounters/:id/:deployId', {
                    templateUrl: 'templates/perfcounter.html',
                    controller: 'PerfCounterController'
                }).
                otherwise({
                    templateUrl: 'templates/index.html',
                    controller: 'IndexController'
                });
        }
    ])
    .controller('IndexController', ['$scope', function ($scope) {
    }]);

