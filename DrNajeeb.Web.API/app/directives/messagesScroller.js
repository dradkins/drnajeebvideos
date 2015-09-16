(function (app) {

    var messagesScroller = function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                element.slimScroll({
                    height: '340px'
                });
            }
        };
    };
    app.directive("messagesScroller", messagesScroller);

}(angular.module("DrNajeebAdmin")));