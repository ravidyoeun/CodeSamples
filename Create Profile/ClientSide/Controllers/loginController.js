(function () {
    "use strict";

    angular.module(APPNAME)
        .controller('loginApiController', LoginApiController);

    LoginApiController.$inject = ['$scope', '$baseController', "$UserService", '$uibModal'];




    function LoginApiController(
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
      
        vm.onEmpError = _onEmpError;

        vm.openModal = _openModal;

        vm.redirectToProductMarketPlace = _redirectToProductMarketPlace;
        vm.redirectToSupplierSearch = _redirectToSupplierSearch;

        //  modal directive
        vm.modalSelected = null;


        //-- this line to simulate inheritance
        $baseController.merge(vm, $baseController);

        //this is a wrapper for our small dependency on $scope
        vm.notify = vm.$UserService.getNotifier($scope);

        //this is like the sabio.startUp function
        
   

        function _redirectToProductMarketPlace() {
            window.location = "http://quotemule.dev/home/productmarketplace";
            console.log('relocation to productmarketplace passed');
        };

        function _redirectToSupplierSearch() {

            window.location = "http://quotemule.dev/Public/SupplierProfiles";

        };


        function _selectEmployee(anEmp) {
            console.log("anEmp: ", anEmp);
            vm.selectedProfile = anEmp;
        }

        function _onEmpError(jqXhr, error) {
            console.error('ERROR:', jqXhr);
        }


        // function to handle modal
        function _openModal() {
            var modalInstance = vm.$uibModal.open({
                animation: true,

                templateUrl: '/Scripts/app/UserProfile/Templates/loginModal.html',       //  this tells it what html template to use. it must exist in a script tag OR external file
                controller: 'loginModalController as lmc',    //  this controller must exist and be registered with angular for this to work
                size: 'Lg',
                resolve: {  //  anything passed to resolve can be injected into the modal controller as shown below
                   
                }
            });

            //  when the modal closes it returns a promise
            modalInstance.result.then(function () {
                //  if the user closed the modal by clicking Login
           

            }, function () {
                console.log('Modal dismissed at: ' + new Date());   //  if the user closed the modal by clicking cancel
            });
        }

     

    }
})();