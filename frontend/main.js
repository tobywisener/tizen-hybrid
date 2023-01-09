(function() {
    var APP_ID = 'YyOUsYYoBY.TizenDotNet1.dll',

    outputElement = document.getElementById('appControlOutput'),
        localPort = null;

    outputElement.innerHTML += "<br/>Starting message port...";

    try {
        localPort = tizen.messageport.requestLocalMessagePort('example_message_port');

        var localPortWatchId = localPort.addMessagePortListener(function(data, replyPort) {
            for (var i = 0; i < data.length; i++) {
                outputElement.innerHTML += "<br/>MESSAGEPORT: " + data[i].key + " - " + JSON.stringify(data[i]); //.value
            }
            if (replyPort) {
                outputElement.innerHTML += '<br/>replyPort given: ' + replyPort.messagePortName;
            }
        });

        outputElement.innerHTML += "<br/>Listening messgae port " + localPortWatchId;
    } catch (er) {
        outputElement.innerHTML = "<br/>(MP) Fatal error: " + er;
    }

    outputElement.innerHTML += "<br/>Starting app control...";

    try {

        const appControl = new tizen.ApplicationControl('http://org.example.TizenDotNet1/dlnascan', null, 'application/json', null, [new tizen.ApplicationControlData("key", ["value"])]);

        tizen.application.launchAppControl(appControl, APP_ID, function () {
            outputElement.innerHTML += "<br/>launchAppControl success";
        }, function (error) {
            outputElement.innerHTML += "<br/>launchAppControl failure: " + error;
        }, {
            /* Callee sent a reply. */
            onsuccess: function(data)
            {
                outputElement.innerHTML += "<br/>appControlReply reply " + JSON.stringify(data);
            },
            /* Callee returned failure. */
            onfailure: function(data)
            {
                outputElement.innerHTML += "<br/>appControlReply failure " + JSON.stringify(data);
            }
        });
    } catch (er) {
        outputElement.innerHTML = "<br/>(AC) Fatal error: " + er;
    }
})();
