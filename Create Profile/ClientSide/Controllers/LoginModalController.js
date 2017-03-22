//- Angular Modal Controller
(function () {
    "use strict";

    angular.module(APPNAME)
        .controller('loginModalController', LoginModalController);

    LoginModalController.$inject = ['$scope', '$baseController', '$uibModalInstance', '$UserService', '$alertService']

    function LoginModalController(
        $scope
        , $baseController
        , $uibModalInstance
        , $UserService
        , $alertService
        ) {

        var vm = this;

        $baseController.merge(vm, $baseController);

        vm.$scope = $scope;
        vm.$uibModalInstance = $uibModalInstance;
        vm.$UserService = $UserService;
        vm.login_button = _loginAccount;
        vm.alertService = $alertService;
    

        vm.loginRequestSuccess = function (data) {
            console.log('Successful login request: ', data);
            vm.alertService.success('Please wait while we login...', 'Success!');
            console.log('Running Toastr');
            vm.$uibModalInstance.close();
            // redirect to other page

            window.location.replace('/Dashboard/Index');

        };

        vm.loginRequestError = function (data) {
            console.log('An error occured trying to Log In:', data);
            vm.alertService.error('The email or password is incorrect.', 'Error');
            //vm.$uibModalInstance.dismiss('cancel');
        };
        vm.cancel = function () {
            vm.$uibModalInstance.dismiss('cancel');
        };

        function _loginAccount() {
            event.preventDefault();

            console.log('running vm.login_button');

            var loginFormPayload = {

                'Email': vm.username,
                'Password': vm.password
            };

            console.log('loginFormPayload data: ', loginFormPayload.Email);
           

            vm.$UserService.loginRequest(loginFormPayload, vm.loginRequestSuccess, vm.loginRequestError);
        };

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    }




})();