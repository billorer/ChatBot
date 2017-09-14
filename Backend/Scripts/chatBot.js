$(document).ready(function () {

    //$('#submitButton').keypress(function (e) {
    //    if (e.keyCode == 13)
    //    {
    //        ajaxAnswerInviter();
    //    }
    //});

    /**
     * Variables used for the dotDraw() function
        interValDots and answerProcedureFinished variables are used to stop the drawing function
        dots variable contains the number of dots on the screen
     */
    var interValDots;
    var answerProcedureFinished = false;
    var dots = 0;

    $('#submitButton').click(ajaxAnswerInviter);

    /**
     * This function invites the ajax functions
       To get the information for the user on the screen and also to get the answer for his question
       Disables the submit button and appears its question in the chatbox
     */
    function ajaxAnswerInviter() {
        if ($('#usermsg').val() != "") {

            var ajaxInfoMessage = $.ajax({
                type: "GET",
                url: "/Home/UpdateViewInfoMessage"
            });
            ajaxInfoMessage.done(updateInfoMessage);

            $('#submitButton').prop('disabled', true);

            $('#chatbox').append("<div class='chatMe'><" + getTime() + "><span class='chatUserName'> Me: </span>" + $('#usermsg').val() + "</div>");
            updateScroll();

            var ajaxGetAnswer = $.ajax({
                type: "GET",
                url: "/Home/ChatMessage",
                data: {
                    "msg": $('#usermsg').val(),
                    "method": $('input[name=optradio]:checked').val()
                }
            });
            ajaxGetAnswer.done(proceedAnswer);
        }
        return;
    }

    /**
     * This function updates the info message section and invites the dotDrawer() function
     * @param {any} result
     */
    function updateInfoMessage(result) {
        $('#errorMessage').after(result);
        answerProcedureFinished = false;
        interValDots = setInterval(dotDrawer, 1000);
    }

    /**
     * This funcion clears the input text field
       Updates the scroll on the chatbox
       Appears the message from the result
       Enables the submit button
     * @param {any} result
     */
    function proceedAnswer(result) {
        $('#usermsg').val("");

        if (result == "You must choose a method (QnA or Accord Bot)!") {
            $('#errorMessage').append(result);
        } else {
            answerProcedureFinished = true;
            $('#errorDiv').empty();
            $('#chatbox').append("<div class='chatAssistant'><p><" + getTime() + "><span class='chatAssistantName'> Assistant: </span></p>" + result + "</div>");
        }
        updateScroll();
        $('#submitButton').prop('disabled', false);
    }

    /**
     * This function draws dots on the screen, in the error message section
       If the dots number reaches 3 then its reseted and restarted.
     */
    function dotDrawer() {
        if (dots < 3) {
            $('#wait').append('.');
            dots++;
        }
        else {
            $('#wait').empty();
            dots = 0;
        }
        if (answerProcedureFinished) {
            clearInterval(interValDots);
            $('#wait').empty();
        }
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