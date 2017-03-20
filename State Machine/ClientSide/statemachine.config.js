/// <reference path="C:\SF.code\C27\Sabio.Web\Views/Shared/_UpdateQuoteRequestForm.cshtml" />
/// <reference path="C:\SF.code\C27\Sabio.Web\Views/QuoteRequest/draftquoterequest.cshtml" />
/// <reference path="C:\SF.code\C27\Sabio.Web\Views/StateWizard/draftquoterequest.cshtml" />

//  we wrap the config in its own IIFE just like the controllers and services
(function () {
    "use strict";

  

    angular.module(APPNAME)
        .config(["$routeProvider", "$locationProvider", function ($routeProvider, $locationProvider) {

            $routeProvider.when('/', {
                templateUrl: '/StateWizard/DraftStatus',
                controller: 'quoterequestController',
                controllerAs: 'qrc'
            }).when('/active', {
                templateUrl: '/StateWizard/ActiveStatus',
                controller: 'bidController',
                controllerAs: 'bc'
            }).when('/pending', {
                templateUrl: '/StateWizard/PendingStatus',
                controller: 'bidController',
                controllerAs: 'bc'
            }).when('/complete', {
                templateUrl: '/StateWizard/CompleteStatus',
                controller: 'mainStateController',
                controllerAs: 'completeController'
            });

            $locationProvider.html5Mode(false).hashPrefix('');

        }])

        
    .controller("StateWizardController", function ($scope) {
        $scope.name = "QuoteRequestView";

    })

    

})();

