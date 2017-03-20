//====================================================================
// quotemule.services
//====================================================================
quotemule.services = quotemule.services || {};

quotemule.services.quoteStateMachine = quotemule.services.quoteStateMachine || {};



//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// AJAX GET CURRENT STATUS
quotemule.services.quoteStateMachine.getStatusByQuoteId = function (quoteId, onAjaxSuccess, onAjaxError) {
    var url = "/api/quotestate/" + quoteId;

    var settings = {
        cache: false,
        dataType: "json",
        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
        success: onAjaxSuccess,
        error: onAjaxError,
        type: "GET"
    };

    $.ajax(url, settings);


};


//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// AJAX UPDATE QUOTE STATE - Publish State 1
quotemule.services.quoteStateMachine.updateStateforQuote = function (quoteId, data, onAjaxSuccess, onAjaxError) {
    var url = "/api/quotestate/" + quoteId;

    var settings = {
        cache: false,
        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
        data: data,
        dataType: "json",
        success: onAjaxSuccess,
        error: onAjaxError,
        type: "PUT"
    };
    $.ajax(url, settings);
};