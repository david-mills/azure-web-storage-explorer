
'use strict';

// Declare app level module
var app = angular.module('ajaxSpinner', []);

app.config(['$httpProvider', function ($httpProvider) {
    var interceptor = ['$injector', function ($injector) {
        return $injector.get('ajaxNotificationInterceptor');
    }];
    $httpProvider.interceptors.push(interceptor);
}]);

app.factory('ajaxNotificationInterceptor', [
    'ajaxNotification',
    '$injector',
    '$q', function (ajaxNotification, $injector, $q) {
        var _http = null;
        var _requestEnded = function () {
            _http = _http || $injector.get('$http');
            if (_http.pendingRequests.length < 1) {
                ajaxNotification.requestEnded();
            }
        };
        return {
            request: function (config) {
                if (config.url.indexOf('templates') < 0) {
                    ajaxNotification.requestStarted();
                }
                return config;
            },

            response: function (response) {
                if (response.config.url.indexOf('templates') < 0) {
                    _requestEnded();
                }
                return response;
            },

            responseError: function (reason) {
                _requestEnded();
                return $q.reject(reason);
            }
        }
    }]
);

app.factory('ajaxNotification', ['$rootScope', function ($rootScope) {
    // private notification messages
    var startEvent = 'ajaxNotificationStarted';
    var endEvent = 'ajaxNotificationEnded';

    return {
        requestStarted: function () {
            $rootScope.$broadcast(startEvent);
        },
        requestEnded: function () {
            $rootScope.$broadcast(endEvent);
        },
        onRequestStarted: function (handler) {
            $rootScope.$on(startEvent, function (event) {
                handler(event);
            });
        },

        onRequestEnded: function (handler) {
            $rootScope.$on(endEvent, function (event) {
                handler(event);
            });
        },

    };
}]);

app.directive('ajaxnotificationpopup', ['ajaxNotification', function (ajaxNotification) {
    return {
        restrict: "E",
        replace: true,
        template: '<div class="ajax-spinner" ng-show="loader"><div class="ajax-spinner-container"><span class="spinner"></span></div></div>',
        link: function (scope) {
            scope.loader = false;

            ajaxNotification.onRequestStarted(function () {
                scope.loader = true;
            });
            ajaxNotification.onRequestEnded(function () {
                scope.loader = false;
            });
        }
    };
}]);
