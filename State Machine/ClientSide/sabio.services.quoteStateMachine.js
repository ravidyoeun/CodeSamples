//====================================================================
// sabio.services
//====================================================================
sabio.services = sabio.services || {};

sabio.services.quoteStateMachine = sabio.services.quoteStateMachine || {};



//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// AJAX GET CURRENT STATUS
sabio.services.quoteStateMachine.getStatusByQuoteId = function (quoteId, onAjaxSuccess, onAjaxError) {
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
sabio.services.quoteStateMachine.updateStateforQuote = function (quoteId, data, onAjaxSuccess, onAjaxError) {
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