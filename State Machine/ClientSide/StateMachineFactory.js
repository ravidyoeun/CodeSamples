(function () {
    "use strict";

    //  APPNAME is a type of variable called a "constant" because it never changes. it is defined in _LayoutAlternate.cshtml
    //  service is implemented as a factory: https://github.com/johnpapa/angular-styleguide#style-y040
    angular.module(APPNAME)
        .factory('$stateMachineService', StateMachineFactory);

    //  manually identify dependencies for injection: https://github.com/johnpapa/angular-styleguide#style-y091
    //  $quotemule is a reference to quotemule.page object. quotemule.page is created in quotemule.js
    StateMachineFactory.$inject = ['$baseService', '$quotemule'];

    function StateMachineFactory($baseService, $quotemule) {
        //  quotemule.page has been injected as $quotemule so we can reference anything that is attached to quotemule.page here
        var stateMachineObject = quotemule.services.statemachine;

        //  merge the jQuery object with the angular base service to simulate inheritance
        var newService = $baseService.merge(true, {}, stateMachineObject, $baseService);

        return newService;
    }
})();
