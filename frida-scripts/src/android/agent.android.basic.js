import { installExternalStorageHooks } from "./modules/android.external_storage.js";
import { installSharedPreferencesHooks } from "./modules/android.shared_preferences.js";

const PROFILE_ID = "android.basic";
const SCHEMA_VERSION = 1;

function emit(event) {
    send({
        schemaVersion: SCHEMA_VERSION,
        profileId: PROFILE_ID,
        platform: "android",
        timestamp: new Date().toISOString(),
        ...event
    });
}

function installModule(moduleId, installer) {
    try {
        emit({
            type: "module.installing",
            moduleId
        });

        installer({
            emit: event => emit({
                moduleId,
                ...event
            })
        });

        emit({
            type: "module.installed",
            moduleId
        });
    } catch (e) {
        emit({
            type: "module.error",
            moduleId,
            message: String(e),
            stack: e && e.stack ? String(e.stack) : null
        });
    }
}

emit({
    type: "agent.started"
});

installModule("android.external_storage", installExternalStorageHooks);
installModule("android.shared_preferences", installSharedPreferencesHooks);

emit({
    type: "agent.ready"
});
