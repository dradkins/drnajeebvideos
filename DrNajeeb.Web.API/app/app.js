(function () {

    var app = angular.module("DrNajeebAdmin",
        [
            "ngRoute",
            "angular-loading-bar",
            "ui.bootstrap",
            "720kb.datepicker",
            "color.picker",
            "checklist-model",
            "ngAnimate",
            "ui.sortable",
            "toastr",
            "jcs-autoValidate",
            "LocalStorageModule",
            "chart.js"
        ]);

    app.config(function ($routeProvider) {
        $routeProvider
            .when("/dashboard", {
                templateUrl: "/app/views/dashboard.html",
                controller: "DashboardController"
            })
            .when("/subscription", {
                templateUrl: "/app/views/subscription.html",
                controller: "SubscriptionController"
            })
            .when("/categories", {
                templateUrl: "/app/views/categories.html",
                controller: "CategoriesController"
            })
            .when("/features", {
                templateUrl: "/app/views/features.html",
                controller: "FeaturesController"
            })
            .when("/videos", {
                templateUrl: "/app/views/videos.html",
                controller: "VideosController"
            })
            .when("/videos/:searchTerm", {
                templateUrl: "/app/views/videos.html",
                controller: "VideosController"
            })
            .when("/users", {
                templateUrl: "/app/views/users.html",
                controller: "UserController"
            })
            .when("/login", {
                templateUrl: "/app/views/login.html",
                controller: "LoginController"
            })
            .when("/support", {
                templateUrl: "/app/views/support.html",
                controller: "SupportController"
            })
            .when("/categories/:categoryId/:categoryName/videos", {
                templateUrl: "/app/views/CategoryVideos.html",
                controller: "CategoryVideoController"
            })
            .when("/most-watched-videos", {
                templateUrl: "/app/views/most_watched_videos.html",
                controller: "MostWatchedVideosController"
            })
            .when("/video-download-stats", {
                templateUrl: "/app/views/video_download_stats.html",
                controller: "VideoDownloadStatsController"
            })
            .when("/downloads/:userId", {
                templateUrl: "/app/views/user_video_download_stats.html",
                controller: "UserVideoDownloadStatsController"
            })
            .when("/user-stats", {
                templateUrl: "/app/views/user_stats.html",
                controller: "UserStatsController"
            })
            .when("/revenue", {
                templateUrl: "/app/views/revenue.html",
                controller: "RevenueController"
            })
            .when("/most-active-users", {
                templateUrl: "/app/views/most_active_users.html",
                controller: "MostActiveUsersController"
            })
            .when("/ghost-users", {
                templateUrl: "/app/views/ghost_users.html",
                controller: "GhostUsersController"
            })
            .when("/user-activities/:userId", {
                templateUrl: "/app/views/user_activities.html",
                controller: "UserActivityController"
            })
        .otherwise({ redirectTo: "/dashboard" })
    });

    app.run(function ($rootScope, $location, CurrentUserService) {
        $rootScope.location = $location;
        $rootScope.USER_NAME = CurrentUserService.profile.username;
        $rootScope.FullName = CurrentUserService.profile.fullName;
        $rootScope.LOG_OUT = function () {
            CurrentUserService.logout();
        }
        $rootScope.SEARCH_TERM = "";
        $rootScope.SEARC_VIDEO = function () {
            console.log($rootScope.SEARCH_TERM);
            if ($rootScope.SEARCH_TERM && $rootScope.SEARCH_TERM.length > 2) {
                $location.path("/videos/" + $rootScope.SEARCH_TERM);
            }
            else {
                alert("Input no valid, Min. length is 3 characters");
            }
        }
    });

}());