Java.perform(function () {
    send({
        type: "log",
        message: "Java runtime is available"
    });

    var Activity = Java.use("android.app.Activity");

    Activity.onResume.implementation = function () {
        send({
            type: "event",
            name: "activity.onResume",
            activity: this.getClass().getName()
        });

        return this.onResume();
    };
});
