import Java from "frida-java-bridge";

if (!Java.available) {
    send({
        type: "log",
        level: "warning",
        message: "Java runtime is not available"
    });
} else {
    Java.perform(function () {
        send({
            type: "log",
            message: "Java runtime is available"
        });

        const Activity = Java.use("android.app.Activity");

        Activity.onResume.implementation = function () {
            send({
                type: "event",
                name: "activity.onResume",
                activity: this.getClass().getName()
            });

            return this.onResume();
        };
    });
}
