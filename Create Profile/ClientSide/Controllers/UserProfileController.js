(function () {
    "use strict";

    angular.module(APPNAME)
        .controller('userProfileApiController', UserProfileApiController);

    UserProfileApiController.$inject = ['$scope', '$baseController', "$UserService", '$uibModal'];

    function UserProfileApiController(
         $scope
         , $baseController
         , $UserService
         , $uibModal
   ) {

        //  controllerAs with vm syntax: https://github.com/johnpapa/angular-styleguide#style-y032
        var vm = this;//this points to a new {}

        vm.item = null;

        $baseController.merge(vm, $baseController);

        vm.$UserService = $UserService;
        vm.$scope = $scope;
        vm.$uibModal = $uibModal;
        //  bindable members (functions) always go up top while the "meat" of the functions go below: https://github.com/johnpapa/angular-styleguide#style-y033
        vm.receiveItems = _receiveItems;
        vm.onEmpError = _onEmpError;

        vm.openModal = _openModal;


        //  modal directive
        vm.modalSelected = null;


        //-- this line to simulate inheritance
        $baseController.merge(vm, $baseController);

        //this is a wrapper for our small dependency on $scope
        vm.notify = vm.$UserService.getNotifier($scope);

        //this is like the sabio.startUp function

        // render();
        renderProfile();
        function renderProfile() {
            //  defer data operations into an external service: https://github.com/johnpapa/angular-styleguide#style-y035
            vm.$UserService.getById(vm.receiveItems, vm.onEmpError);

        }

        function _receiveItems(data) {
            //this receives the data and calls the special
            //notify method that will trigger ng to refresh UI
            vm.notify(function () {
                vm.item = data.item;


                //console.log('renderProfile(): ', vm.item);

                //vm.firstName = vm.item.firstName;
                vm.lastName = vm.item.lastName;
                vm.jobTitle = vm.item.jobTitle;
                vm.email = vm.item.email;
                vm.companyName = vm.item.companyName;
                vm.phoneNumber = vm.item.phoneNumber;
                vm.companyRole = vm.item.companyRole;
                //console.log("COMPANY ROLE", vm.companyRole);
                //console.log('URL link before if statement: ', vm.item.url);

                if (vm.item.url === null) {

                    vm.item.url = "http://gurucul.com/wp-content/uploads/2015/01/default-user-icon-profile.png";
                };
                vm.url = vm.item.url;
            });
        }



        function _selectEmployee(anEmp) {
            //console.log("anEmp: ", anEmp);
            vm.selectedProfile = anEmp;
        }

        function _onEmpError(jqXhr, error) {
            //console.error('ERROR:', error);
        }


        // function to handle modal
        function _openModal() {
            var modalInstance = vm.$uibModal.open({
                animation: true,
                templateUrl: '/Scripts/app/UserProfile/Templates/EditUserProfileModal.html',       //  this tells it what html template to use. it must exist in a script tag OR external file
                controller: 'editProfileModalController as epmc',    //  this controller must exist and be registered with angular for this to work

                resolve: {  //  anything passed to resolve can be injected into the modal controller as shown below
                    userProfileItems: function () {
                        console.log('vm.item', vm.item);
                        return vm.item;
                    }
                }
            });

            //  when the modal closes it returns a promise
            modalInstance.result.then(function () {
                //  if the user closed the modal by clicking Save
                renderProfile();

            }, function () {
                //console.log('Modal dismissed at: ' + new Date());   //  if the user closed the modal by clicking cancel
            });
        }

     

    }
})();