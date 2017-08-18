$(document).ready(function () {

    //$('#submitButton').keypress(function (e) {
    //    if (e.keyCode == 13)
    //    {
    //        ajaxAnswerInviter();
    //    }
    //});

    $('#submitButton').click(ajaxAnswerInviter);

    function ajaxAnswerInviter() {
        if ($('#usermsg').val() != "") {
            $('#chatbox').append("<p class='chatMe'><" + getTime() + "> Me: " + $('#usermsg').val() + "</p>");
            updateScroll();

            var promise = $.ajax({
                type: "GET",
                url: "/Home/ChatMessage",
                data: {
                    "msg": $('#usermsg').val(),
                    "method": $('input[name=optradio]:checked').val()
                }
            });

            promise.done(proceedAnswer);
            //promise.fail(errorFunction);
            //promise.always(alwaysFunction);
        }
        return;
    }

    function proceedAnswer(result) {
        $('#usermsg').val("");

        if (~result.indexOf("Figure")) {
            var splitTextBeforeAfterImage = result.match(/(.*)\img_.*(.*)/);
            var imageNumber = splitTextBeforeAfterImage[0].match(/\d+/);
            var modifiedBeforeText = splitTextBeforeAfterImage[0].split("img_" + imageNumber);

            //var finalAnswer = "";
            //if (~modifiedBeforeText[0].indexOf("listDecimal")) {
            //    var splitTextList = modifiedBeforeText[0].split("listDecimal");
            //    for (var index = 0; index < splitTextList.length; index++) {
            //        if (~splitTextList[index].indexOf("textStyleNormal")) {
            //            if (~finalAnswer.indexOf("<ul class") && ~finalAnswer.indexOf("<ul class"))
            //            finalAnswer += splitTextList[index].replace("textStyleNormal", "");
            //        } else if (!~finalAnswer.indexOf("<ul class")){
            //            finalAnswer += "<ul class='listDecimal'><li>" + splitTextList[index] + "</li>";
            //        } else {
            //            "<li>" + splitTextList[index] + "</li>";
            //        }
            //    }
            //}

            //if (~modifiedBeforeText[0].indexOf("listCircle")) {

            //}

            $('#chatbox').append("<p class='chatAssistant'><" + getTime() + "> Assistant: "
                + modifiedBeforeText[0] + " \n\r "
                + "</p><img class='figureImage' src= '/Home/GetImage?imgNumber=" + imageNumber + "' /><p class='figureText'>"
                + modifiedBeforeText[1] + "</p>"
                + "<p>");

        } else if (~result.indexOf("img_")){
            var splitTextBeforeAfterImage = result.match(/(.*)\img_.*(.*)/);
            var imageNumber = splitTextBeforeAfterImage[0].match(/\d+/);
            var modifiedBeforeText = splitTextBeforeAfterImage[0].split("img_" + imageNumber);

            $('#chatbox').append("<p class='chatAssistant'><" + getTime() + "> Assistant: "
                + modifiedBeforeText[0] + " \n\r "
                + "</p><img class='figureImage' src= '/Home/GetImage?imgNumber=" + imageNumber + "'/>");
        }
        else {
            $('#chatbox').append("<p class='chatAssistant'><" + getTime() + "> Assistant: " + result + "</p> ");
        }
        updateScroll();
    }

    /**
     * This function keeps the chatbox updated, keeps it scrolled down
     */
    function updateScroll() {
        $('#chatbox').scrollTop($('#chatbox')[0].scrollHeight);
    }

    /**
     * This function gets the current time and returns it.
     */
    function getTime() {
        var time = new Date($.now());

        var minutes = time.getMinutes();

        if (minutes < 10)
            minutesString = 0 + minutes + ""; 
        else
            minutesString = minutes;

        var curTime = time.getHours() + ':' + minutes + ':' + time.getSeconds();

        return curTime;
    }

});