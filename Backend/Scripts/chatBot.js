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

            $('#submitButton').prop('disabled', true);

            $('#chatbox').append("<div class='chatMe'><" + getTime() + "><span class='chatUserName'> Me: </span>" + $('#usermsg').val() + "</div>");
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
        $('#chatbox').append("<div class='chatAssistant'><p><" + getTime() + "><span class='chatAssistantName'> Assistant: </span></p>" + result + "</div>");
        updateScroll();
        $('#submitButton').prop('disabled', false);
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