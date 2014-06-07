
angular.module('app')
    .factory('deploymentsService', [
        '$http',
        function($http) {
            return {
                getDeployments: function() {
                    return $http.get('/api/deployments/get/');
                },
                getDeployment: function(id, deployId) {
                    return $http.get('/api/deployments/get/' + id + '?deployId=' + deployId);
                },
                updateDeployment: function (id, deployId, configuration, instance) {
                    return $http.put('/api/deployments/put/'+ id + '?deployId=' + deployId, {
                        id: instance,
                        configuration: configuration
                    });
                },
                updateAllDeployments: function (id, deployId, configuration, instance) {
                    return $http.put('/api/deployments/putall/' + id + '?deployId=' + deployId, {
                        id: instance,
                        configuration: configuration
                    });
                }
            };
        }
    ])
    .controller('DeploymentsController', function ($scope, $location, deploymentsService, routeData, $cookies) {
        deploymentsService.getDeployments().success(function(data) {
            $scope.deployments = data;
            $.each(data, function (index,  x) {
                x.deploymentId = $cookies[x.id + "_deployment"];
            });
        });

        $scope.goToDeployment = function (deploy) {
            if (!deploy.deploymentId || deploy.deploymentId == '') {
                return;
            }
            $cookies[deploy.id + "_deployment"] = deploy.deploymentId;
            $location.path('/'+ routeData.link + '/' + deploy.id + '/' + deploy.deploymentId);
        };

    })
    .controller('DeploymentController', function ($scope, $routeParams, deploymentsService) {
        $scope.id = $routeParams.id;
        $scope.deployId = $routeParams.deployId;
        $scope.selectedCounters = [];
        $scope.update = function() {
            deploymentsService.getDeployment($scope.id, $scope.deployId).success(function(data) {
                $scope.deployment = data;
            });
        };
        $scope.update();
        $scope.timePattern = new RegExp('([0-1][0-9]|2[0-3]):([0-5][0-9]):([0-5][0-9])');
        $scope.numberPattern = new RegExp('^[0-9]+$');
        $scope.viewCurrentRow = function (row) {
            $scope.currentRow = {
                id: row.id,
                data: row.configuration
            }
        };
        $scope.addCounterToCurrent = function() {
            $scope.currentRow.data.performanceCounters.dataSources.push({
                counterSpecifier: '',
                sampleRate: '00:00:30'
            });
        };

        $scope.removeCounterFromCurrent = function (index) {
            $scope.currentRow.data.performanceCounters.dataSources.splice(index, 1);
        };
        
        $scope.updateInstance = function () {
            deploymentsService.updateDeployment($scope.id, $scope.deployId, $scope.currentRow.data, $scope.currentRow.id).success(function (data) {
                $scope.update();
            });
        }
        $scope.updateAllInstances = function () {
            deploymentsService.updateAllDeployments($scope.id, $scope.deployId, $scope.currentRow.data, $scope.currentRow.id).success(function (data) {
                $scope.update();
            });
        }
    })
    .run(function (editableOptions) {
        editableOptions.theme = 'bs3';
    });;