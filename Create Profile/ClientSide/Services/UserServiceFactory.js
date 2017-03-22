(function () {
    "use strict";

    //  APPNAME is a type of variable called a "constant" because it never changes. it is defined in _LayoutAlternate.cshtml
    //  service is implemented as a factory: https://github.com/johnpapa/angular-styleguide#style-y040
    angular.module(APPNAME)
        .factory('$UserService', UserServiceFactory);

    //  manually identify dependencies for injection: https://github.com/johnpapa/angular-styleguide#style-y091
    //  $sabio is a reference to sabio.page object. sabio.page is created in sabio.js
    UserServiceFactory.$inject = ['$baseService', '$sabio'];

    function UserServiceFactory($baseService, $sabio) {
        //  sabio.page has been injected as $sabio so we can reference anything that is attached to sabio.page here
        var aSabioServiceObject = sabio.services.public;

        //  merge the jQuery object with the angular base service to simulate inheritance
        var newService = $baseService.merge(true, {}, aSabioServiceObject, $baseService);

        return newService;
    }
})();
