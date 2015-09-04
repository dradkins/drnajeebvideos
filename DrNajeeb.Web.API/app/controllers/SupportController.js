'use strict';

(function (app) {

    var SupportController = function ($scope, SupportService, toastr) {

        $scope.contactedUsers = [];
        $scope.messages == [];
        $scope.pagingInfo = {
            page: 1,
            itemsPerPage: 50,
            totalItems: 0
        };
        $scope.selectedUser = null;
        $scope.support = {
            subject: "",
            message: "",
            toUserId: null
        };
        $scope.form = null;


        $scope.selectPage = function (page) {
            $scope.pagingInfo.page = page;
            loadUsers();
        };

        $scope.showMessages = function (user) {
            user.totalUnreadMessages = 0;
            $scope.selectedUser = user;
            $scope.messages = [];
            SupportService.loadUserMessage(user.userId).then(onUserMessages, onUserMessagesError);
        }

        $scope.sendMessage = function (form) {
            $scope.form = form;
            if (form.$valid) {
                $scope.support.toUserId = $scope.selectedUser.userId;
                SupportService.sendMessage($scope.support)
                            .then(onMessageSend, onMessageError);
            }
            else {
                toastr.warning("The data you provided is not valid. Please verify data and send again.")
            }
        };

        var onMessageSend = function (data) {
            $scope.support.message = "";
            $scope.form.$setPristine(true);
            $scope.messages.push(data);
            toastr.info("message sent to user successfully");
        }

        var onMessageError = function (error) {
            console.log(error);
            toastr.error("unable to send message at this time");
        }

        var onUserMessages = function (data) {
            console.log(data);
            $scope.messages = data;
        }

        var onUserMessagesError = function (error) {
            console.log(error);
            toastr.error("unable to load user messages at this time please try again in few seconds");
        }

        var onUsersLoaded = function (data) {
            console.log(data);
            $scope.contactedUsers = data;
        }

        var onUsersLoadingError = function (error) {
            console.log(error);
            toastr.error("unable to load users at this time, please try later");
        }

        var loadUsers = function () {
            console.log("Loading Users");
            SupportService.loadUsers($scope.pagingInfo).then(onUsersLoaded, onUsersLoadingError);
        }

        loadUsers();

    };

    SupportController.$inject = ["$scope", "SupportService", "toastr"];

    app.controller("SupportController", SupportController);

}(angular.module("DrNajeebAdmin")));