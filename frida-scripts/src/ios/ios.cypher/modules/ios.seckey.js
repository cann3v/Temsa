import { IOS_CIPHER_CONFIG } from "../config.js";
import { attachExport, isNull } from "../utils/native.js";
import { captureBacktrace } from "../utils/backtrace.js";
import { parseCfType } from "../utils/cf.js";

export function installIosSecKeyHooks({ emit, config = IOS_CIPHER_CONFIG.seckey }) {
  if (!config.enabled) return;

  hookSecKeyCreateSignature(emit, config);
}

function hookSecKeyCreateSignature(emit, config) {
  attachExport("SecKeyCreateSignature", {
    onEnter(args) {
      this.key = args[0];
      this.algorithm = args[1];
      this.dataToSign = args[2];
      this.errorPtr = args[3];

      this.event = {
        type: "telemetry.event",
        category: "crypto",
        event: "seckey.create_signature",
        api: "SecKeyCreateSignature",
        operation: "sign",
        key: parseSecKey(this.key, config),
        algorithm: parseCfValue(this.algorithm, config),
        dataToSign: config.includeData
          ? parseCfValue(this.dataToSign, config)
          : parseCfDataMetadata(this.dataToSign, config),
        backtraceType: config.includeBacktrace ? "native" : null,
        backtrace: captureBacktrace(this.context, config.includeBacktrace)
      };
    },
    onLeave(retval) {
      emit({
        ...this.event,
        signature: config.includeData
          ? parseCfValue(retval, config)
          : parseCfDataMetadata(retval, config),
        error: readOutCfError(this.errorPtr, config),
        success: !isNull(retval)
      });
    }
  }, emit);
}

function parseSecKey(value, config) {
  if (value === null || value === undefined || isNull(value)) {
    return null;
  }

  const parsed = parseCfType(value, {
    maxDepth: config.maxDepth || 5,
    maxDataLength: config.maxDataLength || 512
  });

  if (parsed === null) {
    return {
      pointer: value.toString()
    };
  }

  return {
    pointer: value.toString(),
    value: parsed
  };
}

function parseCfValue(value, config) {
  if (value === null || value === undefined || isNull(value)) {
    return null;
  }

  return parseCfType(value, {
    maxDepth: config.maxDepth || 5,
    maxDataLength: config.maxDataLength || 512
  });
}

function parseCfDataMetadata(value, config) {
  const parsed = parseCfValue(value, config);

  if (parsed === null) {
    return null;
  }

  if (parsed.type === "data" && parsed.value) {
    return {
      type: "data",
      length: parsed.value.length,
      capturedLength: parsed.value.capturedLength,
      truncated: parsed.value.truncated
    };
  }

  return parsed;
}

function readOutCfError(errorPtr, config) {
  if (errorPtr === null || errorPtr === undefined || isNull(errorPtr)) {
    return null;
  }

  try {
    const error = errorPtr.readPointer();

    if (error.isNull()) {
      return null;
    }

    return parseCfType(error, {
      maxDepth: config.maxDepth || 5,
      maxDataLength: config.maxDataLength || 512
    });
  } catch (e) {
    return {
      error: String(e),
      pointer: errorPtr.toString()
    };
  }
}
