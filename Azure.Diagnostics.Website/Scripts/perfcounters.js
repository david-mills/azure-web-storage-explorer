angular.module('app')
    .factory('perfCounterService', [
        '$http',
        function($http) {
            return {
                getCounters: function(id, deployId, counters, endDate, interval) {
                    var counterQuery = "";
                    var newEndDate = new Date(endDate.getFullYear(), endDate.getMonth(), endDate.getDate(), endDate.getHours(), endDate.getMinutes(), endDate.getSeconds());

                    $.each(counters, function(index, counter) {
                        counterQuery += "&counters=" + counter;

                    });
                    return $http.get('/api/perfcounters/get/' + id + '?deployid=' + deployId + '&' + counterQuery + '&interval=' + (interval != 60 ? "day" : "hour") + '&endDateUtc=' + newEndDate.toISOString(), { cache: true });
                }
            };
        }
    ])
    .controller('PerfCounterController', function($scope, $routeParams, perfCounterService, deploymentsService) {
        $scope.deployId = $routeParams.deployId;
        $scope.id = $routeParams.id;
        $scope.interval = 60;
        $scope.endDate = new Date();
        deploymentsService.getDeployment($scope.id, $scope.deployId).success(function(data) {
            $scope.perfCounters = data.instances[0].configuration.performanceCounters.dataSources;
            $scope.selectedCounters = [];
        });
        $scope.updateCounters = function() {
            if ($scope.selectedCounters.length > 0) {
                perfCounterService.getCounters($scope.id, $scope.deployId, $scope.selectedCounters, $scope.endDate, $scope.interval).success(function(data) {

                    $scope.counters = [];
                    $.each(data, function(index, value) {
                        var values = [];
                        $.each(value.values, function(index, entry) {
                            values.push({ c: [{ v: new Date(entry.ticks) }, { v: entry.value }] });
                        });
                        $scope.counters.push({
                            type: 'LineChart',
                            data:
                            {
                                "cols": [
                                    { id: "time", type: "datetime" },
                                    { id: "values", type: "number" }
                                ],
                                'rows': values
                            },

                            "options": {
                                "title": value.counterName,
                                "isStacked": "false",
                                "displayExactValues": true,
                                "vAxis": {
                                    "gridlines": {
                                        "count": 10
                                    }
                                },
                                "hAxis": {
                                    format: 'dd MM HH:mm'
                                },
                                "legend": 'none',
                            }

                        });

                    });
                });
            }
        };
        $scope.updateInterval = function() {
            $scope.updateCounters();
        };
        $scope.nextClick = function() {
            var current = new Date();
            var newDate = new Date($scope.endDate.getTime() + $scope.interval * 60000);
            if (current > newDate) {
                $scope.endDate = newDate;
                $scope.updateCounters();
            }
        };
        $scope.prevClick = function() {
            $scope.endDate = new Date($scope.endDate.getTime() - $scope.interval * 60000);
            $scope.updateCounters();
        };
        $scope.refresh = function() {
            $scope.endDate = new Date();
            $scope.updateCounters();
        };
    })