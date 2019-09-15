window.BlazorRedux = {
    reduxDevTools: null,
    interop: null,
    init: function () {
        console.log('Initializing dev tools!');
        var self = window.BlazorRedux;
        var ext = window.__REDUX_DEVTOOLS_EXTENSION__;

        if (!ext) {
            console.warn("Redux DevTools extension is not installed.");
            return;
        }
        ext.notifyErrors();
        self.reduxDevTools = ext.connect();
        if (!self.reduxDevTools) {
            console.warn("Could not connect to Redux DevTools.");
            return;
        }

        self.reduxDevTools.subscribe(function (message) {
            if (self.interop) {
                self.interop.invokeMethod("ReceiveMessage", message);
            }
        });
    },
    send: function (action, data, state) {
        var self = window.BlazorRedux;
        self.reduxDevTools.send({type: action, payload: data}, state);
    },
    sendInitial: function (state, interop) {
        var self = window.BlazorRedux;
        self.reduxDevTools.init(state);
        self.interop = interop;
    }
};

window.BlazorRedux.init();