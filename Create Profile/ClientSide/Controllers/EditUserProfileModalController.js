//- Angular Modal Controller
(function () {
    "use strict";

    angular.module(APPNAME)
        .controller('editProfileModalController', EditProfileModalController);

    EditProfileModalController.$inject = ['$scope', '$baseController', '$uibModalInstance', '$UserService', '$alertService', 'userProfileItems']

    function EditProfileModalController(
        $scope
        , $baseController
        , $uibModalInstance
        , $UserService
        , $alertService
        , userProfileItems) {

        var vm = this;

        $baseController.merge(vm, $baseController);

        vm.alertService = $alertService;
        vm.$scope = $scope;
        vm.$uibModalInstance = $uibModalInstance;
        vm.$UserService = $UserService;
        vm.profile = userProfileItems;
        vm.updateProfile = _updateProfile;

        vm.onDropzoneSending = _onDropzoneSending;
        vm.onDropzoneSuccess = _onDropzoneSuccess;

        vm.updateSuccess = function () {
            console.log('click save');
            vm.alertService.success('Updated Profile', 'Success!');
            vm.$uibModalInstance.close();
        };

        vm.modalError = function (data) {
            console.log('data', data);
            vm.alertService.error('There was an error!', 'Uh-Oh');
            vm.$uibModalInstance.dismiss('cancel');
        };
        vm.cancel = function () {
            vm.$uibModalInstance.dismiss('cancel');
        };


        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        function _updateProfile() {

            var updateProfilePayload = {

                "userId": $('#PAGEUSER').val(), // <-- current user id
                "firstName": vm.profile.firstName,
                "lastName": vm.profile.lastName,
                //"jobTitle": vm.profile.jobTitle,
                "phoneNumber": vm.profile.phoneNumber,
                // Captured by the dropzone success handler
                "mediaId": vm.profile.mediaId


            };

            console.log('updateProfilePayload.userid: ', updateProfilePayload.userId);

            vm.$UserService.updateProfile(sabio.p.currentUserId, updateProfilePayload, vm.updateSuccess, vm.modalError);
        };




        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        function _onDropzoneSending(file, xhr, formData) {
            console.log("DZ Sending");

            var mediaType = "profilePicture"; // <-- This value is specific to a profile-picture upload modal. 

            // User IDs are stored as strings
            var userId = sabio.p.currentUserId; // <-- global variable for userid

            // Company IDs are stored as int
            var companyId = Number(sabio.p.companyId);  // <-- global variable for company id

            formData.append("MediaType", mediaType);
            formData.append("UserId", userId);
            formData.append("CompanyId", companyId);

        }; // End _onDropzoneSending




        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        function _onDropzoneSuccess(file, response) {
            console.log("DZ Success");

            // User IDs are stored as strings
            var id = sabio.p.currentUserId;
            console.log('dropzone success response:', response)
            var payload = {
                'UserId': id,
                'MediaId': response.item
            };

            vm.profile.mediaId = response.item;

            var onSuccess = function (data) { console.log("update success! ", data); };
            var onError = function (data) { console.log("An error occured: ", data) };

            sabio.services.public.updateProfileMediaId(id, payload, onSuccess, onError);

        }; // End _onDropzoneSuccess
    }

})();