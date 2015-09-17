'use strict';

(function (app) {

    var SupportController = function ($scope, SupportService, toastr, NotificationsService) {

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
                NotificationsService.sendMessage($scope.support.message, $scope.selectedUser.userName);
            }
            else {
                toastr.warning("The data you provided is not valid. Please verify data and send again.")
            }
        };

        $scope.$on("messageReceived", function (event, data) {
            toastr.info("New message received");
            if ($scope.selectedUser && $scope.selectedUser.userName == data.user) {
                $scope.messages.push({
                    messageDateTime: new Date(),
                    messageText: data.message,
                    isFromAdmin: false,
                    isFromUser: true
                });
            }
            else {
                angular.forEach($scope.contactedUsers, function (user) {
                    if (user.userName == data.user) {
                        user.totalUnreadMessages += 1;
                        return false;
                    }
                })
            }
        });

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

        var init = function () {
            NotificationsService.connect();
        }

        loadUsers();
        init();

    };

    SupportController.$inject = ["$scope", "SupportService", "toastr", "NotificationsService"];

    app.controller("SupportController", SupportController);

}(angular.module("DrNajeebAdmin")));