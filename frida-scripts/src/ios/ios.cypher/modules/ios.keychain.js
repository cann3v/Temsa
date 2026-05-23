import { IOS_CIPHER_CONFIG } from "../config.js";
import { attachExport, isNull, toInt } from "../utils/native.js";
import { captureBacktrace } from "../utils/backtrace.js";
import { parseCfType } from "../utils/cf.js";

export function installIosKeychainHooks({ emit, config = IOS_CIPHER_CONFIG.keychain }) {
  if (!config.enabled) return;
  hookSecItemAdd(emit, config);
  hookSecItemCopyMatching(emit, config);
  hookSecItemUpdate(emit, config);
  hookSecItemDelete(emit, config);
}

function hookSecItemAdd(emit, config) {
  attachExport("SecItemAdd", {
    onEnter(args) {
      this.query = args[0];
      this.resultPtr = args[1];
      this.event = baseKeychainEvent("keychain.add", "SecItemAdd", "add", this.context, config);
      this.item = parseKeychainDictionary(this.query, config);
    },
    onLeave(retval) {
      emit({
        ...this.event,
        item: this.item,
        result: readOutCfType(this.resultPtr, config),
        status: toInt(retval)
      });
    }
  }, emit);
}

function hookSecItemCopyMatching(emit, config) {
  attachExport("SecItemCopyMatching", {
    onEnter(args) {
      this.query = args[0];
      this.resultPtr = args[1];
      this.event = baseKeychainEvent("keychain.copy_matching", "SecItemCopyMatching", "copy_matching", this.context, config);
      this.queryObject = parseKeychainDictionary(this.query, config);
    },
    onLeave(retval) {
      emit({
        ...this.event,
        query: this.queryObject,
        result: readOutCfType(this.resultPtr, config),
        status: toInt(retval)
      });
    }
  }, emit);
}

function hookSecItemUpdate(emit, config) {
  attachExport("SecItemUpdate", {
    onEnter(args) {
      this.query = args[0];
      this.attributesToUpdate = args[1];
      this.event = baseKeychainEvent("keychain.update", "SecItemUpdate", "update", this.context, config);
      this.queryObject = parseKeychainDictionary(this.query, config);
      this.updateObject = parseKeychainDictionary(this.attributesToUpdate, config);
    },
    onLeave(retval) {
      emit({
        ...this.event,
        query: this.queryObject,
        attributesToUpdate: this.updateObject,
        status: toInt(retval)
      });
    }
  }, emit);
}

function hookSecItemDelete(emit, config) {
  attachExport("SecItemDelete", {
    onEnter(args) {
      this.query = args[0];
      this.event = baseKeychainEvent("keychain.delete", "SecItemDelete", "delete", this.context, config);
      this.queryObject = parseKeychainDictionary(this.query, config);
    },
    onLeave(retval) {
      emit({
        ...this.event,
        query: this.queryObject,
        status: toInt(retval)
      });
    }
  }, emit);
}

function baseKeychainEvent(event, api, operation, context, config) {
  return {
    type: "telemetry.event",
    category: "storage",
    event,
    api,
    operation,
    backtraceType: config.includeBacktrace ? "native" : null,
    backtrace: captureBacktrace(context, config.includeBacktrace)
  };
}

function parseKeychainDictionary(value, config) {
  const parsed = parseCfType(value, {
    maxDepth: config.maxDepth || 5,
    maxDataLength: config.maxDataLength || 512
  });
  if (parsed === null) return null;
  return parsed.type === "dictionary" ? parsed.value : parsed;
}

function readOutCfType(resultPtr, config) {
  if (resultPtr === null || resultPtr === undefined || isNull(resultPtr)) return null;
  try {
    const value = resultPtr.readPointer();
    if (value.isNull()) return null;
    const parsed = parseCfType(value, {
      maxDepth: config.maxDepth || 5,
      maxDataLength: config.maxDataLength || 512
    });
    if (parsed === null) return null;
    if (parsed.type === "dictionary" || parsed.type === "array" || parsed.type === "data") return parsed.value;
    if (parsed.type === "string" || parsed.type === "boolean" || parsed.type === "number") return parsed.value;
    return parsed;
  } catch (e) {
    return { error: String(e), pointer: resultPtr.toString() };
  }
}
