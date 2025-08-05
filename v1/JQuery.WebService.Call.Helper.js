//-----------------------------------------------------------------------------+
// jQuery call AJAX Page Method                                                |
//-----------------------------------------------------------------------------+
function PageMethod(fn, paramArray, successFn, errorFn) {
    //console.log("pagemethod2");
    //var pagePath = window.location.pathname;
    //var pathName = window.location.pathname.split('/');
    var pagePath = "WebService.asmx";
    //-------------------------------------------------------------------------+
    // Create list of parameters in the form:                                  |
    // {"paramName1":"paramValue1","paramName2":"paramValue2"}                 |
    //-------------------------------------------------------------------------+
    var paramList = '';

    if (paramArray.length > 0) {
        for (var i = 0; i < paramArray.length; i += 2) {
            if (paramList.length > 0) paramList += ',';
            paramList += '"' + paramArray[i] + '":"' + paramArray[i + 1] + '"';
        }
    }
    paramList = '{' + paramList + '}';
    //console.log(paramList);
    //Call the page method
    var request = $.ajax({
        cache: false,
        type: "POST",
        url: pagePath + "/" + fn,
        contentType: "application/json; charset=utf-8",
        data: paramList,
        dataType: "json"
    });
    request.done(successFn);
    request.fail(errorFn);
}