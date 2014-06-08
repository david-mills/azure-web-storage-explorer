

angular.module('app')
    .factory('storageService', [
        '$http', '$window',
        function ($http, $window) {
            return {
                getTables: function () {
                    return $http.get('/api/storage/get/', { cache: true });
                },
                getSystemTables: function () {
                    return $http.get('/api/storage/getsystem/', { cache: true });
                },
                getTableProperties: function (tableName) {
                    return $http.get('/api/storage/getproperties/' + tableName, { cache: true });
                },
                getTable: function (tableName, keyType, filter, from, to, token) {
                    var request = '/api/storage/get/' + tableName + '?keytype=' + keyType;
                    if (from && to) {
                        var startDate = new Date(from);
                        var endDate = new Date(to);

                        request = request + '&from=' + startDate.toISOString() + '&to=' + endDate.toISOString();
                    }
                    if (filter) {
                        request = request + '&filter=' + filter;
                    }
                    if (token) {
                        request = request + '&token=' + token;
                    }
                    return $http.get(request, { cache: true });
                },
                exportTable: function (tableName, keyType, filter, from, to) {
                    var request = '/api/storage/export/' + tableName + '?keytype=' + keyType;
                    if (from && to) {
                        var startDate = new Date(from);
                        var endDate = new Date(to);
                        request = request + '&from=' + startDate.toISOString() + '&to=' + endDate.toISOString();
                    }
                    if (filter) {
                        request = request + '&filter=' + filter;
                    }
                    $window.location.assign(request, { cache: true });
                }
            };
        }
    ])
    .controller('TablesController', ['$scope', 'storageService', 'tableData', function ($scope, storageService, tableData) {
        if (tableData.isSystem) {
            storageService.getSystemTables().success(function (data) {
                $scope.tables = data;
            });
        } else {
            storageService.getTables().success(function (data) {
                $scope.tables = data;
            });

        }
    }])
    .controller('TableController', ['$scope', '$routeParams', 'storageService', 'tablePagerNotification', 'tableData', function ($scope, $routeParams, storageService, tablePagerNotification, tableData) {
        $scope.tableName = $routeParams.tableId;
        if (!tableData.data) {
            return;
        }
        $scope.keyType = tableData.data.keyType;
        $scope.filter = "";
        $scope.applyFilter = function () {
            if (($scope.fromDate || $scope.toDate) || $scope.filter) {
                if ($scope.fromDate && !$scope.toDate) {
                    $scope.toDate = new Date();
                }
                if (!$scope.fromDate && $scope.toDate) {
                    $scope.fromDate = new Date($scope.toDate.getTime() - (60 * 60 * 1000));
                }
                $scope.hasFilter = true;
                $scope.update();
            } else {
                $scope.hasFilter = false;
            }
        };
        tablePagerNotification.onNextPage(function (token) {
            $scope.update(token);
        });
        tablePagerNotification.onPrevPage(function (token) {
            $scope.update(token);
        });

        $scope.export = function () {
            storageService.exportTable($scope.tableName, $scope.keyType, $scope.filter, $scope.fromDate, $scope.toDate);
        };
        $scope.update = function (token) {
            if ($scope.hasFilter) {
                storageService.getTable($scope.tableName, $scope.keyType, $scope.filter, $scope.fromDate, $scope.toDate, token).success(function (data) {
                    $scope.allHeadings = [{ value: 'PartitionKey', index: 0 }, { value: 'RowKey', index: 1 }, { value: 'Timestamp', index: 2 }].concat($.map(data.headings, function (a, index) {
                        return { value: a, index: index + 3 };
                    }));

                    if (!$scope.selectedColumnIndex) {
                        $scope.selectedColumnIndex = $.map($scope.allHeadings, function (a) {
                            return a.index;
                        });
                    }
                    $scope.headings = $.grep($scope.allHeadings, function (value, index) {
                        return $scope.selectedColumnIndex.indexOf(index) >= 0;
                    });
                    $scope.entries = $.map(data.storageEntries, function (a) {
                        return {
                            values: $.grep([{ value: a.partitionKey }, { value: a.rowKey }, { value: a.timeStamp }].concat($.map(a.values, function (b) {
                                return { value: b };
                            })), function (value, index) {
                                return $scope.selectedColumnIndex.indexOf(index) >= 0;
                            })

                        };
                    });
                    tablePagerNotification.addToken(data.continuationToken);
                });
            }
        };
        $scope.updateInterval = function () {
            $scope.toDate = new Date();
            $scope.fromDate = new Date($scope.toDate.getTime() - ($scope.interval * 60 * 60 * 1000));;
        };
        $scope.selectAllColumns = function () {
            var indexes = angular.copy($.map($scope.allHeadings, function (a) {
                return a.index;
            }));
            $scope.selectedColumnIndex = indexes;
        }
        $scope.deSelectAllColumns = function () {
            $scope.selectedColumnIndex = [];
        }

        $scope.hasFilter = false;
    }]);

