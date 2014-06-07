
'use strict';

var app = angular.module('tableFilter', []);

app.factory('tableFilterNotification', ['$rootScope', function ($rootScope) {
    // private notification messages
    var prevPageEvent = 'pagerPrevNotification';
    var nextPageEvent = 'pagerNextNotification';
    var tokenAddedEvent = 'tokenAddedNotification';
    
    var previousTokens = [];
    var currentToken = "";
    var nextTokens = [];
    return {
        isNextEnabled: function () {
            return nextTokens.length > 0;
        },
        isPrevEnabled : function() {
            return previousTokens.length > 0;
        },
        nextPage: function () {
            if (nextTokens.length > 0) {
                var token = nextTokens.shift();
                previousTokens.push(currentToken);
                currentToken = token;
                $rootScope.$broadcast(nextPageEvent, token);
            }
        },
        initialize: function (token) {
            previousTokens = [];
            currentToken = "";
            if (token) {
                nextTokens = [token];
            }
            $rootScope.$broadcast(tokenAddedEvent, token);
        },
        addToken: function (token) {
            if (token && nextTokens.length == 0) {
                nextTokens.push(token);
            }
            $rootScope.$broadcast(tokenAddedEvent, token);
        },
        prevPage: function () 
        {
            if (previousTokens.length > 0) {
                var token = previousTokens.pop();
                nextTokens.unshift(currentToken);
                currentToken = token;
                $rootScope.$broadcast(prevPageEvent, token);
            }
        },
        onNextPage: function (handler) {
            $rootScope.$on(nextPageEvent, function (event, data) {
                handler(data);
            });
        },
        onPrevPage: function (handler) {
            $rootScope.$on(prevPageEvent, function (event, data) {
                handler(data);
            });
        },
        onTokenAdded: function (handler) {
            $rootScope.$on(tokenAddedEvent, function (event, data) {
                handler(data);
            });
        }

    };
}]);

app.directive('tablepagerlinks', ['tablePagerNotification', function (tablePagerNotification) {
    return {
        restrict: "E",
        replace:true,
        template: '<ul class="pagination">\
            <li><a ng-click="prevClick();" ng-class="{disabled: !prevEnabled}">&laquo; Prev</a></li>\
            <li><a ng-click="nextClick();" ng-class="{disabled: !nextEnabled}">Next &raquo;</a></li>\
        </ul>',
        link: function (scope) {
            scope.nextClick = function() {
                if (scope.nextEnabled) {
                    tablePagerNotification.nextPage();
                }
            };
            scope.prevClick = function () {
                if (scope.prevEnabled) {
                    tablePagerNotification.prevPage();
                }
            };
            tablePagerNotification.onTokenAdded(function() {
                scope.nextEnabled = tablePagerNotification.isNextEnabled();
                scope.prevEnabled = tablePagerNotification.isPrevEnabled();
            });
        }
    };
}]);
