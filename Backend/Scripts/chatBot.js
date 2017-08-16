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
            $.ajax({
                type: "GET",
                url: "/Home/ChatMessage",
                data: {
                    "msg": $('#usermsg').val(),
                    "method": $('input[name=optradio]:checked').val()
                },
                success: function (result) {
                    $('#usermsg').val("");
                    $('#chatbox').append("<p class='chatAssistant'><" + getTime() + "> Assistant: " + result + "</p>");
                    updateScroll();
                }
            });
        }
        return;
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