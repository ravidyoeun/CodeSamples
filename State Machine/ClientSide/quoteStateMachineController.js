
//- Controller for State Machine using NG ROUTE - Ravid Yoeun
// ** To change html template files please look for statemachine.config.js located in app/quoterequests folder
// ** The html view for state machine wizard is in CreateNg.cshtml which is controlled by this controller as mainStateController

(function () {
    "use strict";

    angular.module(APPNAME)
        .controller('quoteStateMachineController', QuoteStateMachineController);

    QuoteStateMachineController.$inject = ['$scope', '$baseController', '$location', "$quoteStateMachineService", "$quoteService", '$alertService', '$uibModal', '$timeout'];

    function QuoteStateMachineController(
          $scope
        , $baseController
        , $location
        , $quoteStateMachineService
        , $quoteService
        , $alertService
        , $uibModal
        , $timeout
   ) {
        console.log("quoteStateMachineController is running!!!");

        var vm = this;

        $baseController.merge(vm, $baseController);

        vm.$scope = $scope;
        vm.alertService = $alertService;
        vm.$location = $location
        vm.$quoteStateMachineService = $quoteStateMachineService;
        vm.$quoteService = $quoteService;
        vm.$timeout = $timeout;
        vm.$scope = $scope;
        vm.$uibModal = $uibModal;
        vm.selectedQuoteId = $('#QUOTEID').val();

        vm.userId = $('#PAGEUSER').val();
        vm.onEmpError = _onError;

        vm.setSubmit = _updateStatus;
        vm.setAccept = _updateStatus;
        vm.setAcceptedAddContract = _updateStatus;
        vm.setContractAdded = _updateStatus;
        vm.setAlter = _updateStatus;
        vm.setDecline = _updateStatus;
        vm.setComplete = _updateStatus;
        vm.getStatusById = _getStatusById;

        vm.thisStatus = 0;
        vm.modelStatus = null;

        vm.stateName = null;
        vm.quoteState = null;
        vm.statusClass = null;
        vm.statusCheck = 0;
        vm.statusObject = null;
        //  modal directive
        vm.modalSelected = null;
        vm.statusObjectState = null;
        vm.Eventid = 0;
        vm.draftStatus = false;
        vm.reviewStatus = false;
        vm.pendingStatus = false;
        vm.completeStatus = false;
        vm.quoteRequestData = null;

        vm.currentRequestLabel = "Current Request:";

        //vm.tabs = [
        //  {
        //      link: '#/',
        //      label: 'Draft'
        //  },
        //  {
        //      link: '#/active',
        //      label: 'Active'
        //  },
        //  {
        //      link: '#/pending',
        //      label: 'Pending'
        //  },
        //  {
        //      link: '#/complete',
        //      label: 'Complete'
        //  }
        //];

        //vm.selectedTab = vm.tabs[0];

        // vm.tabClass = _tabClass;
        // vm.setSelectedTab = _setSelectedTab;

        // Make sure to wrap recent data calls in a notify function to let angular know of the changes
        vm.notify = vm.$quoteStateMachineService.getNotifier($scope);


        // Run this function on startup
        vm.getStatusById();
        //render();

        //function render() {
        //    vm.setUpCurrentRequest(vm);
        //    console.log('vm.setSelectedTab:', vm.setSelectedTab);
        //    switch (vm.currentRequest.originalPath) {
        //        case "/":
        //            vm.heading = "Main Controller";
        //            vm.message = "hello! welcome to the routes demo. I am the main controller and this is the main page.";
        //            break;

        //        case "/active":
        //            vm.heading = "About Us";
        //            vm.message = "This text is coming from the main controller also but it's changing because of the new route. It's the same controller but it loads a different template into ng-view.";
        //            break;

        //        case "/pending":
        //            vm.heading = "Querystring parameters are key/value pairs.";
        //            vm.message = "They are passed in the URL of the page on GET requests. Notice how Angular captures all of these params in a variable which you can access as $route.current.params.";
        //            break;

        //        case "/complete":
        //            vm.heading = "Querystring parameters are key/value pairs.";
        //            vm.message = "They are passed in the URL of the page on GET requests. Notice how Angular captures all of these params in a variable which you can access as $route.current.params.";
        //            break;
        //    }
        //}


        // Fetch the current Status of the Quote Request
        // QRStatus: 0 = Cancelled or Delete, 1 = Draft, 2 = Active, 3 = Pending, 4 = Complete
        function _getStatusById() {

            console.log('Grabbing the current Status of the QuoteId Project');

            // Call the service for getting the current Status Id
            vm.$quoteStateMachineService.getStatusByQuoteId(vm.selectedQuoteId, _onSuccess, _onError);
        };


        function _onError(jqXhr, error) {
            console.error();
        };

        // If the service call for getting the status is successful 
        function _onSuccess(data) {

            vm.notify(function () {
                console.log('Rendering Success handler!!! Heres the data received: ', data);
                vm.quoteName = data.item.quoteRequestName;
                vm.stateName = data.item.stateName;
                vm.quoteState = data.item.quoteState;
                console.log('The Current Status is: ', vm.statusCheck);

                // Set the correct class names for the state wizard tabs
                _checkStateWizard(vm.statusCheck);

                // Set the correct button views depending on the status
                //_showButtons(vm.statusCheck);

                // Force view redirect if they change url in browser
                _forceRedirectToCorrectView(vm.thisStatus);

                setTimeout(function () {
                    $(window).trigger('resize');
                    //console.log('trigger resize running');
                }, 200);
            });
        };

        //When ever a button is pressed this function is executed and we pass the Eventid from the buttons
        function _updateStatus(Eventid) {

            console.log('Button Pressed');

            //vm.$quoterequestService.GetByQuoteId(vm.selectedQuoteRequestId, _captureProjectName, _onError);
            console.log('current quoteid: ', vm.selectedQuoteId);
            var stateData = {

                "quoteId": vm.selectedQuoteId,
                "companyId": vm.userId,
                "quoteState": vm.quoteState,
                "EventId": Eventid
            };

            // We set the Eventid value so we can use it to properly redirect the user
            vm.thisEventId = Eventid;

            //console.log('stateData.EventId: ', stateData.EventId);

            //- update to the appropriate status
            vm.$quoteStateMachineService.updateStateforQuote(stateData.quoteId, stateData, _onSuccessUpdate, _onError);

        };


        //- Success handler for when a status updates
        function _onSuccessUpdate(data) {

            console.log("Status Updated!", data);
            //location.reload(true);
            //if (vm.thisEventId == 7) {
            //    vm.alertService.success('Quote Request: ' + vm.quoteName, 'Completed!');
            //}
            //redirect the view to the updated url for Ng Route
            _redirectTo(vm.thisEventId);



        };

        //- This function will properly display the correct information for the QR's status
        function _checkStateWizard(statusCheck) {


            console.log('running _checkStateWizard');
            console.log('statusCheck Value:', statusCheck);
            var statusObject = [{

                'link': '#/',
                'stepNumb': '1',
                'stepStatus': 'Draft',
                'stepTitle': 'Step 1 Draft',
                'statusClass': 'selected',


            },
            {
                'link': '#/active',
                'stepNumb': '2',
                'stepStatus': 'Accept Bids',
                'stepTitle': 'Step 2 Accept Bids',
                'statusClass': 'selected'

            }
           ,
           {
               'link': '#/pending',
               'stepNumb': '3',
               'stepStatus': 'Review Bids',
               'stepTitle': 'Step 3 Review Bids',
               'statusClass': 'selected'

           }
           ,
           {
               'link': '#/complete',
               'stepNumb': '',
               'stepStatus': 'Complete',
               'stepTitle': 'Quote Request Completed',
               'statusClass': 'selected'

           }]

            //- Here we are setting the correct CSS styling for the state wizard status
            //- e.g. class="selected" 
            //- CSS helper
            //- selected = Arrow , done = green check mark, disabled = greyed out circle

            // loop through the object
            for (var i = 0; i < statusObject.length; i++) {
                // check the statusId for draft status
                if (statusCheck <= 1) {

                    //status is in draft mode
                    statusObject[0].statusClass = 'selected';
                    statusObject[1].statusClass = 'disabled';
                    statusObject[2].statusClass = 'disabled';
                    statusObject[3].statusClass = 'disabled';

                }
                else if (statusCheck == 2) {

                    //status is in active mode
                    statusObject[0].statusClass = 'done';
                    statusObject[1].statusClass = 'selected';
                    statusObject[2].statusClass = 'disabled';
                    statusObject[3].statusClass = 'disabled';

                }
                else if (statusCheck == 3) {

                    //status is in pending mode
                    statusObject[0].statusClass = 'done';
                    statusObject[1].statusClass = 'done';
                    statusObject[2].statusClass = 'selected';
                    statusObject[3].statusClass = 'disabled';

                }
                else {

                    //status is in complete mode
                    statusObject[0].statusClass = 'done';
                    statusObject[1].statusClass = 'done';
                    statusObject[2].statusClass = 'done';
                    statusObject[3].statusClass = 'done';

                }
            }

            // Pass in the value back
            vm.statusObjectState = statusObject;

        };


        //function _tabClass(tab) {
        //    if (vm.selectedTab == tab) {
        //        return "selected";
        //    } else {
        //        return "";
        //    }
        //}

        //function _setSelectedTab(tab) {
        //    console.log("set selected tab", tab);
        //    vm.selectedTab = tab;
        //}

        // this function will properly redirect the user to the correct view depending on the EventId 
        function _redirectTo(thisEventId) {

            // depending on the event the user pressed, the url will switch to move to the next tab
            switch (thisEventId) {
                case 0:
                    vm.link = '/active';
                    break;
                case 2:
                    vm.link = '/active';
                    break;
                case 3:
                    vm.link = '/pending';
                    break;
                case 4:
                    vm.link = '/active';
                    break;
                case 5:
                    vm.link = '/active';
                    break;
                case 6:
                    vm.link = '/';
                    break;
                case 7:
                    vm.link = '/complete';
                    break;
                default:
                    break;
            }

            // In order for $location.path to work correctly, we put it in a notify function
            vm.notify(function () {
                console.log('redirecting to: ', vm.link);
                $location.path(vm.link);

            });

            // Call Get Status function to reload everything
            vm.getStatusById();

        }

        //- this function shows the correct set of buttons depending on the status
        //function _showButtons(thisStatus) {

        //    // show the correct buttons
        //    switch (thisStatus) {
        //        case 1:
        //            vm.draftStatus = true;
        //            break;
        //        case 2:
        //            vm.reviewStatus = true;
        //            break;
        //        case 3:
        //            vm.pendingStatus = true;
        //            break;
        //        case 4:
        //            vm.completeStatus = true;
        //            break;

        //        default:
        //            break;
        //    };
        //}

        //- We force the user to be redirected to the correct view if they try to change the URL link >:( 
        function _forceRedirectToCorrectView(thisStatus) {

            switch (thisStatus) {
                case 1:

                    //if user is in draft
                    vm.redirectLink = '/';
                    break;
                case 2:
                    //-if user is in active
                    vm.redirectLink = '/active';
                    break;
                case 3:
                    //-if user is in pending
                    vm.redirectLink = '/pending';
                    break;
                case 4:
                    //- if user is in complete
                    vm.redirectLink = '/complete';
                    break;

                default:
                    break;
            };

            //vm.notify(function () {
            $timeout(function () {
                console.log('running location.path');

                $location.path(vm.redirectLink);
            });


            //});
        }


    }


})();