import { installIosCipherHooks } from "./ios.cypher/modules/ios.cipher.js";

const PROFILE_ID = "ios.basic";
const SCHEMA_VERSION = 1;

function emit(event) {
  send({
    schemaVersion: SCHEMA_VERSION,
    profileId: PROFILE_ID,
    platform: "ios",
    timestamp: new Date().toISOString(),
    ...event
  });
}

function installModule(moduleId, installer) {
  try {
    emit({ type: "module.installing", moduleId });
    installer({
      emit: event => emit({
        moduleId,
        ...event
      })
    });
    emit({ type: "module.installed", moduleId });
  } catch (e) {
    emit({
      type: "module.error",
      moduleId,
      message: String(e),
      stack: e && e.stack ? String(e.stack) : null
    });
  }
}

emit({ type: "agent.started" });
installModule("ios.cipher", installIosCipherHooks);
emit({ type: "agent.ready" });
